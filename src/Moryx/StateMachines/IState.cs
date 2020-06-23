// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines
{
    /// <summary>
    /// Interface representing an state machine state
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Will be called while entering the next state
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Will be called while exiting the current state
        /// </summary>
        void OnExit();
    }
}
