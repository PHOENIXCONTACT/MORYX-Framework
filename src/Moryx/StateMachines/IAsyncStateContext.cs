// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.StateMachines;

/// <summary>
/// Represents the context of a state machine including a typed state property
/// </summary>
public interface IAsyncStateContext
{
    /// <summary>
    /// Update state on context
    /// </summary>
    Task SetStateAsync(StateBase state, CancellationToken cancellationToken);
}
