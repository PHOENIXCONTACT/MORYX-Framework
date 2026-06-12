// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Material;

/// <summary>
/// Event args carrying a single <see cref="IMaterialContainer"/>.
/// </summary>
public class MaterialContainerEventArgs : EventArgs
{
    /// <summary>
    /// Container associated with the event.
    /// </summary>
    public IMaterialContainer Container { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public MaterialContainerEventArgs(IMaterialContainer container)
    {
        Container = container;
    }
}