// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Configuration;

#if USE_WCF
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Communication.Endpoints;
using System.Net;
using Moryx.Web;
#endif

namespace Moryx.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Service contracts for database operations.
    /// </summary>
#if USE_WCF
    [ServiceContract]
    [Endpoint(Name = nameof(IDatabaseMaintenance), Version = "3.0.0.0")]
#endif
    internal interface IDatabaseMaintenance
    {
        /// <summary>
        /// Get all database configs
        /// </summary>
        /// <returns>The fetched DataModels.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "/", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        DataModel[] GetAll();

        /// <summary>
        /// Drop all data models
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "/", Method = WebRequestMethodsExtension.Http.Delete,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse EraseAll();

        /// <summary>
        /// Get all database config
        /// </summary>
        /// <returns>The fetched DataModel.</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        DataModel GetModel(string targetModel);

        /// <summary>
        /// Set database config
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/config", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        void SetDatabaseConfig(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Test a new config
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/config/test", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        TestConnectionResponse TestDatabaseConfig(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Create all datamodels with current config
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "createall", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse CreateAll();

        /// <summary>
        /// Create a new database matching the model
        /// </summary>
        /// <returns>True if database could be created</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/create", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse CreateDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Erases the database given by the model
        /// </summary>
        /// <returns>True if erased successful</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}", Method = WebRequestMethodsExtension.Http.Delete,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse EraseDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Dumps the database matching the model to create a restoreable backup
        /// </summary>
        /// <returns>True if async dump is in progress</returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/dump", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse DumpDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Restores the database.
        /// </summary>
        /// <returns></returns>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/restore", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse RestoreDatabase(string targetModel, RestoreDatabaseRequest request);

        /// <summary>
        /// Updates database model to the specified update.
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/{migrationName}/migrate", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        DatabaseUpdateSummary MigrateDatabaseModel(string targetModel, string migrationName, DatabaseConfigModel config);

        /// <summary>
        /// Rollback of all migrations made
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/rollback", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse RollbackDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Execute setup for this config
        /// </summary>
#if USE_WCF
        [OperationContract]
        [WebInvoke(UriTemplate = "model/{targetModel}/setup", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
#endif
        InvocationResponse ExecuteSetup(string targetModel, ExecuteSetupRequest request);
    }
}
