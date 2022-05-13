using System;
using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Resources
{
    public interface IResourceModificationExtended: IResourceModification
    {
        /// <summary>
        /// Get all resources inluding the private ones of this type that match the predicate
        /// </summary>
        IEnumerable<TResource> GetAllResources<TResource>(Func<TResource, bool> predicate)
            where TResource : class, IResource;
    }
}
