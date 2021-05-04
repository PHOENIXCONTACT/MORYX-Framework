using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Container extensions
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Register local module container to kestrel
        /// </summary>
        public static IContainer RegisterToKestrel(this IContainer container,
            IKestrelHttpHostFactory kestrelHttpHostFactory)
        {
            kestrelHttpHostFactory.RegisterContainer(container);
            return container;
        }

        /// <summary>
        /// Unregister local module container to kestrel
        /// </summary>
        public static IContainer UnregisterFromKestrel(this IContainer container,
            IKestrelHttpHostFactory kestrelHttpHostFactory)
        {
            kestrelHttpHostFactory.UnregisterContainer(container);
            return container;
        }
    }
}
