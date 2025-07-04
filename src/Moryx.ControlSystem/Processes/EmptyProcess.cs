// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Process implementation that can occupy a holder position. Unlike <value>null</value> it
    /// indicates that the position is mechanically prevented from being mounted. The process
    /// behaves like a normal process and can be moved/mounted.
    /// </summary>
    public class EmptyProcess : ProcessReplacement
    {
        /// <summary>
        /// Constant id that represents an <see cref="EmptyProcess"/>
        /// </summary>
        public const long ProcessId = -42;

        /// <summary>
        /// Create empty process with constant value
        /// </summary>
        public EmptyProcess() : base(ProcessId)
        {
        }

        /// <summary>
        /// Create empty process with id. Id might represent a process that should replace this instance later
        /// or simply a key to identify the instance
        /// </summary>
        public EmptyProcess(long processId) : base(processId)
        {
        }
    }
}
