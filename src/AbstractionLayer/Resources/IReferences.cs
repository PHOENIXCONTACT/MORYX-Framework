using System.Collections.Generic;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Interface for reference collections. This is necessary to keep Castle from interfering
    /// </summary>
    public interface IReferences<TResource> : ICollection<TResource>
        where TResource : IResource
    {
    }
}