namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Extensions for resources
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// Returns the string representation of the resource type
        /// </summary>
        /// <param name="resource">Source resource</param>
        /// <returns>Type identifier as string</returns>
        public static string ResourceType(this IResource resource)
        {
            return resource.GetType().FullName;
        }
    }
}