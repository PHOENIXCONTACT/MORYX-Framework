using System;
using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Public api for new resource detail view models. 
    /// </summary>
    public interface IResourceDetails : IEditModeViewModel, IDetailsViewModel
    {
        /// <summary>
        /// Gets the current resource id.
        /// </summary>
        long CurrentResourceId { get; }

        /// <summary>
        /// Method to load the needed resource details from the server
        /// </summary>
        Task Load(long resourceId);

        /// <summary>
        /// Creates a new resource by the defined resource type
        /// </summary>
        Task Create(string resourceTypeName, long parentResourceId, object constructor); // TODO: Fix this!!!
    }
}