// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// Base class for JobScheduler implementations
    /// </summary>
    public abstract class JobSchedulerBase<T> : IJobScheduler
        where T : JobSchedulerConfig
    {
        #region Dependencies

        /// <summary>
        /// Gets or sets the job list.
        /// </summary>
        public IJobList JobList { get; set; }

        #endregion

        /// <summary>
        /// Empty array for jobs without dependencies
        /// </summary>
        private readonly Job[] EmptyDependencies = [];

        /// <summary>
        /// Dependency map that can be helpful for job scheduling
        /// </summary>
        private readonly Dictionary<Job, List<Job>> _dependencies = new();

        /// <summary>
        /// Gets or sets the configuration of the scheduler
        /// </summary>
        protected T Config { get; private set; }

        /// <inheritdoc />
        public virtual void Initialize(JobSchedulerConfig config)
        {
            Config = (T)config;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Check if a certain job has dependencies
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected IReadOnlyList<Job> Dependencies(Job target)
        {
            return _dependencies.ContainsKey(target)
                ? (IReadOnlyList<Job>)_dependencies[target]
                : EmptyDependencies;
        }

        /// <summary>
        /// Add another dependency to the map
        /// </summary>
        protected void AddDependency(Job target, Job dependency)
        {
            if (_dependencies.ContainsKey(target))
            {
                _dependencies[target].Add(dependency);
            }
            else
            {
                _dependencies[target] = new List<Job> { dependency };
            }
        }

        /// <summary>
        /// Remove a dependency from the map
        /// </summary>
        protected bool RemoveDependency(Job target, Job dependency)
        {
            if (_dependencies.ContainsKey(target))
            {
                var removed = _dependencies[target].Remove(dependency);

                if (_dependencies[target].Count == 0)
                    _dependencies.Remove(target);

                return removed;
            }
            return false;
        }

        /// <inheritdoc />
        public abstract IEnumerable<Job> SchedulableJobs(IEnumerable<Job> jobs);

        /// <inheritdoc />
        public abstract void JobsReady(IEnumerable<Job> startableJobs);

        /// <inheritdoc />
        public abstract void JobUpdated(Job job, JobClassification classification);

        /// <inheritdoc />
        public event EventHandler SlotAvailable;
        /// <summary>
        /// Raise the <see cref="SlotAvailable"/> event
        /// </summary>
        protected void RaiseSlotAvailable() => SlotAvailable(this, EventArgs.Empty);

        /// <inheritdoc />
        public event EventHandler<Job> Scheduled;
        /// <summary>
        /// Raise the <see cref="SlotAvailable"/> event
        /// </summary>
        protected void RaiseJobScheduled(Job job) => Scheduled(this, job);

        /// <inheritdoc />
        public event EventHandler<Job> Suspended;
        /// <summary>
        /// Raise the <see cref="SlotAvailable"/> event
        /// </summary>
        protected void RaiseJobSuspended(Job job) => Suspended(this, job);

    }
}
