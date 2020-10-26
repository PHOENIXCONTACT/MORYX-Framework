// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Service contract for the common maintenance.
    /// </summary>
    [ServiceContract]
    [ServiceVersion("3.0.0.0")]
    public interface ICommonMaintenance
    {
        /// <summary>
        /// Get the server time.
        /// </summary>
        /// <returns>The current server time.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ServerTime", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ServerTimeResponse GetServerTime();

        /// <summary>
        /// Get information about the application
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ApplicationInfo", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ApplicationInformationResponse GetApplicationInfo();

        /// <summary>
        /// Get information about the host
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "HostInfo", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        HostInformationResponse GetHostInfo();

        /// <summary>
        /// Get information about the system load
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "SystemLoad", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        SystemLoadResponse GetSystemLoad();

        /// <summary>
        /// Get information about the application load
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "ApplicationLoad", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ApplicationLoadResponse GetApplicationLoad();
    }
}
