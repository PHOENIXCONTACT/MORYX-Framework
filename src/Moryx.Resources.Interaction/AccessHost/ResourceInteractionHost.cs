// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;

#if USE_WCF
using Moryx.Resources.Wcf;
using Moryx.Tools.Wcf;
#else
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Resources.Interaction
{
    /// <summary>
    /// Default interaction resource which hosts the default web service
    /// </summary>
    [Description("Resource to host the default web service")]
    [ResourceRegistration, DependencyRegistration(typeof(IResourceInteraction))]
#if USE_WCF
    public sealed class ResourceInteractionHost : InteractionResource
    {
        /// <inheritdoc />
        protected override Type ServiceContract => typeof(IResourceInteraction);

        /// <summary>
        /// Constructor to set <see cref="HostConfig"/> defaults.
        /// </summary>
        public ResourceInteractionHost()
        {
            HostConfig = new HostConfig
            {
                Endpoint = ResourceInteraction.Endpoint,
                BindingType = ServiceBindingType.WebHttp,
                MetadataEnabled = true
            };
        }
    }
#else
    public sealed class ResourceInteractionHost : Resource
    {
        private IEndpointHost _host;

        /// <summary>
        /// Host factory
        /// </summary>
        public IEndpointHostFactory HostFactory { get; set; }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            _host = HostFactory.CreateHost(typeof(IResourceInteraction), null);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();

            _host.Start();
        }

        /// <inheritdoc />
        protected override void OnDispose()
        {
            _host.Stop();

            base.OnDispose();
        }
    }
#endif
}
