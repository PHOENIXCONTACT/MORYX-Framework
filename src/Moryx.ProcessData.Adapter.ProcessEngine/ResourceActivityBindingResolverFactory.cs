// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;
using Moryx.Bindings;

namespace Moryx.ProcessData.Adapter.ProcessEngine
{
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
            var tracing = ((Activity) source).Tracing.Transform<Tracing>();
            if (tracing == null)
                return null;

            var proxy = _resourceManagement.GetResource<IResource>(tracing.ResourceId);
            // Dirty hack to extract target
            var property = proxy.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p => p.Name == "Target");
            var resource = (IResource)property.GetValue(proxy);
            return resource;
        }

        protected override bool Update(object source, object value)
        {
            throw new System.NotImplementedException();
        }
    }
}
