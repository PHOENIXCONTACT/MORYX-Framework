// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
    private IResourceTypeController _typeController;
    private readonly Dictionary<string, Delegate> _eventHandlers = new();

    /// <summary>
    /// Back-reference to the Castle-generated proxy object, set after creation
    /// so that event senders are replaced with the proxy identity.
    /// </summary>
    public IResource ProxyReference { get; set; }

    public Resource ProxyTarget { get; private set; }

    public ResourceProxy(Resource target, IResourceTypeController typeController)
    {
        ProxyTarget = target;
        _typeController = typeController;
        ProxyTarget.CapabilitiesChanged += OnCapabilitiesChanged;
    }

    long IResource.Id => ProxyTarget?.Id ?? throw new ProxyDetachedException();

    string IResource.Name => ProxyTarget?.Name ?? throw new ProxyDetachedException();

    public ICapabilities Capabilities => ProxyTarget?.Capabilities ?? throw new ProxyDetachedException();

    /// <summary>
    /// Register an event handler subscribed on the target, so it can be unsubscribed on detach.
    /// Called by <see cref="ResourceProxyBuilder"/> during proxy creation.
    /// </summary>
    public void RegisterEventHandler(string eventName, Delegate handler)
    {
        _eventHandlers[eventName] = handler;
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

        var targetType = ProxyTarget.GetType();
        foreach (var (eventName, handler) in _eventHandlers)
        {
            var eventInfo = targetType.GetEvent(eventName);
            eventInfo?.RemoveEventHandler(ProxyTarget, handler);
        }
        _eventHandlers.Clear();

        _typeController = null;
        ProxyTarget = null;
    }

    /// <summary>
    /// Convert a resource reference to its proxy
    /// </summary>
    public IResource ConvertToProxy(IResource instance)
    {
        if (instance is null || _typeController is null)
        {
            return null;
        }

        return _typeController.GetProxy((Resource)instance);
    }

    /// <summary>
    /// Extract the real resource from a proxy
    /// </summary>
    public static IResource ExtractFromProxy(IResource instance)
    {
        if (instance is null)
        {
            return null;
        }

        if (instance is IResourceProxy proxy)
        {
            return proxy.ProxyTarget;
        }

        return instance;
    }

    private void OnCapabilitiesChanged(object sender, ICapabilities e)
    {
        CapabilitiesChanged?.Invoke(ProxyReference ?? this, e);
    }

    public override string ToString() => ProxyTarget?.ToString() ?? "Detached Proxy";

    public event EventHandler<ICapabilities> CapabilitiesChanged;
}
