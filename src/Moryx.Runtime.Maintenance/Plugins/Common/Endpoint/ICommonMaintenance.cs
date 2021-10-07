// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#if USE_WCF
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Common
{
    /// <summary>
    /// Service contract for the common maintenance.
    /// </summary>
#if USE_WCF
    [ServiceContract]
    [Endpoint(Name = nameof(ICommonMaintenance), Version = "3.0.0.0")]
#endif
    internal interface ICommonMaintenance
    {
        /// <summary>
        /// Get the server time.
        /// </summary>
        /// <returns>The current server time.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "time", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ServerTimeResponse GetServerTime();

        /// <summary>
        /// Get information about the application
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "info/application", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ApplicationInformationResponse GetApplicationInfo();

        /// <summary>
        /// Get information about the host
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "info/system", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        HostInformationResponse GetHostInfo();

        /// <summary>
        /// Get information about the system load
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "info/system/load", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        SystemLoadResponse GetSystemLoad();

        /// <summary>
        /// Get information about the application load
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "info/application/load", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ApplicationLoadResponse GetApplicationLoad();
    }
}
