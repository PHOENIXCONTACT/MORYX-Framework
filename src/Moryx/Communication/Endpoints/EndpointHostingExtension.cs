using Moryx.Container;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Extensions for the module container to activate hosting
    /// </summary>
    public static class ContainerExtension
    {
        /// <summary>
        /// Activate hosting and register the necessary components
        /// </summary>
        public static IContainer ActivateHosting(this IContainer container, IEndpointHosting hosting)
        {
            hosting.ActivateHosting(container);
            return container;
        }
    }
}