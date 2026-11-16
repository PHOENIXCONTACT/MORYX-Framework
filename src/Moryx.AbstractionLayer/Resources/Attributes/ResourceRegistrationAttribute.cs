// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Simplified plugin registration attribute for resources
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ResourceRegistrationAttribute : PluginAttribute
{
    /// <summary>
    /// ReadOnly Name of component.
    /// For resources the name is the FullName of the type
    /// </summary>
    public new string Name => base.Name;

    /// <summary>
    /// Generic registration with lifecycle <see cref="LifeCycle.Transient"/>
    /// </summary>
    public ResourceRegistrationAttribute()
        : base(LifeCycle.Transient, typeof(IResource))
    {
    }
}
