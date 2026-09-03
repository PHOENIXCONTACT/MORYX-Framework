// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Proxies;

/// <summary>
/// Mixin that Castle adds to every generated proxy via <see cref="ResourceProxyBuilder"/>.
/// Implements <see cref="IResource"/> base members and provides the detach lifecycle.
/// Event wiring to the target is done by <see cref="ResourceProxyBuilder.Build"/> at creation time.
/// </summary>
internal class ResourceProxy : IResourceProxy
{
    private readonly List<(EventInfo Event, Delegate Handler)> _eventHandlers = new();

    /// <summary>
    /// Back-reference to the Castle-generated proxy object, set after creation
    /// so that event senders are replaced with the proxy identity.
    /// </summary>
    public IResource ProxyReference { get; set; }

    /// <summary>
    /// The real resource instance this proxy represents.
    /// </summary>
    public Resource ProxyTarget { get; private set; }

    public ResourceProxy(Resource target)
    {
        ProxyTarget = target;
        ProxyTarget.CapabilitiesChanged += OnCapabilitiesChanged;
    }

    long IResource.Id => (ProxyTarget ?? throw new ProxyDetachedException()).Id;

    string IResource.Name => (ProxyTarget ?? throw new ProxyDetachedException()).Name;

    public ICapabilities Capabilities => (ProxyTarget ?? throw new ProxyDetachedException()).Capabilities;

    /// <summary>
    /// Register an event handler subscribed on the target, so it can be unsubscribed on detach.
    /// Called by <see cref="ResourceProxyBuilder"/> during proxy creation.
    /// </summary>
    public void RegisterEventHandler(EventInfo eventInfo, Delegate handler)
    {
        _eventHandlers.Add((eventInfo, handler));
    }

    /// <summary>
    /// Unsubscribe all event handlers and release the target
    /// </summary>
    public void DetachProxy()
    {
        if (ProxyTarget == null)
        {
            return;
        }

        ProxyTarget.CapabilitiesChanged -= OnCapabilitiesChanged;

        foreach (var (eventInfo, handler) in _eventHandlers)
        {
            eventInfo.GetRemoveMethod(nonPublic: true)?.Invoke(ProxyTarget, [handler]);
        }

        _eventHandlers.Clear();

        ProxyTarget = null;
    }

    private void OnCapabilitiesChanged(object sender, ICapabilities e)
    {
        CapabilitiesChanged?.Invoke(ProxyReference ?? this, e);
    }

    public override string ToString() => ProxyTarget?.ToString() ?? "Detached Proxy";

    public event EventHandler<ICapabilities> CapabilitiesChanged;
}
