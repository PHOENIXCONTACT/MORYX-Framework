// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.StateMachines
{
    /// <summary>
    /// Represents the context of a state machine including a typed state property
    /// </summary>
    public interface IStateContext
    {
        /// <summary>
        /// Update state on context
        /// </summary>
        void SetState(IState state);
    }
}
