using System;
using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Facade for returning all resources and not only resources implementing IPublicResource
    /// </summary>
    // TODO: Move into IResourceManagement
    public interface IResourceModificationExtended: IResourceModification
    {
        /// <summary>
        /// Get all resources inluding the private ones of this type that match the predicate
        /// </summary>
        IEnumerable<TResource> GetAllResources<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;
    }
}
