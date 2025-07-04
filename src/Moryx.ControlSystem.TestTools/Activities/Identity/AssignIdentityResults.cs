﻿namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Results of identity assignment
    /// </summary>
    public enum AssignIdentityResults
    {
        /// <summary>
        /// Production step was successful
        /// </summary>
        Assigned = 0,

        /// <summary>
        /// Failed to assign identity
        /// </summary>
        FailedToAssign = 1,

        /// <summary>
        /// Article was not found
        /// </summary>
        UnknownTarget = 2,

        /// <summary>
        /// The activity could not be started at all.
        /// </summary>
        TechnicalFailure = 3
    }
}