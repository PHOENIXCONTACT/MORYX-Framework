// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Interaction
{
    /// <summary>
    /// Interface to provide functions for interaction of resources.
    /// </summary>
    [ServiceContract]
    [ServiceVersion("5.0.0")]
    internal interface IResourceInteraction
    {
        /// <summary>
        /// Full type tree of currently installed resources
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "types", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceTypeModel GetTypeTree();

        /// <summary>
        /// Load resources by query
        /// <param name="query"></param>
        /// <returns></returns>
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "query", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel[] GetResources(ResourceQuery query);

        /// <summary>
        /// Get the details of the given resource.
        /// </summary>
        /// <param name="id">Ids of the resources</param>
        /// <returns>A model with all details loaded.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/{id}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel GetDetails(string id);

        /// <summary>
        /// Get the details of the given resource.
        /// </summary>
        /// <param name="ids">Ids of the resources</param>
        /// <returns>A model with all details loaded.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "batch/{ids}", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel[] GetDetailsBatch(string ids);

        /// <summary>
        /// Invoke method on the resource
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/{id}/invoke/{method}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Entry InvokeMethod(string id, string method, Entry parameters);

        /// <summary>
        /// Creates an active resource from the given plugin name. Name should be existent to create configs for the resource.
        /// </summary>
        /// <param name="type">Resource type to create instance of</param>
        /// <returns>A new created resource model.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "construct/{type}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel Construct(string type);

        /// <summary>
        /// Creates an active resource from the given plugin name. Name should be existent to create configs for the resource.
        /// </summary>
        /// <param name="type">Resource type to create instance of</param>
        /// <param name="method">Method to invoke</param>
        /// <param name="parameters">Optional constructor parameters</param>
        /// <returns>A new created resource model.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "construct/{type}?method={method}", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel ConstructWithParameters(string type, string method, Entry parameters);

        /// <summary>
        /// Save resource in the database.
        /// </summary>
        /// <param name="resource">The resource which should be saved.</param>
        /// <returns>The saved resource with the database id.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel Save(ResourceModel resource);

        /// <summary>
        /// Save resource in the database.
        /// </summary>
        /// <param name="id">Id of the resource to update</param>
        /// <param name="model">The resource which should be saved.</param>
        /// <returns>The saved resource with the database id.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/{id}", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel Update(string id, ResourceModel model);

        /// <summary>
        /// Removes a resource from the database.
        /// </summary>
        /// <param name="id">The resource which should be removed.</param>
        /// <returns>true when removing was successful.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/{id}", Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        void Remove(string id);
    }
}
