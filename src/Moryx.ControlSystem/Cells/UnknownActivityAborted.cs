// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Message send by the resource managment when it aborted an activity for an
    /// unkown session.
    /// </summary>
    public class UnknownActivityAborted : ActivityCompleted
    {
        internal UnknownActivityAborted(IActivity aborted, Session wrapper)
            : base(aborted, wrapper)
        {
            aborted.Fail();
            AbortedActivity = aborted;
        }

        /// <summary>
        /// Activity that was aborted
        /// </summary>
        public IActivity AbortedActivity { get; }
    }
}
