// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.StateMachines;

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base class for devices to reduce boilerplate code
    /// </summary>
    public abstract class Driver : Resource, IDriver, IStateContext, IAsyncStateContext
    {
        /// <inheritdoc />
        public IDriverState CurrentState { get; private set; }

        void IStateContext.SetState(StateBase state)
        {
            CurrentState = (IDriverState)state;
            StateChanged?.Invoke(this, CurrentState);

            OnStateChanged();
        }

        Task IAsyncStateContext.SetStateAsync(StateBase state, CancellationToken cancellationToken)
        {
            CurrentState = (IDriverState)state;
            StateChanged?.Invoke(this, CurrentState);

            return OnStateChangedAsync(cancellationToken);
        }

        /// <summary>
        /// Will be called after the state change when <see cref="SyncStateBase"/> is used
        /// </summary>
        protected virtual void OnStateChanged()
        {
        }

        /// <summary>
        /// Will be called after the state change when <see cref="AsyncStateBase"/> is used
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        protected virtual Task OnStateChangedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public event EventHandler<IDriverState> StateChanged;
    }
}
