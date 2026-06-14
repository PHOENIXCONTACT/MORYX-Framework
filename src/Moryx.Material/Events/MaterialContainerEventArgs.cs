// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Events;

/// <summary>
/// Event args for events related to a single <see cref="IMaterialContainer"/>.
/// </summary>
/// <remarks>
/// Creates a new instance.
/// </remarks>
public class MaterialContainerEventArgs(IMaterialContainer container) : EventArgs
{
    /// <summary>
    /// Container associated with the event.
    /// </summary>
    public IMaterialContainer Container { get; } = container;
}
