// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Modules
{
    /// <summary>
    /// Exception thrown if server module wasn't ready for requested operation
    /// </summary>
    public class HealthStateException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HealthStateException()
        {
        }

        /// <summary>
        /// Create a new instance of the <see cref="HealthStateException"/> to notify a facade user of an
        /// invalid operation
        /// </summary>
        /// <param name="current">Current state of the module</param>
        public HealthStateException(ServerModuleState current)
        {
            Current = current;
        }

        /// <summary>
        /// Current state of the module
        /// </summary>
        public ServerModuleState Current { get; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message => $"Current HealthState {Current} of service does not allow requested operation.";
    }
}
