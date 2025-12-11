// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Threading;
using Moryx.Tools;

namespace Moryx.ControlSystem.ProcessEngine.Jobs
{
    [Component(LifeCycle.Singleton, typeof(IJobManager))]
    internal class JobManager : IJobManager, IDisposable
    {
        #region Dependencies

        public IJobStorage JobStorage { get; set; }

        public IJobDataFactory JobDataFactory { get; set; }

        public IJobDataList JobList { get; set; }

        public IJobHandler[] JobHandlers { get; set; }

        public IModuleLogger Logger { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        #endregion

        /// <summary>
        /// Job scheduler used by this job manager
        /// </summary>
        private IJobScheduler _scheduler;

        /// <summary>
        /// Indicator if job processing is active
        /// </summary>
        private bool _running;

        /// <summary>
        /// Thread that works on the task queue
        /// </summary>
        private ParallelOperationsQueue<Action> _taskQueue;

        /// <inheritdoc />
        public void Configure(IJobScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        /// <inheritdoc />
        public bool AcceptingExternalJobs => _running;

        /// <inheritdoc />
        public void Start()
        {
            JobList.StateChanged += OnJobStateChanged;
            _scheduler.SlotAvailable += OnSlotsAvailable;
            _scheduler.Scheduled += OnJobScheduled;
            _scheduler.Suspended += OnJobSuspended;

            // Activate
            _running = true;

            // Create the queue worker AFTER restoring jobs
            _taskQueue = new ParallelOperationsQueue<Action>(task => task.Invoke(), ParallelOperations, Logger);

            // Reload jobs from the database
            ReloadJobs();
        }

        /// <inheritdoc />
        public void Stop()
        {
            _running = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            JobList.StateChanged -= OnJobStateChanged;
            _scheduler.SlotAvailable -= OnSlotsAvailable;
            _scheduler.Scheduled -= OnJobScheduled;
            _scheduler.Suspended -= OnJobSuspended;
        }

        private void ReloadJobs()
        {
            // Load existing jobs
            var jobs = JobStorage.GetAll();
            jobs.ForEach(j => j.Load());

            // Restore on job list
            JobList.Restore(jobs, JobStorage.Save);

            // Inform the scheduler about our new jobs
            _taskQueue.Enqueue(UseAvailableSlots);
        }

        public bool AwaitBoot(int timeoutSec)
        {
            var timeout = timeoutSec * 1000;

            // Stopwatch to abort the loop after configured timeout. That should be enough to restore the last state
            var stopWatch = Stopwatch.StartNew();

            // Loop until all previously running jobs are resumed
            while (stopWatch.ElapsedMilliseconds < timeout && JobList.Any(j => j.Classification < JobClassification.Running && j.RunningProcesses.Count > 0))
            {
                Thread.Sleep(1);
            }
            stopWatch.Stop();

            return stopWatch.ElapsedMilliseconds > timeout;
        }

        public async Task<IReadOnlyList<IProductionJobData>> Add(JobCreationContext context)
        {
            // Split the given templates into chunks within our process limitation
            var jobDatas = new List<IProductionJobData>();
            foreach (var template in context.Templates)
            {
                var amount = (int)template.Amount;
                do
                {
                    // Replace endless jobs with max size jobs
                    var jobSize = (amount < IdShiftGenerator.MaxAmount & amount > 0) ? amount : IdShiftGenerator.MaxAmount;
                    var jobData = (IProductionJobData)JobDataFactory.Create(template, jobSize);
                    jobDatas.Add(jobData);
                    amount -= jobSize;
                } while (amount > 0);
            }

            // Enqueue adding to job list and wait for completion
            var tcs = new TaskCompletionSource();
            _taskQueue.Enqueue(() => AddJobs(jobDatas, context.Position, tcs));

            await tcs.Task;

            // Now schedule the scheduling
            if (_running)
                _taskQueue.Enqueue(UseAvailableSlots);

            // Return the jobs without awaiting the task
            return jobDatas;
        }

        private void AddJobs(List<IProductionJobData> jobDatas, JobPosition position, TaskCompletionSource tcs)
        {
            try
            {
                JobList.Add(new LinkedList<IJobData>(jobDatas), position, JobStorage.Save);
                tcs.SetResult();
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        }

        private void OnSlotsAvailable(object sender, EventArgs args)
        {
            if (_running)
                _taskQueue.Enqueue(UseAvailableSlots);
        }

        private void UseAvailableSlots()
        {
            // Extract all idle jobs
            var jobDatas = JobList.OfType<IProductionJobData>()
                .Where(pj => pj.Classification == JobClassification.Idle).ToList();

            // Filter jobs that can be scheduled
            var jobs = jobDatas.Select(j => j.Job);
            var schedulable = _scheduler.SchedulableJobs(jobs).ToList();
            // Restore order of jobs
            var ordered = jobDatas.Where(jd => schedulable.Any(j => j == jd.Job)).ToList();
            if (ordered.Count == 0)
                return;

            // Split into chunks if schedulable jobs have gaps in JobList
            var totalJobs = new List<IJobData>();
            var linkedJobs = new LinkedList<IJobData>();
            for (var index = 0; index < ordered.Count; index++)
            {
                var jobData = ordered[index];
                // Add to current collection
                linkedJobs.AddLast(jobData);

                // See if there is a next one and it is a direct neighbor
                if (index < ordered.Count - 1 && ordered[index + 1] == JobList.Next(jobData))
                    continue;

                // Handle what we have so far and add to JobList
                foreach (var handler in JobHandlers)
                    handler.Handle(linkedJobs);
                JobList.Add(linkedJobs, JobPosition.Expand, JobStorage.Save);

                // Add to total jobs
                totalJobs.AddRange(linkedJobs);
                // Reset if we have more
                if (index < ordered.Count - 1)
                    linkedJobs = new LinkedList<IJobData>();
            }

            // If non of the available jobs could be readied
            if (totalJobs.Count == 0)
                return;

            // Set ready AFTER they were added to the job list to avoid race conditions
            foreach (var jobData in totalJobs)
                jobData.Ready();

            // Inform the scheduler about our new jobs
            _scheduler.JobsReady(totalJobs.Select(j => j.Job));
        }

        private void OnJobScheduled(object sender, Job e)
        {
            var jobData = JobList.Get(e.Id);
            jobData.Start();
        }

        private void OnJobSuspended(object sender, Job e)
        {
            var jobData = JobList.Get(e.Id);
            jobData.Stop();
        }

        private void OnJobStateChanged(object sender, JobStateEventArgs eventArgs)
        {
            if (eventArgs.CurrentState.Classification < JobClassification.Completed)
            {
                _taskQueue.Enqueue(() => ProcessJobState(eventArgs));
                JobStorage.UpdateState(eventArgs.JobData, eventArgs.CurrentState);
            }
            else if (_running)
            {
                _taskQueue.Enqueue(() => ProcessJobCompleted(eventArgs.JobData));
            }
        }

        private void ProcessJobState(JobStateEventArgs eventArgs)
        {
            // Only classification changes are relevant for the scheduler
            var oldClassification = eventArgs.PreviousState.Classification;
            var classification = eventArgs.CurrentState.Classification;
            if (_running && oldClassification != classification && classification >= JobClassification.Running)
                _scheduler.JobUpdated(eventArgs.JobData.Job, classification);
        }

        private void ProcessJobCompleted(IJobData completedJob)
        {
            try
            {
                _scheduler.JobUpdated(completedJob.Job, JobClassification.Completed);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "Failed to forward completed job to scheduler");
            }

            JobList.Remove(completedJob, JobStorage.Save);

            JobDataFactory.Destroy(completedJob);
        }
    }
}

