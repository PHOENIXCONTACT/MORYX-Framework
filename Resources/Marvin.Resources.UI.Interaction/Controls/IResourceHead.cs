namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Interface representing the resource head information
    /// </summary>
    public interface IResourceHead
    {
        /// <summary>
        /// The name of the resource
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The global identifier of the resource
        /// </summary>
        string GlobalIdentifier { get; set; }

        /// <summary>
        /// The local identifier of the resource
        /// </summary>
        string LocalIdentifier { get; set; }
    }
}