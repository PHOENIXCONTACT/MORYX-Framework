// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Base service contract for the maintenance module.
    /// </summary>
    [ServiceContract]
    [ServiceVersion("3.0.0")]
    public interface IModuleMaintenance
    {
        /// <summary>
        /// Gets the dependency evaluation.
        /// </summary>
        /// <returns>The dependency evaluation.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "dependencyEvaluation", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        DependencyEvaluation GetDependencyEvaluation();

        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ServerModuleModel[] GetAll();

        /// <summary>
        /// Get health state
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/healthstate", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ServerModuleState HealthState(string moduleName);

        /// <summary>
        /// Get health state
        /// </summary>
        /// <param name="moduleName">Name of the module.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/notifications", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        NotificationModel[] Notifications(string moduleName);

        /// <summary>
        /// Try to start the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be started.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/start", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void Start(string moduleName);

        /// <summary>
        /// Try to stop the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be stopped.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/stop", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void Stop(string moduleName);

        /// <summary>
        /// Try to reincarnate the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module which should be reincarnated.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/reincarnate", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void Reincarnate(string moduleName);

        /// <summary>
        /// Update the modules failure and startbehavior
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void Update(string moduleName, ServerModuleModel module);

        /// <summary>
        /// Confirms the warning of the module with the moduleName.
        /// </summary>
        /// <param name="moduleName">Name of the module where the warning will confirmed.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/confirm", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void ConfirmWarning(string moduleName);

        /// <summary>
        /// Get the config for the module from the moduleName.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        /// <returns>Configuration of the requested module.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/config", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Config GetConfig(string moduleName);

        /// <summary>
        /// Set the given config and react to updated mode.
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/config", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void SetConfig(string moduleName, SaveConfigRequest request);

        /// <summary>
        /// Get all server modules.
        /// </summary>
        /// <returns>A list of the server modules.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/console", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        MethodEntry[] GetMethods(string moduleName);

        /// <summary>
        /// Invokes a method
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "modules/{moduleName}/console", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Entry InvokeMethod(string moduleName, MethodEntry method);
    }
}
