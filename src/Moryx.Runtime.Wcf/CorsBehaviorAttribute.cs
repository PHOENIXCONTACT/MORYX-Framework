// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Moryx.Runtime.Wcf
{
    /// <summary>
    /// With help from https://stackoverflow.com/a/60630766/6082960
    /// </summary>
    internal class CorsBehaviorAttribute : BehaviorExtensionElement, IEndpointBehavior, IDispatchMessageInspector
    {
        public override Type BehaviorType => typeof(CorsBehaviorAttribute);

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public void Validate(ServiceEndpoint endpoint) { }

        protected override object CreateBehavior() => this;

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply.Properties["httpResponse"] is HttpResponseMessageProperty httpResponse)
            {
                httpResponse.Headers["Access-Control-Allow-Origin"] = "*";
                httpResponse.Headers["Access-Control-Request-Method"] = "POST,GET,PUT,DELETE,OPTIONS";
                httpResponse.Headers["Access-Control-Allow-Headers"] = "X-Requested-With,Content-Type";
            }
        }
    }
}
