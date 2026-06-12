// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;

namespace Moryx.Material;

/// <summary>
/// Event args raised when a container transitions between states.
/// </summary>
public class ContainerStateChangedEventArgs : MaterialContainerEventArgs
{
    /// <summary>
    /// State left (may be <c>null</c> if this is the initial state).
    /// </summary>
    public MaterialContainerStateBase? OldState { get; }

    /// <summary>
    /// State entered.
    /// </summary>
    public MaterialContainerStateBase NewState { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public ContainerStateChangedEventArgs(IMaterialContainer container, MaterialContainerStateBase? oldState, MaterialContainerStateBase newState)
        : base(container)
    {
        OldState = oldState;
        NewState = newState;
    }
}