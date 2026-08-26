// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Castle.DynamicProxy;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Proxies;

/// <summary>
/// Builds resource proxies using Castle.DynamicProxy.
/// </summary>
internal class ResourceProxyBuilder
{
    private static readonly ProxyGenerator _generator = new();

    /// <summary>
    /// Create a proxy instance for the given resource
    /// </summary>
    public static IResourceProxy Build(Resource target, IReadOnlyList<Type> interfaces, IResourceTypeController typeController)
    {
        var mixin = new ResourceProxy(target);

        // Build proxy options with the mixin providing IResource + IResourceProxy
        var options = new ProxyGenerationOptions();
        options.AddMixinInstance(mixin);
        options.BaseTypeForInterfaceProxy = typeof(ResourceProxyBase);

        // Separate primary and additional interfaces
        var primaryInterface = interfaces[0];
        var additionalInterfaces = interfaces.Skip(1).ToArray();

        // Create interceptor with deferred proxy reference
        var interceptor = new ResourceInterceptor(mixin, typeController);

        var proxy = (IResourceProxy)_generator.CreateInterfaceProxyWithoutTarget(
            primaryInterface, additionalInterfaces, options, interceptor);

        // Complete initialization: set proxy reference for sender replacement in events
        mixin.ProxyReference = proxy;

        // Wire up target events to forward through the interceptor
        WireTargetEvents(target, interfaces, mixin, interceptor);

        return proxy;
    }

    /// <summary>
    /// Subscribe to all events on the target resource and forward them through the interceptor
    /// </summary>
    private static void WireTargetEvents(Resource target, IReadOnlyList<Type> interfaces,
        ResourceProxy mixin, ResourceInterceptor interceptor)
    {
        var processedEvents = new HashSet<string>();

        foreach (var iface in interfaces)
        {
            foreach (var eventInfo in iface.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
            {
                // Use qualified key to support same-named events on different interfaces
                var eventKey = ResourceInterceptor.CreateEventKey(iface, eventInfo.Name);

                // Skip if already processed (same event inherited on multiple interfaces)
                if (!processedEvents.Add(eventKey))
                {
                    continue;
                }

                var handler = CreateEventForwarder(eventInfo, eventKey, interceptor);
                if (handler is null)
                {
                    continue;
                }

                // Use the add-method of the interface - the runtime resolves to the correct
                // implementation, including explicit interface implementations.
                eventInfo.GetAddMethod(nonPublic: true)!.Invoke(target, [handler]);
                mixin.RegisterEventHandler(eventInfo, handler);
            }
        }
    }

    /// <summary>
    /// Create a delegate that forwards an event from the target to the interceptors RaiseEvent
    /// </summary>
    private static Delegate CreateEventForwarder(EventInfo targetEvent, string eventName, ResourceInterceptor interceptor)
    {
        var handlerType = targetEvent.EventHandlerType!;

        if (handlerType == typeof(EventHandler))
        {
            return new EventHandler((sender, args) => interceptor.RaiseEvent(eventName, sender, args));
        }

        // EventHandler<T>
        if (handlerType.IsGenericType && handlerType.GetGenericTypeDefinition() == typeof(EventHandler<>))
        {
            var argType = handlerType.GetGenericArguments()[0];
            return CreateGenericForwarder(argType, eventName, interceptor);
        }

        return null;
    }

    private static Delegate CreateGenericForwarder(Type argType, string eventName, ResourceInterceptor interceptor)
    {
        var method = typeof(ResourceProxyBuilder)
            .GetMethod(nameof(CreateTypedForwarder), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(argType);

        return (Delegate)method.Invoke(null, [eventName, interceptor]);
    }

    private static EventHandler<TArg> CreateTypedForwarder<TArg>(string eventName, ResourceInterceptor interceptor)
    {
        return (sender, args) => interceptor.RaiseEvent(eventName, sender, args);
    }
}
