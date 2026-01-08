// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Activities;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Message send by the resource when it
    /// </summary>
    public class ReadyToWork : Session, ICompletableSession
    {
        /// <summary>
        /// Signal ready to work
        /// </summary>
        internal ReadyToWork(ActivityClassification classification, ReadyToWorkType type, ProcessReference reference, IConstraint[] constraints)
            : base(classification, reference)
        {
            ReadyToWorkType = type;
            Constraints = constraints;
        }

        /// <summary>
        /// Constructor to resume session with new ready to work
        /// </summary>
        /// <param name="currentSession"></param>
        internal ReadyToWork(Session currentSession)
            : base(currentSession)
        {
            // Constraints are dropped on resume
            Constraints = EmptyConstraints;
            // We always resume a session as pull
            ReadyToWorkType = ReadyToWorkType.Pull;
        }

        /// <summary>
        /// Gets or sets the type of the ready to work.
        /// </summary>
        /// <value>
        /// The type of the ready to work.
        /// </value>
        public ReadyToWorkType ReadyToWorkType { get; }

        /// <summary>
        /// Checks if the context matches to the constrains given by the resource
        /// </summary>
        public IConstraint[] Constraints { get; }

        /// <summary>
        /// Creates the start activity message to send an activity to a resource.
        /// </summary>
        /// <param name="activity">The activity.</param>
        public ActivityStart StartActivity(Activity activity)
        {
            return new ActivityStart(this, activity) { Process = activity.Process };
        }

        /// <summary>
        /// Creates the SequenceCompleted message
        /// </summary>
        public SequenceCompleted CompleteSequence(Process process, bool processActive, params long[] nextCells)
        {
            return new SequenceCompleted(this, processActive, nextCells) { Process = process };
        }

        /// <summary>
        /// Interrupt the currently running session
        /// </summary>
        public NotReadyToWork PauseSession()
        {
            return new NotReadyToWork(this);
        }
    }
}
