namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Component to create new, tracked resource instances
    /// </summary>
    public interface IResourceCreator
    {
        /// <summary>
        /// Create a new resource instance but DO NOT save it
        /// </summary>
        Resource Instantiate(string type);

        /// <summary>
        /// Instantiate a typed resource
        /// </summary>
        TResource Instantiate<TResource>()
            where TResource : Resource;

        /// <summary>
        /// Instantiate a typed resource
        /// </summary>
        TResource Instantiate<TResource>(string type)
            where TResource : class, IResource;

        /// <summary>
        /// Remove resource, but only flag it deleted
        /// </summary>
        bool Destroy(IResource resource);

        /// <summary>
        /// Remove a resource permanently and irreversible
        /// </summary>
        bool Destroy(IResource resource, bool permanent);
    }
}