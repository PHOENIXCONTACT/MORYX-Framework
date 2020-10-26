// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Interaction
{
    /// <summary>
    /// Default interaction resource which hosts the default web service
    /// </summary>
    [Description("Resource to host the default web service")]
    [ResourceRegistration, DependencyRegistration(typeof(IResourceInteraction))]
    public sealed class ResourceInteractionHost : Resource
    {
        /// <summary>
        /// Factory to create the web service
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <summary>
        /// Host config injected by resource manager
        /// </summary>
        [DataMember, EntrySerialize]
        public HostConfig HostConfig { get; set; }

        /// <inheritdoc />
        public override object Descriptor => HostConfig;

        /// <summary>
        /// Current service host
        /// </summary>
        private IConfiguredServiceHost _host;

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

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            _host = HostFactory.CreateHost<IResourceInteraction>(HostConfig);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();

            _host.Start();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            _host.Stop();

            base.OnStop();
        }
    }
}
