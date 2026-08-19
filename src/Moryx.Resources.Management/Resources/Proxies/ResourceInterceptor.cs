// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Castle.DynamicProxy;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Proxies;

/// <summary>
/// Castle interceptor that forwards all interface calls to the real resource target.
/// Handles property/method forwarding, resource reference conversion, and event plumbing.
/// </summary>
internal class ResourceInterceptor : IInterceptor
{
    private readonly ResourceProxy _mixin;

    // Cache for resolved target methods to avoid repeated GetInterfaceMap calls
    private readonly Dictionary<MethodInfo, MethodInfo> _methodCache = new();

    // Event delegate fields: eventName -> multicast delegate
    private readonly Dictionary<string, Delegate> _eventDelegates = new();

    /// <summary>
    /// Reference to the Castle-generated proxy. Set after proxy creation
    /// so that event senders are replaced with the proxy identity.
    /// </summary>
    public IResource ProxyReference { get; set; }

    public ResourceInterceptor(ResourceProxy mixin)
    {
        _mixin = mixin;
    }

    public void Intercept(IInvocation invocation)
    {
        var method = invocation.Method;

        // Forward mixin-handled methods directly (IResource, ICastleResourceProxy)
        var declaringType = method.DeclaringType;
        if (declaringType != null && (typeof(IResourceProxy).IsAssignableFrom(declaringType)
                                     || declaringType == typeof(IResource)))
        {
            invocation.ReturnValue = method.Invoke(_mixin, invocation.Arguments);
            return;
        }

        // Event add/remove
        if (method.IsSpecialName)
        {
            if (method.Name.StartsWith("add_"))
            {
                HandleEventAdd(method.Name[4..], invocation.Arguments[0] as Delegate);
                return;
            }
            if (method.Name.StartsWith("remove_"))
            {
                HandleEventRemove(method.Name[7..], invocation.Arguments[0] as Delegate);
                return;
            }
        }

        // Detach guard
        var target = _mixin.ProxyTarget ?? throw new ProxyDetachedException();

        // Resolve the method on the actual target type
        var targetMethod = ResolveTargetMethod(target.GetType(), method);

        // Extract proxies from resource-typed arguments
        var arguments = invocation.Arguments;
        var parameters = method.GetParameters();
        for (var i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] is not null && IsResourceReference(parameters[i].ParameterType))
            {
                arguments[i] = ExtractArgument(arguments[i], parameters[i].ParameterType);
            }
        }

        // Invoke on real target
        var result = targetMethod.Invoke(target, arguments);

        // Convert resource-typed return values to proxies
        if (result is not null && IsResourceReference(method.ReturnType))
        {
            result = ConvertResult(result, method.ReturnType);
        }

        invocation.ReturnValue = result;
    }

    private MethodInfo ResolveTargetMethod(Type targetType, MethodInfo interfaceMethod)
    {
        if (_methodCache.TryGetValue(interfaceMethod, out var cached))
        {
            return cached;
        }

        // Try interface mapping first for explicit implementations
        var declaringType = interfaceMethod.DeclaringType;
        if (declaringType is { IsInterface: true })
        {
            var map = targetType.GetInterfaceMap(declaringType);
            for (var i = 0; i < map.InterfaceMethods.Length; i++)
            {
                if (map.InterfaceMethods[i] == interfaceMethod)
                {
                    _methodCache[interfaceMethod] = map.TargetMethods[i];
                    return map.TargetMethods[i];
                }
            }
        }

        // Fallback: find by name and parameter types
        var paramTypes = interfaceMethod.GetParameters().Select(p => p.ParameterType).ToArray();
        var resolved = targetType.GetMethod(interfaceMethod.Name,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
        _methodCache[interfaceMethod] = resolved;
        return resolved;
    }

    #region Event Handling

    private void HandleEventAdd(string eventName, Delegate handler)
    {
        if (handler is null)
        {
            return;
        }

        lock (_eventDelegates)
        {
            _eventDelegates.TryGetValue(eventName, out var existing);
            _eventDelegates[eventName] = Delegate.Combine(existing, handler);
        }
    }

    private void HandleEventRemove(string eventName, Delegate handler)
    {
        if (handler is null)
        {
            return;
        }

        lock (_eventDelegates)
        {
            if (!_eventDelegates.TryGetValue(eventName, out var existing))
            {
                return;
            }

            var updated = Delegate.Remove(existing, handler);
            if (updated is null)
            {
                _eventDelegates.Remove(eventName);
            }
            else
            {
                _eventDelegates[eventName] = updated;
            }
        }
    }

    /// <summary>
    /// Raise the proxy-side event in response to a target event firing.
    /// Replaces the sender with the proxy and converts resource-typed args to proxies.
    /// </summary>
    public void RaiseEvent(string eventName, object sender, object args)
    {
        Delegate handler;
        lock (_eventDelegates)
        {
            if (!_eventDelegates.TryGetValue(eventName, out handler))
            {
                return;
            }
        }

        // Determine expected argument type from the delegate signature
        var handlerType = handler.GetType();
        var expectedArgType = handlerType.IsGenericType
            ? handlerType.GetGenericArguments()[0]
            : typeof(EventArgs);

        // Convert resource-typed event args to proxies
        if (args is IEnumerable<IResource> resourceArgs and not IResource)
        {
            var converted = resourceArgs.Select(r => _mixin.ConvertToProxy(r)).ToArray();
            args = CastCollection(converted, expectedArgType);
        }
        else if (args is IResource)
        {
            args = _mixin.ConvertToProxy((IResource)args);
        }

        handler.DynamicInvoke(ProxyReference, args);
    }

    #endregion

    #region Resource Reference Conversion

    private static bool IsResourceReference(Type type)
    {
        return typeof(IResource).IsAssignableFrom(type)
            || typeof(IEnumerable<IResource>).IsAssignableFrom(type);
    }

    private object ConvertResult(object result, Type returnType)
    {
        if (result is IEnumerable<IResource> collection and not IResource)
        {
            return CastCollection(collection.Select(r => _mixin.ConvertToProxy(r)).ToArray(), returnType);
        }

        if (result is IResource singleResource)
        {
            return _mixin.ConvertToProxy(singleResource);
        }

        return result;
    }

    private static object ExtractArgument(object arg, Type parameterType)
    {
        if (arg is IEnumerable<IResource> collection and not IResource)
        {
            var extracted = collection.Select(r => (IResource)ResourceProxy.ExtractFromProxy(r)).ToArray();
            return CastCollection(extracted, parameterType);
        }

        if (arg is IResource resource)
        {
            return ResourceProxy.ExtractFromProxy(resource);
        }

        return arg;
    }

    private static Array CastCollection(IResource[] items, Type targetType)
    {
        // Determine element type
        var elementType = targetType.IsArray
            ? targetType.GetElementType()!
            : targetType.GetGenericArguments().FirstOrDefault() ?? typeof(IResource);

        // Create typed array
        var typedArray = Array.CreateInstance(elementType, items.Length);
        for (var i = 0; i < items.Length; i++)
        {
            typedArray.SetValue(items[i], i);
        }

        return typedArray;
    }

    #endregion
}
