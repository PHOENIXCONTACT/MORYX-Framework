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
    private readonly IResourceTypeController _typeController;

    // Cache for resolved target methods to avoid repeated GetInterfaceMap calls
    private readonly Dictionary<MethodInfo, MethodInfo> _methodCache = new();

    // Event delegate fields: eventName -> multicast delegate
    private readonly Dictionary<string, Delegate> _eventDelegates = new();

    public ResourceInterceptor(ResourceProxy mixin, IResourceTypeController typeController)
    {
        _mixin = mixin;
        _typeController = typeController;
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

        // MethodInfo.Invoke wraps any target exception in TargetInvocationException.
        // Unwrap it so callers see the original exception with its stack trace.
        object result;
        try
        {
            result = targetMethod.Invoke(target, arguments);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            // Rethrow the original exception preserving its stack trace.
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            return; // unreachable
        }

        // Convert resource-typed return values to proxies
        if (result is not null && IsResourceReference(method.ReturnType))
        {
            result = ConvertResult(result, method.ReturnType);
        }

        invocation.ReturnValue = result;
    }

    /// <summary>
    /// Display name used by <see cref="ResourceProxyBase.ToString"/>
    /// </summary>
    public string GetDisplayName() => _mixin.ToString();

    private MethodInfo ResolveTargetMethod(Type targetType, MethodInfo interfaceMethod)
    {
        // For constructed generic methods, resolve the definition and then construct with the type arguments
        if (interfaceMethod.IsGenericMethod && !interfaceMethod.IsGenericMethodDefinition)
        {
            var definition = interfaceMethod.GetGenericMethodDefinition();
            var targetDefinition = ResolveTargetMethod(targetType, definition);
            return targetDefinition.MakeGenericMethod(interfaceMethod.GetGenericArguments());
        }

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
            var converted = resourceArgs.Select(r => ConvertToProxy(r)).ToArray();
            args = CastCollection(converted, expectedArgType);
        }
        else if (args is IResource resource)
        {
            args = ConvertToProxy(resource);
        }

        handler.DynamicInvoke(_mixin.ProxyReference, args);
    }

    #endregion

    #region Resource Reference Conversion

    private IResource ConvertToProxy(IResource instance)
    {
        if (instance is null)
        {
            return null;
        }

        return _typeController.GetProxy((Resource)instance);
    }

    private static bool IsResourceReference(Type type)
    {
        return typeof(IResource).IsAssignableFrom(type)
               || typeof(IEnumerable<IResource>).IsAssignableFrom(type);
    }

    private object ConvertResult(object result, Type returnType)
    {
        if (result is IEnumerable<IResource> collection and not IResource)
        {
            return CastCollection(collection.Select(r => ConvertToProxy(r)).ToArray(), returnType);
        }

        if (result is IResource singleResource)
        {
            return ConvertToProxy(singleResource);
        }

        return result;
    }

    private static object ExtractArgument(object arg, Type parameterType)
    {
        if (arg is IEnumerable<IResource> collection and not IResource)
        {
            var extracted = collection.Select(r => (IResource)ExtractFromProxy(r)).ToArray();
            return CastCollection(extracted, parameterType);
        }

        if (arg is IResource resource)
        {
            return ExtractFromProxy(resource);
        }

        return arg;
    }

    private static IResource ExtractFromProxy(IResource instance)
    {
        if (instance is IResourceProxy proxy)
        {
            return proxy.ProxyTarget;
        }

        return instance;
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
