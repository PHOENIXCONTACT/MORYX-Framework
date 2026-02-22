// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Resources.Attributes;

/// <summary>
/// Specifies the synchronization behavior for a resource's digital twin
/// across multiple application instances.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ResourceSynchronizationAttribute(string SynchronizationTypeId): Attribute
{
    /// <summary>
    /// Defines the property serialization strategy for this resource.
    /// </summary>
    public SynchronizationMode Mode { get; set; }

    /// <summary>
    /// Unique identifier for the resource synchronization type.
    /// </summary>
    public string SynchronizationTypeId { get; } = SynchronizationTypeId;
}

/// <summary>
/// Defines the property serialization strategy.
/// </summary>
public enum SynchronizationMode
{
    /// <summary>
    /// The entire resource object, including all its properties, is serialized and synchronized.
    /// </summary>
    Full,

    /// <summary>
    /// Only properties explicitly marked with the [SynchronizableMember] attribute will be 
    /// serialized and synchronized.
    /// </summary>
    Selective
}