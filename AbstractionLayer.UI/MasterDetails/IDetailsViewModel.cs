namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Interface for details view models
    /// </summary>
    public interface IDetailsViewModel
    {
        /// <summary>
        /// Initializes this instance with all needed information
        /// </summary>
        void Initialize(IInteractionController controller, string typeName);
    }
}
