// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Communication.Endpoints;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    internal class KestrelEndpointHost : IEndpointHost, IDisposable
    {
        private readonly Type _controller;
        private readonly IContainer _container;
        /// <summary>
        /// Kestrel hosting responsible for the controller
        /// </summary>
        public KestrelEndpointHosting Hosting { get; set; }

        public KestrelEndpointHost(Type endpoint, IContainer container) : this(endpoint, null, container)
        {
        }

        public KestrelEndpointHost(Type endpoint, object config, IContainer container)
        {
            _container = container;

            // If the endpoint is registered, use the implementation for controller matching
            _controller = _container.GetRegisteredImplementations(endpoint).Any()
                ? _container.GetRegisteredImplementations(endpoint).FirstOrDefault()
                : endpoint;
        }

        public void Start()
        {
            Hosting.LinkController(_controller, _container);
        }

        public void Stop()
        {
            Hosting.UnlinkController(_controller, _container);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}