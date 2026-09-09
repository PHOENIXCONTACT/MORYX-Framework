// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Castle.DynamicProxy;

namespace Moryx.Resources.Management.Proxies;

/// <summary>
/// Base class for Castle-generated proxy types.
/// Overrides <see cref="ToString"/> to display the resource identity
/// instead of the Castle proxy type name.
/// </summary>
internal class ResourceProxyBase
{
    /// <inheritdoc />
    public override string ToString()
    {
        // Castle adds the mixin as the first IInterceptor target,
        // so we find the ResourceProxy mixin through the proxy's interceptors
        if (this is not IProxyTargetAccessor accessor)
        {
            return base.ToString();
        }

        foreach (var interceptor in accessor.GetInterceptors())
        {
            if (interceptor is ResourceInterceptor resourceInterceptor)
            {
                return resourceInterceptor.GetDisplayName();
            }
        }

        return base.ToString();
    }
}
