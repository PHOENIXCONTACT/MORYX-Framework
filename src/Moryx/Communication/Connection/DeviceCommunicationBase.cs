// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Threading;
using Moryx.Tools;

namespace Moryx.Communication
{

    /// <summary>
    /// Implements a common process of establishing connections of any kind
    /// which is defined by the user.
    /// It handles the need of executing frequent status checks when a connection
    /// could be established or further connection attempts when not.
    /// </summary>
    public abstract class DeviceCommunicationBase : IDeviceCommunication
    {
        /// <inheritdoc/>
        protected int _timerId;

        /// <inheritdoc/>
        protected bool _started = false;

        /// <inheritdoc/>
        protected Func<FunctionResult> _execute;

        /// <inheritdoc />
        public event EventHandler Executed;
        /// <inheritdoc />
        public event EventHandler<FunctionResult> Failed;

        /// <summary>
        /// <see cref="IParallelOperations"/> to enalbe derived types
        /// to execute parallel operations
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <inheritdoc />
        public abstract void Start(Func<FunctionResult> execute);

        /// <inheritdoc />
        public virtual void Stop()
        {
            ParallelOperations.StopExecution(_timerId);
            _started = false;
        }

        /// <summary>
        /// Raises the <see cref="Executed"/> event
        /// </summary>
        protected void RaiseExecutedEvent()
        {
            Executed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Failed"/> event
        /// </summary>
        /// <param name="result">A <see cref="FunctionResult"/> to be evaluated by the caller</param>
        protected void RaiseFailureEvent(FunctionResult result)
        {
            Failed?.Invoke(this, result);
        }
    }
}

