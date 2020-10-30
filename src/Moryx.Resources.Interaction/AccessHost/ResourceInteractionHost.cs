// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Resources.Wcf;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Interaction
{
    /// <summary>
    /// Default interaction resource which hosts the default web service
    /// </summary>
    [Description("Resource to host the default web service")]
    [ResourceRegistration, DependencyRegistration(typeof(IResourceInteraction))]
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
                Endpoint = "resources",
                BindingType = ServiceBindingType.WebHttp,
                MetadataEnabled = true
            };
        }
    }
}
