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
    [ServiceVersion("2.0.0")]
    public interface IResourceInteraction
    {

        /// <summary>
        /// Full type tree of currently installed resources
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "test", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        string Test();

        /// <summary>
        /// Full type tree of currently installed resources
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "resources/typetree", Method = WebRequestMethods.Http.Get,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceTypeModel GetTypeTree();

        /// <summary>
        /// Load resources by query
        /// <param name="query"></param>
        /// <returns></returns>
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/query", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel[] GetResources(ResourceQuery query);

        /// <summary>
        /// Get the details of the given resource.
        /// </summary>
        /// <param name="ids">Ids of the resources</param>
        /// <returns>A model with all details loaded.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/details", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel[] GetDetails(long[] ids);

        /// <summary>
        /// Invoke method on the resource
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/invoke", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        Entry InvokeMethod(InvokeMethod invokeMethod);

        /// <summary>
        /// Creates an active resource from the given plugin name. Name should be existent to create configs for the resource.
        /// </summary>
        /// <param name="resourceType">Resource type to create instance of</param>
        /// <param name="constructor">Optional constructor method</param>
        /// <returns>A new created resource model.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/create", Method = WebRequestMethods.Http.Put,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel Create(CreateResource createResource);

        /// <summary>
        /// Save resource in the database.
        /// </summary>
        /// <param name="resource">The resource which should be saved.</param>
        /// <returns>The saved resource with the database id.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/save", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        ResourceModel Save(ResourceModel resource);

        /// <summary>
        /// Removes a resource from the database.
        /// </summary>
        /// <param name="id">The resource which should be removed.</param>
        /// <returns>true when removing was successful.</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "resource/remove", Method = WebRequestMethods.Http.Post,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        bool Remove(long id);
    }
}
