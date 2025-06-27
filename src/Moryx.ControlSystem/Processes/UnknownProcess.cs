// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Process implementation that represents an unknown physical state of position or cell. Instead of <value>null</value>
    /// this can be passed between cells and properly removed from the machine to automatically eliminate the uncertainty.
    /// </summary>
    public class UnknownProcess : ProcessReplacement
    {
        /// <summary>
        /// Constant id if no process id is given
        /// </summary>
        public const long ProcessId = -13;

        /// <summary>
        /// Create unknown process using constant <see cref="ProcessId"/>
        /// </summary>
        public UnknownProcess() : base(ProcessId)
        {
            
        }

        /// <summary>
        /// Create unknown process with id
        /// </summary>
        public UnknownProcess(long processId) : base(processId)
        {
        }
    }
}
