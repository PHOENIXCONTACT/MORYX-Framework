// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.StateMachines;

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base class for devices to reduce boilerplate code
    /// </summary>
    public abstract class Driver : Resource, IDriver, IStateContext
    {
        /// <inheritdoc />
        public IDriverState CurrentState { get; private set; }

        void IStateContext.SetState(StateBase state)
        {
            CurrentState = (IDriverState)state;
            StateChanged?.Invoke(this, CurrentState);

            OnStateChanged();
        }

        /// <summary>
        /// Will be called after the state change
        /// </summary>
        protected virtual void OnStateChanged()
        {
        }

        /// <inheritdoc />
        public event EventHandler<IDriverState> StateChanged;
    }

    /// <summary>
    /// Base class for devices to reduce boilerplate code
    /// </summary>
    public abstract class AsyncDriver : Resource, IDriver, IAsyncStateContext
    {
        /// <inheritdoc />
        public IDriverState CurrentState { get; private set; }

        Task IAsyncStateContext.SetStateAsync(StateBase state)
        {
            CurrentState = (IDriverState)state;
            StateChanged?.Invoke(this, CurrentState);

            return OnStateChanged();
        }

        /// <summary>
        /// Will be called after the state change
        /// </summary>
        protected virtual Task OnStateChanged()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public event EventHandler<IDriverState> StateChanged;

    }
}
