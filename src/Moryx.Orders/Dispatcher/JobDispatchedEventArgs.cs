﻿// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.ControlSystem.Jobs;

namespace Moryx.Orders.Dispatcher
{
    /// <summary>
    /// Event args used to inform about a dispatched job
    /// </summary>
    public class JobDispatchedEventArgs : EventArgs
    {
        /// <summary>
        /// Operation reference for the dispatched job
        /// </summary>
        public Operation Operation { get; }

        /// <summary>
        /// List of dispatched jobs
        /// </summary>
        public IReadOnlyList<Job> Jobs { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="JobDispatchedEventArgs"/>
        /// </summary>
        public JobDispatchedEventArgs(Operation operation, IReadOnlyList<Job> jobs)
        {
            Operation = operation;
            Jobs = jobs;
        }
    }
}