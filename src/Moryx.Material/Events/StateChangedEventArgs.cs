// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;

namespace Moryx.Material;

/// <summary>
/// Event args for <see cref="IMaterialContainer.StateChanged"/>.
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    /// <summary>
    /// The container which changed.
    /// </summary>
    public IMaterialContainer Container { get; }

    /// <summary>
    /// Previous state (may be null on initial transition).
    /// </summary>
    public MaterialContainerStateBase? OldState { get; }

    /// <summary>
    /// New state.
    /// </summary>
    public MaterialContainerStateBase NewState { get; }

    /// <summary>
    /// Creates a new instance of <see cref="StateChangedEventArgs"/>.
    /// </summary>
    public StateChangedEventArgs(IMaterialContainer container, MaterialContainerStateBase? oldState, MaterialContainerStateBase newState)
    {
        Container = container;
        OldState = oldState;
        NewState = newState;
    }
}