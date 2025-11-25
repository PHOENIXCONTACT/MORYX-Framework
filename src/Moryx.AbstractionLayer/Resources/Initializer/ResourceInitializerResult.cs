// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Result of the resource initialization
/// </summary>
public class ResourceInitializerResult
{
    /// <summary>
    /// If true, the resources will not be saved, but are assumed to already have been saved within the initializer
    /// </summary>
    public bool Saved { get; set; }

    /// <summary>
    /// Initialized resources, only roots should be returned as resources are saved recursively
    /// </summary>
    public IReadOnlyList<Resource> InitializedResources { get; set; }
}
