// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Threading
{
    /// <summary>
    /// Interface to observe the usage of <see cref="IParallelOperations"/>
    /// for diagnostic purposes.
    /// </summary>
    public interface IParallelObserver
    {
        /// <summary>
        /// An operation was passed to <see cref="IParallelOperations.ExecuteParallel(Action)"/> and is awaiting execution by the pool
        /// </summary>
        void Scheduled(Delegate operation, object userState);

        /// <summary>
        /// An operation was passed to <see cref="IParallelOperations.ScheduleExecution(Action, int, int)"/> and is awaiting execution
        /// </summary>
        void Scheduled(Delegate operation, object userState, int delay, int interval);

        /// <summary>
        /// The operation has received CPU time and is starting execution.
        /// </summary>
        void Executing(Delegate operation, object userState);

        /// <summary>
        /// The operation was completed and the thread is returned to the Pool.
        /// </summary>
        void Completed(Delegate operation, object userState);
    }
}
