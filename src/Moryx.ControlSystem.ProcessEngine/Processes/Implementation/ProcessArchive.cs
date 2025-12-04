// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Workplans;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Model.Repositories;
using ProcessContext = Moryx.ControlSystem.ProcessEngine.Model.ProcessContext;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Central component providing access to the processes
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IProcessArchive))]
    internal class ProcessArchive : IProcessArchive
    {
        /// <summary>
        /// Product management to access recipes
        /// </summary>
        public IProductManagement ProductManagement { get; set; }

        /// <summary>
        /// Injected JobList reference to access running jobs
        /// </summary>
        public IJobDataList JobList { get; set; }

        /// <summary>
        /// Injected ActivityPool to access running processes
        /// </summary>
        public IActivityDataPool ActivityPool { get; set; }

        /// <summary>
        /// Injected context factory to access completed jobs
        /// </summary>
        public IUnitOfWorkFactory<ProcessContext> UnitOfWorkFactory { get; set; }

        /// <summary>
        /// Process storage to load activities
        /// </summary>
        public IProcessStorage ProcessStorage { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }

        public IReadOnlyList<IProcess> GetProcesses(ProductInstance productInstance)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var processRepo = uow.GetRepository<IProcessEntityRepository>();

                var query = (from processEntity in processRepo.Linq
                             where processEntity.ReferenceId == productInstance.Id
                             select new { processEntity.Id, processEntity.Job.RecipeId }).ToList();

                var processes = new List<IProcess>();
                foreach (var match in query)
                {
                    var recipe = (ProductionRecipe)ProductManagement.LoadRecipe(match.RecipeId);
                    var process = (ProductionProcess)recipe.CreateProcess();
                    process.Id = match.Id;
                    process.ProductInstance = productInstance;
                    var context = new ProcessWorkplanContext(process);
                    var taskMap = recipe.Workplan.Steps
                        .Select(step => step.CreateInstance(context))
                        .OfType<ITask>().ToDictionary(task => task.Id, task => task);
                    ProcessStorage.FillActivities(uow, process, taskMap);
                    processes.Add(process);
                }

                return processes;
            }
        }

        public IEnumerable<IProcessChunk> GetProcesses(RequestFilter filterType, DateTime start, DateTime end, long[] jobIds)
        {
            // Access database to find all jobs in this time frame
            using (var uow = UnitOfWorkFactory.Create())
            {
                var relevantJobs = GetJobs(uow, filterType, start, end, jobIds);
                if (relevantJobs.Count == 0)
                    yield break;

                foreach (var relevantJob in relevantJobs)
                {
                    // See if we have the job cached
                    yield return JobList.Get(relevantJob.Id) is IProductionJobData job
                        ? new JobDataChunk(job, start, end)
                        : GetFromStorage(uow, relevantJob, start, end);
                }
            }
        }

        /// <summary>
        /// Get all jobs that are relevant for the request
        /// </summary>
        private IReadOnlyList<JobEntity> GetJobs(IUnitOfWork uow, RequestFilter filterType, DateTime start, DateTime end, long[] jobIds)
        {
            var jobRepo = uow.GetRepository<IJobEntityRepository>();

            var all = jobRepo.Linq.Where(j => j.RecipeProvider == ProductManagement.Name); // Filter only production jobs
            if (filterType.HasFlag(RequestFilter.Jobs))
                all = all.Where(j => jobIds.Contains(j.Id));
            if (filterType.HasFlag(RequestFilter.Timed))
                all = all.Where(job => job.Created >= start && job.Created <= end // Job began within the time frame
                                    || job.Updated >= start && job.Updated <= end // Job changed within the time frame
                                    || job.Created <= start && job.Updated >= end); // Job exceeds time frame
            return all.ToList();
        }

        private IProcessChunk GetFromStorage(IUnitOfWork uow, JobEntity jobEntity, DateTime start, DateTime end)
        {
            var processRepo = uow.GetRepository<IProcessEntityRepository>();

            // Otherwise load it on the fly
            var recipe = (IWorkplanRecipe)ProductManagement.LoadRecipe(jobEntity.RecipeId);
            var job = new Job(recipe, jobEntity.Amount)
            {
                Id = jobEntity.Id,
                Classification = JobClassification.Completed
            };
            // Fetch all relevant processes for this job
            // This looks like partial Copy&Paste, but we need to optimize the s**t out of this
            var query = (from processEntity in processRepo.Linq
                         where processEntity.State >= (int)ProcessState.Success && processEntity.JobId == job.Id && processEntity.Activities.Any()
                         let firstActivity = processEntity.Activities.OrderBy(a => a.Started).FirstOrDefault()
                         let lastActivity = processEntity.Activities.OrderByDescending(a => a.Started).FirstOrDefault()
                         where firstActivity.Started >= start && firstActivity.Started <= end // Process started in the time frame
                            || lastActivity.Completed >= start && lastActivity.Completed <= end // Process ended in the time frame
                            || firstActivity.Started <= start && lastActivity.Completed >= end // Process spans over time frame
                         select new { processEntity.Id, processEntity.ReferenceId }).ToList();
            var processes = new IProcess[query.Count];

            // Prepare fake process and task map
            var fakeProcess = recipe.CreateProcess();
            var context = new AbstractionLayer.Processes.ProcessWorkplanContext(fakeProcess);
            var taskMap = recipe.Workplan.Steps
                .Select(step => step.CreateInstance(context))
                .OfType<ITask>().ToDictionary(task => task.Id, task => task);
            // Complete objects with missing values
            for (var index = 0; index < query.Count; index++)
            {
                var process = (ProductionProcess)recipe.CreateProcess();
                process.Id = query[index].Id;
                process.ProductInstance = ProductManagement.GetInstance(query[index].ReferenceId);
                ProcessStorage.FillActivities(uow, process, taskMap);
                processes[index] = process;
            }

            return new ReadonlyProcessChunk(job, processes);
        }

        /// <summary>
        /// Struct that represents a chunk of the full query response
        /// </summary>
        internal class JobDataChunk : IProcessChunk
        {
            private readonly WeakReference<IProductionJobData> _jobData;

            private readonly DateTime _start;
            private readonly DateTime _end;

            /// <summary>
            /// Create a new chunk with a filter for the processes
            /// </summary>
            public JobDataChunk(IProductionJobData jobData, DateTime start, DateTime end)
            {
                _start = start;
                _end = end;

                _jobData = new WeakReference<IProductionJobData>(jobData, false);
            }

            /// <summary>
            /// Try to resolve the weak reference
            /// </summary>
            private IProductionJobData TryGetJob()
            {
                if (_jobData.TryGetTarget(out var job))
                    return job;

                throw new ObjectDisposedException("Target of the chunk has been disposed!");
            }

            /// <summary>
            /// Job the processes belong to
            /// </summary>
            public Job Job => TryGetJob()?.Job;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <inheritdoc />
            public IEnumerator<IProcess> GetEnumerator()
            {
                return new JobDataProcessIterator(TryGetJob().AllProcesses, _start, _end);
            }

            /// <summary>
            /// Iterator for <see cref="IJobData.RunningProcesses"/> that is not affected by collection changes
            /// </summary>
            private class JobDataProcessIterator : IEnumerator<IProcess>
            {
                private int _currentIndex = -1;

                private ProcessData _currentProcess;

                private readonly IReadOnlyList<ProcessData> _processes;

                private readonly DateTime _start;
                private readonly DateTime _end;

                public JobDataProcessIterator(IReadOnlyList<ProcessData> processes, DateTime start, DateTime end)
                {
                    _processes = processes;
                    _start = start;
                    _end = end;
                }

                public bool MoveNext()
                {
                    while (++_currentIndex < _processes.Count)
                    {
                        _currentProcess = _processes[_currentIndex];

                        if (_currentProcess.Completed >= _start && _currentProcess.Started <= _end)
                            return true;

                    }

                    _currentProcess = null;
                    return false;
                }

                public void Reset() => _currentIndex = -1;

                public void Dispose()
                {
                }

                public IProcess Current => _currentProcess.Process;

                object IEnumerator.Current => _currentProcess.Process;
            }
        }

        internal class ReadonlyProcessChunk : IProcessChunk
        {
            private readonly IReadOnlyList<IProcess> _processes;

            public ReadonlyProcessChunk(Job job, IReadOnlyList<IProcess> processes)
            {
                _processes = processes;
                Job = job;
            }

            public Job Job { get; }

            public IEnumerator<IProcess> GetEnumerator()
            {
                return _processes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
