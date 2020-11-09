// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Moryx.Runtime.Wcf
{
    internal class CorsPreflightState
    {
    }

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

        protected override object CreateBehavior() => new CorsBehaviorAttribute();

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            CorsPreflightState preflightState = null;

            if (request.Properties["httpRequest"] is HttpRequestMessageProperty httpRequest)
            {
                // Check if Origin ius set and method is OPTIONS then a cors preflight is requested
                var origin = httpRequest.Headers["Origin"];
                if (!string.IsNullOrEmpty(origin) && httpRequest.Method == "OPTIONS")
                {
                    preflightState = new CorsPreflightState();
                }
            }

            return preflightState;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (reply.Properties["httpResponse"] is HttpResponseMessageProperty httpResponse)
            {
                // Check if this is the response of a cors preflight
                if (correlationState is CorsPreflightState)
                {
                    httpResponse.StatusCode = HttpStatusCode.NoContent;
                    httpResponse.Headers["Access-Control-Allow-Origin"] = "*";
                    httpResponse.Headers["Access-Control-Request-Method"] = "POST,GET,PUT,DELETE,OPTIONS";
                    httpResponse.Headers["Access-Control-Allow-Headers"] = "X-Requested-With,Content-Type";
                }
            }
        }
    }
}
