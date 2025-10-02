// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Resources;

// ToDo: Combine with IResourceManagement in MORYX 10
/// <summary>
/// Resource management API used raise change events for resources
/// </summary>
public interface IResourceManagementChanges
{
    /// <summary>
    /// Raised when a resource reports a change (via Resource.RaiseResourceChanged)
    /// or is changed via the ResourceManagement facade (e.g. Modify).
    /// </summary>
    event EventHandler<IResource> ResourceChanged;
}
