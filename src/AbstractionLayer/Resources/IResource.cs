namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Basic interface of a Resource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Id of the resource
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Name of this resource instance
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets information about this resource.
        /// </summary>
        string LocalIdentifier { get; }

        /// <summary>
        /// Global identifier of a resource
        /// </summary>
        string GlobalIdentifier { get; }

        /// <summary>
        /// Descritpion of a resource
        /// </summary>
        string Description { get; }
    }
}