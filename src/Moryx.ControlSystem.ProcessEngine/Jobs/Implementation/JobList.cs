// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    /// <summary>
    /// Default IJobList implementation
    /// </summary>
    [Component(LifeCycle.Singleton, typeof(IJobDataList), typeof(IJobList))]
    internal class JobList : IJobDataList, IJobList
    {
        #region Dependencies

        [UseChild(nameof(JobList))]
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Module configuration to configure shutdown timeout of the job list
        /// </summary>
        public ModuleConfig Config { get; set; }

        #endregion

        #region Fields

        private readonly LinkedList<IJobData> _jobs = new();
        private readonly ReaderWriterLockSlim _jobLock = new(LockRecursionPolicy.SupportsRecursion);

        #endregion

        #region LifeCycle

        /// <inheritdoc cref="IJobDataList"/>
        public void Start()
        {
        }

        /// <inheritdoc />
        public void Stop()
        {
            AwaitCleanJobList();

            _jobLock.EnterWriteLock();
            try
            {
                _jobs.ForEach(DetachFromJobEvents);

                _jobs.Clear();
            }
            finally
            {
                _jobLock.ExitWriteLock();
            }
        }

        private void AwaitCleanJobList()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var timeOut = Config.JobListStopTimeout * 1000;
            while (stopWatch.ElapsedMilliseconds < timeOut && _jobs.Any(j => !j.IsStable))
                Thread.Sleep(1);

            if (_jobs.Any(j => !j.IsStable))
                Logger.LogWarning("Reached shutdown-timout of the {name} after {seconds}s. Unfinished jobs: {jobs}",
                    nameof(JobList), Config.JobListStopTimeout, string.Join(", ", _jobs.Select(j => j.Id)));
        }

        /// <inheritdoc cref="IJobDataList"/>
        public void Dispose()
        {
        }

        #endregion

        /// <inheritdoc cref="IJobDataList"/>
        public IJobData Get(long jobId)
        {
            _jobLock.EnterReadLock();
            try
            {
                return _jobs.FirstOrDefault(j => j.Id == jobId);
            }
            finally
            {
                _jobLock.ExitReadLock();
            }
        }
        public IJobData Next(IJobData reference)
        {
            _jobLock.EnterReadLock();
            try
            {
                return _jobs.Find(reference)?.Next?.Value;

            }
            finally
            {
                _jobLock.ExitReadLock();

            }
        }

        public IJobData Previous(IJobData reference)
        {
            _jobLock.EnterReadLock();
            var next = _jobs.Find(reference)?.Previous?.Value;
            _jobLock.ExitReadLock();
            return next;
        }

        /// <inheritdoc />
        public IEnumerator<IJobData> GetEnumerator()
        {
            return new JobListEnumerable(this, forward: true);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IJobData> Forward(IJobData startJob)
        {
            return new JobListEnumerable(this, forward: true, current: startJob);
        }

        public IEnumerable<IJobData> Backward()
        {
            return new JobListEnumerable(this, forward: false);
        }

        public IEnumerable<IJobData> Backward(IJobData startJob)
        {
            return new JobListEnumerable(this, forward: false, current: startJob);
        }


        Job IJobList.Previous(Job reference)
        {
            return Previous(Get(reference.Id))?.Job;
        }

        Job IJobList.Next(Job reference)
        {
            return Next(Get(reference.Id))?.Job;
        }
        IEnumerable<Job> IJobList.Forward()
        {
            return new JobListEnumerable(this, forward: true).Select(jd => jd.Job);
        }

        IEnumerable<Job> IJobList.Forward(Job startJob)
        {
            return new JobListEnumerable(this, forward: true, current: Get(startJob.Id)).Select(jd => jd.Job);
        }

        IEnumerable<Job> IJobList.Backward()
        {
            return new JobListEnumerable(this, forward: false).Select(jd => jd.Job);
        }

        IEnumerable<Job> IJobList.Backward(Job startJob)
        {
            return new JobListEnumerable(this, forward: false, current: Get(startJob.Id)).Select(jd => jd.Job);
        }

        private event EventHandler<Job> PublicProgressChanged;
        event EventHandler<Job> IJobList.ProgressChanged
        {
            add => PublicProgressChanged += value;
            remove => PublicProgressChanged -= value;
        }

        /// <inheritdoc />
        public void Restore(IReadOnlyList<IJobData> restoredJobs, Action<ModifiedJobsFragment> saveCallback)
        {
            // Insert jobs into the list
            foreach (var jobData in restoredJobs)
            {
                // Do not restore completed jobs
                if (jobData.Classification == JobClassification.Completed)
                    continue;

                // Add and attach
                _jobs.AddLast(jobData);
                AttachToJobEvents(jobData);
            }

            // Write all changes to the database
            saveCallback(new ModifiedJobsFragment(restoredJobs, null));

            // Give the developer some information
            Logger.Log(LogLevel.Information, "Restored {0} jobs!", restoredJobs.Count);
        }

        /// <inheritdoc />
        public void Add(LinkedList<IJobData> jobDatas, JobPosition jobPosition, Action<ModifiedJobsFragment> saveCallback)
        {
            if (jobDatas is null || jobDatas.Count == 0)
                return;

            // Enter write lock because we generate a new sort order
            _jobLock.EnterWriteLock();

            IReadOnlyList<IJobData> newJobs;
            try
            {
                newJobs = ExecuteAdd(jobDatas, jobPosition, saveCallback);
            }
            finally
            {
                _jobLock.ExitWriteLock();
            }

            Log(newJobs, jobPosition);

            // Raise events
            RaiseAdded(newJobs);
        }

        private void Log(IReadOnlyList<IJobData> newJobs, JobPosition jobPosition)
        {
            var addedJobInformation = newJobs.Count == 1 
                ? $"job {newJobs[0].Id}" 
                : $"jobs {string.Join(", ", newJobs.Select(j => j.Id))}";
            Logger.LogInformation(jobPosition.PositionType switch
            {
                JobPositionType.Append => "Added {jobs} to the end of the job list.",
                JobPositionType.Start => "Added {jobs} to the start of the job list.",
                JobPositionType.BeforeOther => "Added {jobs} before job {reference}",
                JobPositionType.AfterOther => "Added {jobs} after job {reference}",
                JobPositionType.AroundExisting => "Added {jobs} expanding existing job(s)",
                JobPositionType.AppendToRecipe => "Added {jobs} after last job with recipe {reference}",
                _ => throw new InvalidOperationException()
            }, addedJobInformation, jobPosition.ReferenceId);
        }

        private IReadOnlyList<IJobData> ExecuteAdd(LinkedList<IJobData> jobDatas, JobPosition jobPosition,
            Action<ModifiedJobsFragment> saveCallback)
        {
            // Extract all new jobs before altering the job list
            var newJobs = jobDatas.Where(j => !_jobs.Contains(j)).ToList();

            // Insert jobs into the list
            var affectedJobs = new List<IJobData>();
            var previousId = jobPosition.PositionType switch
            {
                JobPositionType.Append => Append(jobDatas, affectedJobs),
                JobPositionType.Start => Insert(jobDatas, affectedJobs),
                JobPositionType.BeforeOther => BeforeOther(jobDatas, affectedJobs, jobPosition),
                JobPositionType.AfterOther => AfterOther(jobDatas, affectedJobs, jobPosition),
                JobPositionType.AroundExisting => Expand(jobDatas, affectedJobs),
                JobPositionType.AppendToRecipe => AppendToRecipe(jobDatas, affectedJobs, jobPosition),
                _ => throw new ArgumentOutOfRangeException(nameof(jobPosition), "Unsupported job position!")
            };

            // Save modified collection to database BEFORE any other action
            saveCallback(new ModifiedJobsFragment(affectedJobs, previousId));

            // Attach all job events
            newJobs.ForEach(AttachToJobEvents);

            return newJobs;
        }

        private long? Append(LinkedList<IJobData> jobDatas, List<IJobData> affectedJobs)
        {
            var previousId = _jobs.Last?.Value.Id;
            foreach (var jobData in jobDatas)
            {
                _jobs.AddLast(jobData);
                affectedJobs.Add(jobData);
            }

            return previousId;
        }

        private long? Insert(LinkedList<IJobData> jobDatas, List<IJobData> affectedJobs)
        {
            LinkedListNode<IJobData> node = null;
            foreach (var jobData in jobDatas)
            {
                node = node == null ? _jobs.AddFirst(jobData) : _jobs.AddAfter(node, jobData);
                affectedJobs.Add(jobData);
            }
            if (node.Next != null)
                affectedJobs.Add(node.Next.Value);

            return null;
        }

        private long? BeforeOther(LinkedList<IJobData> jobDatas, List<IJobData> affectedJobs, JobPosition jobPosition)
        {
            var referenceNode = GetById(jobPosition.ReferenceId.Value);

            // TODO: Automatically fix position if we would insert another recipe between a job and its prepare
            // TODO: This solution only fixes a special case and needs to be replaced with proper job positioning
            if (referenceNode.Value.Recipe.Id != jobDatas.First().Recipe.Id
                && referenceNode.Previous?.Value is ISetupJobData setup
                && setup.Recipe.Execution == Setups.SetupExecution.BeforeProduction
                && setup.Recipe.TargetRecipe.Id == referenceNode.Value.Recipe.Id)
                referenceNode = referenceNode.Previous;

            if (referenceNode.Value.Recipe.Id != jobDatas.First().Recipe.Id
                && referenceNode.Previous?.Value is ISetupJobData cleanup
                && cleanup.Recipe.Execution == Setups.SetupExecution.AfterProduction
                && cleanup.Recipe.TargetRecipe.Id == jobDatas.Last().Recipe.Id)
                referenceNode = referenceNode.Previous;

            var previousId = referenceNode.Previous?.Value.Id;
            foreach (var jobData in jobDatas)
            {
                _jobs.AddBefore(referenceNode, jobData);
                affectedJobs.Add(jobData);
            }
            affectedJobs.Add(referenceNode.Value);

            return previousId;
        }

        private long? AfterOther(LinkedList<IJobData> jobDatas, List<IJobData> affectedJobs, JobPosition jobPosition)
        {
            var referenceNode = GetById(jobPosition.ReferenceId.Value);

            // TODO: Automatically fix position if we would insert another recipe between a job and its clean-up
            // TODO: This solution only fixes a special case and needs to be replaced with proper job positioning
            if (referenceNode.Value.Recipe.Id != jobDatas.First().Recipe.Id
                   && referenceNode.Next?.Value is ISetupJobData setup
                   && setup.Recipe.TargetRecipe.Id == referenceNode.Value.Recipe.Id)
                referenceNode = referenceNode.Next;

            var previousId = referenceNode.Value.Id;
            foreach (var jobData in jobDatas)
            {
                referenceNode = _jobs.AddAfter(referenceNode, jobData);
                affectedJobs.Add(jobData);
            }
            if (referenceNode.Next != null)
                affectedJobs.Add(referenceNode.Next.Value);

            return previousId;
        }

        private LinkedListNode<IJobData> GetById(long id)
        {
            var currentNode = _jobs.First;
            while (currentNode != null)
            {
                if (currentNode.Value.Id == id)
                    return currentNode;
                currentNode = currentNode.Next;
            }

            throw new KeyNotFoundException($"Found no job with id: {id} in the job list! Maybe it was removed?");
        }

        private long? Expand(LinkedList<IJobData> expandedList, List<IJobData> affectedJobs)
        {
            // Find the job that was already part of the list and pad from there
            var existingNode = expandedList.First;
            while (existingNode?.Value.Id == 0)
            {
                existingNode = existingNode.Next;
            }
            affectedJobs.Add(existingNode.Value);

            // Find the equivalent from the main list and store previous id
            var equivalentNode = _jobs.Find(existingNode.Value);
            var previousId = equivalentNode.Previous?.Value.Id;

            // Insert preceding jobs from expandedList into global list
            var referenceNode = equivalentNode;
            var currentNode = existingNode.Previous;
            while (currentNode != null)
            {
                affectedJobs.Insert(0, currentNode.Value);
                referenceNode = _jobs.AddBefore(referenceNode, currentNode.Value);
                currentNode = currentNode.Previous;
            }

            // Process all remaining jobs
            referenceNode = equivalentNode;
            currentNode = existingNode.Next;
            while (currentNode != null)
            {
                affectedJobs.Add(currentNode.Value);

                if (currentNode.Value.Id == 0)
                {
                    // Add new jobs to the list
                    referenceNode = _jobs.AddAfter(referenceNode, currentNode.Value);
                }
                else
                {
                    // For existing nodes skip forward till we are in sync again
                    while (referenceNode != null && currentNode.Value != referenceNode.Value)
                        referenceNode = referenceNode.Next;

                    if (referenceNode == null)
                    {
                        // If we reached the end without finding a match, something is broken
                        var jobString = string.Join(";", expandedList.Select(j => $"{j.Id}-{j.Recipe.Name}"));
                        Logger.Log(LogLevel.Error, "Expanding list failed: {0}", jobString);
                        throw new InvalidOperationException($"Expand: Existing job {currentNode.Value.Id} does not match job list position!");
                    }
                }

                currentNode = currentNode.Next;
            }

            // Job directly after the affected jobs is also affected
            if (referenceNode.Next != null)
                affectedJobs.Add(referenceNode.Next.Value);

            return previousId;
        }

        private long? AppendToRecipe(LinkedList<IJobData> jobDatas, List<IJobData> affectedJobs, JobPosition jobPosition)
        {
            // Walk list in reverse and find the last running job with that recipe id
            var referenceNode = _jobs.Last;
            while (referenceNode != null)
            {
                var job = referenceNode.Value;
                if (job.Recipe.Id == jobPosition.ReferenceId)
                    break;

                referenceNode = referenceNode.Previous;
            }

            // Append all jobs after the reference
            var previousId = referenceNode.Value.Id;
            foreach (var jobData in jobDatas)
            {
                referenceNode = _jobs.AddAfter(referenceNode, jobData);
                affectedJobs.Add(jobData);
            }
            if (referenceNode.Next != null)
                affectedJobs.Add(referenceNode.Next.Value);

            return previousId;
        }

        private void AttachToJobEvents(IJobData jobData)
        {
            jobData.StateChanged += OnJobStateChanged;
            jobData.ProgressChanged += OnJobProgressChanged;
        }

        private void DetachFromJobEvents(IJobData jobData)
        {
            jobData.StateChanged -= OnJobStateChanged;
            jobData.ProgressChanged -= OnJobProgressChanged;
        }

        private void OnJobProgressChanged(object sender, EventArgs e)
        {
            var jobData = (IJobData)sender;
            RaiseProgressChanged(jobData);
        }

        private void OnJobStateChanged(object sender, JobStateEventArgs eventArgs)
        {
            var jobData = (IJobData)sender;

            Logger.Log(LogLevel.Debug, "UpdateJobState: {0} Id {1} changed state from {2} to {3} ", jobData.GetType().Name, jobData.Id, eventArgs.PreviousState, eventArgs.CurrentState);

            // Completed is published differently than other states
            RaiseStateChanged(eventArgs);
        }

        public void Remove(IJobData completedJob, Action<ModifiedJobsFragment> saveCallback)
        {

            if (completedJob == null)
            {
                throw new ArgumentNullException(nameof(completedJob));
            }
            Logger.Log(LogLevel.Information, "Removing job {0}", completedJob.Id);

            _jobLock.EnterWriteLock();
            try
            {

                // Update storage
                var affected = new List<IJobData> { completedJob };
                var jobNode = _jobs.Find(completedJob);

                if (jobNode == null)
                {
                    Logger.Log(LogLevel.Warning, "Can't complete job {0} because it is no longer in the job list", completedJob.Id);
                    return;
                }
                if (jobNode.Next != null)
                    affected.Add(jobNode.Next.Value);
                var previousId = jobNode.Previous?.Value.Id;

                // Completed jobs should be removed
                DetachFromJobEvents(completedJob);

                _jobs.Remove(completedJob);

                saveCallback(new ModifiedJobsFragment(affected, previousId));

                Logger.Log(LogLevel.Information, "Removed job {0} from job list", completedJob.Id);

            }
            finally
            {
                _jobLock.ExitWriteLock();
            }
        }

        #region Events

        /// <inheritdoc cref="IJobDataList"/>
        public event EventHandler<IReadOnlyList<IJobData>> Added;
        private void RaiseAdded(IReadOnlyList<IJobData> newJobs)
        {
            Added?.Invoke(this, newJobs);
        }

        /// <inheritdoc cref="IJobDataList"/>
        public event EventHandler<IJobData> ProgressChanged;
        private void RaiseProgressChanged(IJobData updatedJob)
        {
            ProgressChanged?.Invoke(this, updatedJob);

            PublicProgressChanged?.Invoke(this, updatedJob.Job);
        }

        /// <inheritdoc cref="IJobDataList"/>
        public event EventHandler<JobStateEventArgs> StateChanged;
        private void RaiseStateChanged(JobStateEventArgs eventArgs)
        {
            // Event must be wired
            // ReSharper disable PossibleNullReferenceException
            StateChanged(this, eventArgs);
            // ReSharper restore PossibleNullReferenceException
        }

        #endregion

        /// <summary>
        /// Represents the list as <inheritdoc cref="IEnumerable{T}"/> which locks the
        /// LinkedList while iteration.
        /// </summary>
        private class JobListEnumerable : IEnumerable<IJobData>, IEnumerator<IJobData>
        {
            public IJobData Current => _current?.Value;

            object IEnumerator.Current => Current;

            private readonly JobList _parent;
            private readonly bool _forward;
            private LinkedListNode<IJobData> _current;

            private readonly IJobData _reference;

            public JobListEnumerable(JobList parent, bool forward)
            {
                _parent = parent;
                _forward = forward;

                _parent._jobLock.EnterReadLock();
            }

            public JobListEnumerable(JobList parent, bool forward, IJobData current)
                : this(parent, forward)
            {
                _reference = current;
            }

            public IEnumerator<IJobData> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool MoveNext()
            {
                // Find reference first
                if (_current == null && _reference != null)
                {
                    _current = _parent._jobs.Find(_reference);
                }

                if (_current == null)
                    _current = _forward ? _parent._jobs.First : _parent._jobs.Last;
                else
                    _current = _forward ? _current.Next : _current.Previous;

                return _current != null;
            }

            public void Reset()
            {
                _current = null;
            }

            public void Dispose()
            {
                _parent._jobLock.ExitReadLock();
            }
        }
    }
}
