// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Moryx.Runtime.Kestrel
{
    internal class EndpointCollector : IEndpointCollector
    {
        private readonly IActionDescriptorCollectionProvider _provider;

        public EndpointCollector(IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
        }

        public Endpoint[] AllEndpoints(HttpRequest request)
        {
            var addressRoot = $"{request.Scheme}://{request.Host.Host}:{(request.Host.Port != null ? request.Host.Port.ToString() : "80")}";

            return CollectEndpoints(addressRoot);
        }

        internal Endpoint[] CollectEndpoints(string addressRoot)
        {
            var endpoints = new Dictionary<string, Endpoint>();

            foreach (var actionDescriptor in _provider.ActionDescriptors.Items.Where(d => d is ControllerActionDescriptor).Cast<ControllerActionDescriptor>())
            {
                var controllerTypeInfo = actionDescriptor.ControllerTypeInfo;
                var controllerName = actionDescriptor.ControllerTypeInfo.Name;

                if (typeof(Controller).IsAssignableFrom(controllerTypeInfo) && !endpoints.ContainsKey(controllerName))
                {
                    var endpoint = new Endpoint
                    {
                        Service = controllerTypeInfo.GetCustomAttribute<ServiceNameAttribute>()?.Name ?? controllerName,
                        Version = controllerTypeInfo.GetCustomAttribute<ServiceVersionAttribute>()?.Version ?? "1.0.0.0",
                        Path = controllerTypeInfo.GetCustomAttribute<RouteAttribute>()?.Template ?? "",
                        Address = $"{addressRoot}/{controllerTypeInfo.GetCustomAttribute<RouteAttribute>()?.Template ?? ""}"
                    };

                     endpoints.Add(controllerName, endpoint);
                }
            }

            return endpoints.Values.ToArray();
        }
    }
}
