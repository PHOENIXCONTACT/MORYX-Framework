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

        void IStateContext.SetState(IState state)
        {
            CurrentState = (IDriverState)state;
            StateChanged?.Invoke(this, CurrentState);
        }

        /// <inheritdoc />
        public event EventHandler<IDriverState> StateChanged;
    }
}
