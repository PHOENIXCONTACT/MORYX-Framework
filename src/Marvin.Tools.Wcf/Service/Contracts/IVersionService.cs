// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;
using System.ServiceModel.Web;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Version service for handling of endpoints within an application
    /// </summary>
    [ServiceContract]
    public interface IVersionService
    {
        /// <summary>
        /// Checks if the client is supported. Will use the service name and the client version for check.
        /// </summary>
        /// <param name="service">The name of the service endpoint</param>
        /// <param name="clientVersion">The version of the client</param>
        /// <returns>True for support</returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        bool ClientSupported(string service, string clientVersion);

        /// <summary>
        /// Will return the version of the server
        /// </summary>
        /// <param name="endpoint">Endpoint of the service</param>
        /// <returns>The version string</returns>
        [OperationContract]
        [WebGet]
        string GetServerVersion(string endpoint);

        /// <summary>
        /// Will return a list of service endpoints hostet by the runtime
        /// </summary>
        [OperationContract]
        [WebGet]
        ServiceEndpoint[] ActiveEndpoints();

        /// <summary>
        /// Will return the configuration of the service endpoint
        /// </summary>
        [OperationContract]
        [WebGet]
        ServiceConfig GetServiceConfiguration(string service);
    }
}
