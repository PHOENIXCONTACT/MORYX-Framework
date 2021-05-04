using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Kestrel Http Host factory
    /// </summary>
    public interface IKestrelHttpHostFactory
    {
        /// <summary>
        /// Register a container to the factory
        /// </summary>
        /// <param name="container"></param>
        void RegisterContainer(IContainer container);

        /// <summary>
        /// Unregister a container to the factory
        /// </summary>
        /// <param name="container"></param>
        void UnregisterContainer(IContainer container);
    }
}
