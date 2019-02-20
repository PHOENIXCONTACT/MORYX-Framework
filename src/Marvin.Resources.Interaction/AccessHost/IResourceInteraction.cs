using System;
using System.ServiceModel;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Interaction
{
    /// <summary>
    /// Interface to provide functions for interaction of resources. 
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "2.0.0", MinClientVersion = "2.0.0")]
    public interface IResourceInteraction
    {
        /// <summary>
        /// Full type tree of currently installed resources
        /// </summary>
        [OperationContract]
        ResourceTypeModel[] GetTypeTree();

        /// <summary>
        /// Gets the complete resource tree.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ResourceModel[] GetResourceTree();

        /// <summary>
        /// Get the details of the given resource.
        /// </summary>
        /// <param name="id">Id of the resource</param>
        /// <param name="depth">Loading depth</param>
        /// <returns>A model with all deails loaded.</returns>
        [OperationContract]
        ResourceModel GetDetails(long id, int depth = 1);

        /// <summary>
        /// Invoke method on the resource
        /// </summary>
        [OperationContract]
        Entry InvokeMethod(long id, MethodEntry method);

        /// <summary>
        /// Creates an active resource from the given plugin name. Name should be existend to create configs for the resource.
        /// </summary>
        /// <param name="resourceType">Resource type to create instance of</param>
        /// <param name="constructor">Optional constructor method</param>
        /// <returns>A new created resource model.</returns>
        [OperationContract]
        ResourceModel Create(string resourceType, MethodEntry constructor = null);

        /// <summary>
        /// Save resource in the database. 
        /// </summary>
        /// <param name="resource">The resource which shoule be saved.</param>
        /// <returns>The saved resource with the database id.</returns>
        [OperationContract]
        ResourceModel Save(ResourceModel resource);

        /// <summary>
        /// Removes a resource from the database.
        /// </summary>
        /// <param name="id">The resource which should be removed.</param>
        /// <returns>true when removing was successful.</returns>
        [OperationContract]
        bool Remove(long id);
    }
}