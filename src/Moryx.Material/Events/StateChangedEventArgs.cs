// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;

namespace Moryx.Material.Events;

/// <summary>
/// Event args for <see cref="IMaterialContainer.StateChanged"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of <see cref="StateChangedEventArgs"/>.
/// </remarks>
public class StateChangedEventArgs(IMaterialContainer container, StateInformation? oldState, StateInformation newState) :
    MaterialContainerEventArgs(container)
{
    /// <summary>
    /// Previous state information (may be null on initial transition).
    /// </summary>
    public StateInformation? PreviousStateInformation { get; } = oldState;

    /// <summary>
    /// New state information
    /// </summary>
    public StateInformation NewStateInformation { get; } = newState;
}
