// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;

namespace Moryx.Material;

/// <summary>
/// Event arguments for <see cref="IMaterialContainer.StateChanged"/>.
/// </summary>
/// <param name="oldState">Previous state information, or <c>null</c> for the initial transition.</param>
/// <param name="newState">New state information after the transition.</param>
public class StateChangedEventArgs(StateInformation? oldState, StateInformation newState)
{
    /// <summary>
    /// Previous state information (may be null on initial transition).
    /// </summary>
    public StateInformation? PreviousStateInformation { get; } = oldState;

    /// <summary>
    /// New state information.
    /// </summary>
    public StateInformation NewStateInformation { get; } = newState;
}
