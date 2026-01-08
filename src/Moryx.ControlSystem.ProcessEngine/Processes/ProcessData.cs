// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.Workplans;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Decorator for processes holding the internal data belonging to a process.
    /// </summary>
    internal class ProcessData : IEquatable<ProcessData>
    {
        /// <summary>
        /// All activities of the process
        /// </summary>
        private readonly List<ActivityData> _activities = new(64);

        /// <summary>
        /// Create wrapper for process object
        /// </summary>
        /// <param name="process"></param>
        public ProcessData(Process process)
        {
            Process = process;
        }

        /// <summary>
        /// The process identifier
        /// </summary>
        public long Id
        {
            get => Process.Id;
            set => Process.Id = value;
        }

        /// <summary>
        /// Flag if the entity was already created
        /// </summary>
        public bool EntityCreated { get; set; }

        /// <summary>
        /// Incrementing value of activities
        /// </summary>
        public int ActivityIndex { get; set; }

        /// <summary>
        /// Optional reference to an external entity
        /// </summary>
        public long ReferenceId { get; set; }

        /// <summary>
        /// Targets of the process based on its open activities
        /// </summary>
        public IReadOnlyList<ICell> NextTargets()
        {
            lock (_activities)
            {
                // Get targets for all open activities
                return _activities.Where(a => a.State <= ActivityState.Running)
                    .SelectMany(a => a.Targets).Distinct().ToList();
            }
        }

        /// <summary>
        /// The process this data belongs to
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// Short cut property to <see cref="Process.Recipe"/>
        /// </summary>
        public IRecipe Recipe
        {
            get => Process.Recipe;
            set => Process.Recipe = value;
        }

        /// <summary>
        /// Workflow engine of this process
        /// </summary>
        public IWorkplanEngine Engine { get; set; }

        /// <summary>
        /// Reference to the job
        /// </summary>
        public IJobData Job { get; set; }

        private ProcessState _state;
        /// <summary>
        /// The state of this process.
        /// </summary>
        public ProcessState State
        {
            get { return _state; }
            set
            {
                _state = value;

                // Update Start and End when altering the process state
                if (_state == ProcessState.Running)
                    Started = Activities.FirstOrDefault()?.Tracing.Started ?? DateTime.Now;
                else if (_state >= ProcessState.Success)
                    Completed = Activities.LastOrDefault()?.Tracing.Completed ?? DateTime.Now;
            }
        }

        /// <summary>
        /// Time the process was started
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// Time the process was completed
        /// </summary>
        public DateTime Completed { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// Flag that process was involved in Rework production
        /// </summary>
        public bool Rework { get; set; }

        /// <summary>
        /// Add a new activity
        /// </summary>
        /// <param name="activity"></param>
        public void AddActivity(ActivityData activity)
        {
            lock (_activities)
                _activities.Add(activity);
        }

        /// <summary>
        /// Current number of activities in this process
        /// </summary>
        public int ActivityCount() => _activities.Count;

        /// <summary>
        /// Current number of activities that match the given predicate
        /// </summary>
        public int ActivityCount(Func<ActivityData, bool> predicate)
        {
            lock (_activities)
                return _activities.Count(predicate);
        }

        /// <summary>
        /// The current and the finished decorated activities of this process.
        /// </summary>
        public IReadOnlyList<ActivityData> Activities
        {
            get
            {
                lock (_activities)
                    return _activities.ToArray();
            }
        }

        /// <summary>
        /// Sessions that were reported for this process from a cell
        /// </summary>
        internal List<ResourceAndSession> ReportedSessions { get; } = new();

        /// <summary>
        /// Raise the <see cref="ActivityChanged"/> event
        /// </summary>
        public void RaiseActivityChanged(ActivityData activityData)
        {
            ActivityChanged?.Invoke(this, activityData);
        }

        /// <summary>
        /// Event raised when an activity from this process changes its state
        /// </summary>
        public event EventHandler<ActivityData> ActivityChanged;

        public bool Equals(ProcessData other)
        {
            return Id == other?.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((ProcessData)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
