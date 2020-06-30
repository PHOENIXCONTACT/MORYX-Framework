// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Interaction
{
    /// <summary>
    /// Default interaction resource which hosts the default web service
    /// </summary>
    [Description("Resource to host the default web service")]
    [ResourceRegistration, DependencyRegistration(typeof(IResourceInteraction))]
    public sealed class ResourceInteractionHost : InteractionResource<IResourceInteraction>
    {
        /// <summary>
        /// Constructor to set <see cref="HostConfig"/> defaults.
        /// </summary>
        public ResourceInteractionHost()
        {
            HostConfig = new HostConfig
            {
                Endpoint = "ResourceInteraction",
                BindingType = ServiceBindingType.BasicHttp,
                MetadataEnabled = true,
            };
        }
    }
}
