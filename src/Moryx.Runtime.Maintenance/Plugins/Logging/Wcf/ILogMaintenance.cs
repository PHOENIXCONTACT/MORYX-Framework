// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Communication.Endpoints;
using Moryx.Web;

namespace Moryx.Runtime.Maintenance.Plugins.Logging
{
    /// <summary>
    /// Service contract for logging features of the maintenance.
    /// </summary>
    [ServiceContract]
    [Endpoint(Name = nameof(ILogMaintenance), Version = "3.0.0.0")]
    internal interface ILogMaintenance
    {
        /// <summary>
        /// Get all plugin logger.
        /// </summary>
        /// <returns>Array of plugin logger.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        LoggerModel[] GetAllLoggers();

        /// <summary>
        /// Set the log level of aa logger.
        /// </summary>
        /// <param name="loggerName">Name of the logger</param>
        /// <param name="setLogLevelRequest"></param>
        [OperationContract]
        [WebInvoke(UriTemplate = "logger/{loggerName}/loglevel", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        InvocationResponse SetLogLevel(string loggerName, SetLogLevelRequest setLogLevelRequest);

        /// <summary>
        /// Add a remote appender to the logging stream.
        /// </summary>
        /// <returns>Id to identify the appender.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "appender", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        AddAppenderResponse AddAppender(AddAppenderRequest request);

        /// <summary>
        /// Removes a remote appender from the loggin stream.
        /// </summary>
        /// <param name="appenderId">The id of the appender.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "appender/{appenderId}", Method = WebRequestMethodsExtension.Http.Delete,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        InvocationResponse RemoveAppender(string appenderId);

        /// <summary>
        /// Get the messages of the appender.
        /// </summary>
        /// <param name="appenderId">The id of the appender.</param>
        /// <returns>Log messages for the appender.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "appender/{appenderId}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        LogMessageModel[] GetMessages(string appenderId);
    }
}
