// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Proxies;

/// <summary>
/// Interface applied to Castle-generated proxies as a mixin.
/// Allows identifying a Castle proxy and accessing its target and lifecycle.
/// </summary>
internal interface IResourceProxy : IResource
{
    /// <summary>
    /// The real resource behind this proxy
    /// </summary>
    Resource ProxyTarget { get; }

    /// <summary>
    /// Unsubscribe event handlers and release the target
    /// </summary>
    void DetachProxy();
}
