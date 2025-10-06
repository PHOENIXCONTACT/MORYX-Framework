// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Jobs
{
    /// <summary>
    /// External representation of the current job classification
    /// </summary>
    [Flags]
    public enum JobClassification
    {
        /// <summary>
        /// The job is initial
        /// </summary>
        Idle = 0,

        /// <summary>
        /// The job is startable
        /// </summary>
        Waiting = 1,

        /// <summary>
        /// The job is currently dispatching new processes
        /// </summary>
        Running = 1 << 3,

        /// <summary>
        /// The job is attempting to complete. Last processes will be finished
        /// </summary>
        Completing = 1 << 12,

        /// <summary>
        /// The job is completed. No processes are running and all work is done
        /// </summary>
        Completed = 1 << 16
    }
}
