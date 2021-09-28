// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;
using Moryx.Serialization;

#if USE_WCF
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Communication.Endpoints;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Base service contract for the maintenance module.
    /// </summary>
#if USE_WCF
    [ServiceContract]
    [Endpoint(Name = nameof(IModuleMaintenance), Version = "3.0.0.0")]
#endif
    internal interface IModuleMaintenance
    {
        /// <summary>
        /// Gets the dependency evaluation.
        /// </summary>
        /// <returns>The dependency evaluation.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "dependencies", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        DependencyEvaluation GetDependencyEvaluation();

        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "/", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ServerModuleModel[] GetAll();

        /// <summary>
        /// Get health state
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/healthstate", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        ServerModuleState HealthState(string moduleName);

        /// <summary>
        /// Get health state
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/notifications", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        NotificationModel[] Notifications(string moduleName);

        /// <summary>
        /// Try to start the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be started.</param>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/start", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void Start(string moduleName);

        /// <summary>
        /// Try to stop the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be stopped.</param>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/stop", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void Stop(string moduleName);

        /// <summary>
        /// Try to reincarnate the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be reincarnated.</param>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/reincarnate", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void Reincarnate(string moduleName);

        /// <summary>
        /// Update the modules failure and StartBehavior
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void Update(string moduleName, ServerModuleModel module);

        /// <summary>
        /// Confirms the warning of the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module where the warning will confirmed.</param>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/confirm", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void ConfirmWarning(string moduleName);

        /// <summary>
        /// Get the config for the module from the moduleName.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <returns>Configuration of the requested module.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/config", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        Config GetConfig(string moduleName);

        /// <summary>
        /// Set the given config and react to updated mode.
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/config", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void SetConfig(string moduleName, SaveConfigRequest request);

        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/console", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        MethodEntry[] GetMethods(string moduleName);

        /// <summary>
        /// Invokes a method
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "module/{moduleName}/console", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        Entry InvokeMethod(string moduleName, MethodEntry method);
    }
}
