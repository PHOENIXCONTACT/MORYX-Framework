namespace Marvin.Model
{
    /// <summary>
    /// Interface for all <see cref="IUnitOfWorkFactory"/> to give inherited factories
    /// the chance to register themselves
    /// </summary>
    public interface IParentFactory
    {
        /// <summary>
        /// Register a new child for this parent
        /// </summary>
        void RegisterChild(IUnitOfWorkFactory childFactory, string childModel);
    }
}
