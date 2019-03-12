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
    }
}