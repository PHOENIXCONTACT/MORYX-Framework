// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material.Facade;

/// <summary>
/// Event arguments for events related to a single <see cref="IMaterialContainer"/>.
/// </summary>
/// <param name="container">Container associated with the event.</param>
public class MaterialContainerEventArgs(IMaterialContainer container) : EventArgs
{
    /// <summary>
    /// Container associated with the event.
    /// </summary>
    public IMaterialContainer Container { get; } = container;
}
