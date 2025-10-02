using System;

namespace Moryx.AbstractionLayer.Resources
{
    public interface IResourceManagementChanges
    {
        /// <summary>
        /// Raised when a resource reports a change (via Resource.RaiseResourceChanged)
        /// or is changed via the ResourceManagement facade (e.g. Modify).
        /// </summary>
        event EventHandler<IResource> ResourceChanged;
    }
}
