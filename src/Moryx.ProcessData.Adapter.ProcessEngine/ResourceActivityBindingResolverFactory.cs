// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Resources;
using Moryx.Bindings;

namespace Moryx.ProcessData.Adapter.ProcessEngine;

internal class ResourceActivityBindingResolverFactory : ActivityBindingResolverFactory
{
    private readonly IResourceManagement _resourceManagement;

    public ResourceActivityBindingResolverFactory(IResourceManagement resourceManagement)
    {
        _resourceManagement = resourceManagement;
    }

    protected override IBindingResolverChain CreateBaseResolver(string baseKey)
    {
        return baseKey switch
        {
            "Resource" => new TracingResourceResolver(_resourceManagement),
            _ => base.CreateBaseResolver(baseKey)
        };
    }
}

internal class TracingResourceResolver : BindingResolverBase
{
    private readonly IResourceManagement _resourceManagement;

    public TracingResourceResolver(IResourceManagement resourceManagement)
    {
        _resourceManagement = resourceManagement;
    }

    protected override object Resolve(object source)
    {
        var tracing = ((Activity)source).Tracing.Transform<Tracing>();
        if (tracing == null)
            return null;

        // Usage of original resource instead of proxy
        var resource = _resourceManagement.GetResourcesUnsafe<IResource>(r => r.Id == tracing.ResourceId)
            .FirstOrDefault();

        return resource;
    }

    protected override bool Update(object source, object value)
    {
        throw new NotImplementedException();
    }
}
