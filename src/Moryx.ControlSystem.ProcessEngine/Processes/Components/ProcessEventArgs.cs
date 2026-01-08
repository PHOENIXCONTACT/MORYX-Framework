// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// <see cref="EventArgs"/> for process changes
    /// </summary>
    internal class ProcessEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventArgs"/> class.
        /// </summary>
        public ProcessEventArgs(ProcessData processData, ProcessState trigger)
        {
            ProcessData = processData;
            Trigger = trigger;
        }

        /// <summary>
        /// THe changed or added process
        /// </summary>
        public ProcessData ProcessData { get; }

        /// <summary>
        /// State at the time of triggering
        /// </summary>
        public ProcessState Trigger { get; }
    }
}
