// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Message send by the resource managment when it completed an activity
    /// </summary>
    public class ActivityCompleted : Session, ICompletableSession
    {
        internal ActivityCompleted(Activity completed, Session currentSession)
            : base(currentSession)
        {
            CompletedActivity = completed;
        }

        /// <summary>
        /// Activity that was completed
        /// </summary>
        public Activity CompletedActivity { get; }

        /// <summary>
        /// Complete the current sequence to await new ready to work
        /// </summary>
        public SequenceCompleted CompleteSequence(Process process, bool processActive, params long[] nextCells)
        {
            // Ignore process as its still set
            return new SequenceCompleted(this, processActive, nextCells);
        }
    }
}
