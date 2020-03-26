// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.AbstractionLayer.Resources;
using Marvin.StateMachines;

namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base class for devices to reduce boilerplate code
    /// </summary>
    public abstract class Driver : Resource, IDriver, IStateContext
    {
        /// <see cref="IDriver"/>
        public IDriverState CurrentState { get; private set; }

        void IStateContext.SetState(IState state)
        {
            CurrentState = (IDriverState) state;
            StateChanged?.Invoke(this, CurrentState);
        }

        /// <seealso cref="IDriver"/>
        public event EventHandler<IDriverState> StateChanged;
    }
}
