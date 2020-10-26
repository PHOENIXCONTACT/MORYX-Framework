// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Version service for handling of endpoints within an application
    /// </summary>
    [ServiceContract]
    public interface IVersionService
    {
        /// <summary>
        /// Will return a list of service endpoints hostet by the runtime
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "/", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Endpoint[] AllEndpoints();

        /// <summary>
        /// Will return a filtered list of endpoints by interface
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "service/{service}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Endpoint[] FilteredEndpoints(string service);

        /// <summary>
        /// Will return the configuration of the service endpoint
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "endpoint/{endpoint}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Endpoint GetEndpointConfig(string endpoint);
    }
}
