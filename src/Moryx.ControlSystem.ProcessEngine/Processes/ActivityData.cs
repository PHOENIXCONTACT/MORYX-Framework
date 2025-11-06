// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Workplans;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Decorator for activities holding the internal data belonging to an activity.
    /// </summary>
    internal class ActivityData
    {
        private ICell _resource;
        private ITask _task;

        public static readonly ICell[] EmptyTargets = [];

        /// <summary>
        /// Wrap this activity
        /// </summary>
        /// <param name="activity"></param>
        public ActivityData(Activity activity)
        {
            Activity = activity;
        }

        /// <summary>
        /// The activity this data belongs to
        /// </summary>
        public Activity Activity { get; }

        /// <summary>
        /// The activity's result or null if there is no result yet.
        /// </summary>
        public ActivityResult Result
        {
            get => Activity.Result;
            set => Activity.Result = value;
        }

        /// <summary>
        /// Shortcut to <see cref="IActivity.Tracing"/>
        /// </summary>
        public Tracing Tracing
        {
            get => Activity.Tracing;
            set => Activity.Tracing = value;
        }

        /// <summary>
        /// The activity's database ID
        /// </summary>
        public long Id
        {
            get => Activity.Id;
            set => Activity.Id = value;
        }

        /// <summary>
        /// Indicator if the activity and its ID was already written to the database
        /// </summary>
        public bool EntityCreated { get; set; }

        /// <summary>
        /// The ID of the Resource this activity shall be or was executed on, or <c>0</c> if no resource was selected yet.
        /// </summary>
        public ICell Resource
        {
            get => _resource;
            set
            {
                _resource = value;

                Tracing.Trace(t => t.ResourceId = _resource.Id);
            }
        }

        /// <summary>
        /// Active session interacting with resource
        /// </summary>
        public Session Session { get; set; }

        /// <summary>
        /// Targets of the activity
        /// </summary>
        public IReadOnlyList<ICell> Targets { get; set; } = EmptyTargets;

        /// <summary>
        /// The required ResourceMode for this activity.
        /// RequiredMode is a bit field. More than one bit may be set.
        /// </summary>
        public ActivityClassification Classification => (Activity as IControlSystemActivity)?.Classification ?? ActivityClassification.Production;

        /// <summary>
        /// Capabilities required by this activity
        /// </summary>
        public ICapabilities RequiredCapabilities => Activity.RequiredCapabilities;

        /// <summary>
        /// The state of this activity.
        /// </summary>
        public ActivityState State { get; set; }

        /// <summary>
        /// The task transition that created this activity. Temporary reference only
        /// </summary>
        public ITask Task
        {
            get => _task;
            set
            {
                _task = value;
                Activity.StepId = _task.Id;
            }
        }

        /// <summary>
        /// The process this activity belongs to.
        /// </summary>
        public ProcessData ProcessData { get; set; }

        /// <summary>
        /// Forward the ToString() call to the Activity
        /// </summary>
        public override string ToString() => Activity.ToString();
    }
}
