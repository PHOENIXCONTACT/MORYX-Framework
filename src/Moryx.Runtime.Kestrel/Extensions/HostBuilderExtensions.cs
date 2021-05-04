using Microsoft.Extensions.Hosting;
using Moryx.Container;

namespace Moryx.Runtime.Kestrel
{
    internal static class HostBuilderExtensions
    {
        /// <summary>
        /// Uses <see name="IWindsorContainer" /> as the DI container for the host
        /// </summary>
        /// <param name="hostBuilder">Host builder</param>
        /// <param name="container">Windsor Container to be used for registrations, please note, will be cleared of all existing registrations</param>
        /// <returns>Host builder</returns>
        public static IHostBuilder UseWindsorContainerServiceProvider(this IHostBuilder hostBuilder, LocalContainerWithSubContainer container)
        {
            // Note: the container is a kind of reinitialized after calling this function
            // so call initialize the container
            hostBuilder.UseWindsorContainerServiceProvider(container.InternalContainer);
            container.Initialize();
            return hostBuilder;
        }
	}
}
