using Caliburn.Micro;

namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Interface for detail view models
    /// </summary>
    public interface IDetailsFactory<out T> where T : class 
    {
        /// <summary>
        /// Creates a default products detail view model
        /// </summary>
        T Create();

        /// <summary>
        /// Creates a details view model for the given type name
        /// </summary>
        T Create(string typeName);

        /// <summary>
        /// Destroys the given <see cref="IScreen"/> instance
        /// </summary>
        void Destroy(IScreen screen);
    }
}