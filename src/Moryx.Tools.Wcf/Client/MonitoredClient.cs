// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Moryx.Communication.Endpoints;

namespace Moryx.Tools.Wcf
{
    internal delegate ClientCredentials GetClientCredentials(ICommunicationObject client);
    internal delegate IClientChannel GetInnerChannel(ICommunicationObject client);
    internal delegate void AddEndpointBehavior(ICommunicationObject client, IEndpointBehavior behavior);

    internal class MonitoredClient
    {
        public ClientConfig Config { get; set; }

        public Endpoint Endpoint { get; set; }

        public Binding Binding { get; set; }

        public Type ClientType { get; set; }

        public string ServiceName { get; set; }

        public Action<ConnectionState, ICommunicationObject> ConnectionCallback { get; set; }

        public GetClientCredentials GetClientCredentials { get; set; }

        public GetInnerChannel GetInnerChannel { get; set; }

        public AddEndpointBehavior AddEndpointBehavior { get; set; }

        public object CallbackService { get; set; }

        public InstanceContext CallbackContext { get; set; }

        public IClientChannel InnerChannel { get; set; }

        public ICommunicationObject Instance { get; set; }

        public InternalConnectionState State { get; set; }

        public WcfClientInfo ClientInfo { get; set; }

        public bool Destroy { get; set; }
    }
}
