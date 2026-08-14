// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Event args for <see cref="Resource.Changed"/>
/// </summary>
public class ResourceChangedEventArgs : EventArgs
{
    /// <summary>
    /// If true, the resource will be saved to storage.
    /// </summary>
    public bool Save { get; init; }

    /// <summary>
    /// Name of the property that changed, or null if not specified
    /// </summary>
    public string PropertyName { get; init; }
}
