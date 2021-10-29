// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication.Endpoints;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Wcf
{
    /// <summary>
    /// Resources that host a WCF service to interact with related resources
    /// </summary>
    public abstract class InteractionResource : Resource, IServiceManager
    {
        /// <summary>
        /// Factory to create the web service
        /// </summary>
        public IEndpointHostFactory HostFactory { get; set; }

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
        protected IEndpointHost Host { get; private set; }

        /// <summary>
        /// Registered service instances
        /// </summary>
        protected ICollection<ISessionService> Clients { get; } = new SynchronizedCollection<ISessionService>();

        /// <summary>
        /// Contract type of the service
        /// </summary>
        protected abstract Type ServiceContract { get; }

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Host = HostFactory.CreateHost(ServiceContract, HostConfig);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();

            Host.Start();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            Host.Stop();

            base.OnStop();
        }

        /// <inheritdoc />
        void IServiceManager.Register(ISessionService service)
        {
            Clients.Add(service);
        }

        /// <inheritdoc />
        void IServiceManager.Unregister(ISessionService service)
        {
            Clients.Remove(service);
        }
    }
}
