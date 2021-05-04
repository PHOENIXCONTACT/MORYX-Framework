using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Communication;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Kestrel Http host factory to start up a kestrel server
    /// </summary>
    [InitializableKernelComponent(typeof(IKestrelHttpHostFactory))]
    internal class KestrelHttpHostFactory : IKestrelHttpHostFactory, IInitializable, ILoggingHost
    {
        #region Dependencies
        
        /// <summary>
        /// Config manager
        /// </summary>
        public IConfigManager ConfigManager { get; set; }

        /// <summary>
        /// Logger management
        /// </summary>
        public ILoggerManagement LoggerManagement { get; set; }

        #endregion

        #region Fields and Properties

        string ILoggingHost.Name => nameof(KestrelHttpHostFactory);

        /// <summary>
        /// Logger
        /// </summary>
        public IModuleLogger Logger { get; set; }

        private LocalContainerWithSubContainer _container;
        private PortConfig _portConfig;

        #endregion

        /// <inheritdoc />
        public void Initialize()
        {
            LoggerManagement.ActivateLogging(this);

            _portConfig = ConfigManager.GetConfiguration<PortConfig>();

            _container = new LocalContainerWithSubContainer();

            var hostBuilder = Host.CreateDefaultBuilder()
                .UseWindsorContainerServiceProvider(_container)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(new IPAddress(0), _portConfig.HttpPort);
                    }).ConfigureLogging(builder =>
                    {
                        builder.ClearProviders();
                    }).UseStartup(context => new Startup(context.Configuration, ConfigManager));
                });

            _container.SetInstance(ConfigManager);

            hostBuilder.Build().RunAsync();
        }

        /// <inheritdoc />
        public void RegisterContainer(IContainer container)
        {
            _container.RegisterContainer(container);
        }

        /// <inheritdoc />
        public void UnregisterContainer(IContainer container)
        {
            _container.UnregisterContainer(container);
        }
    }
}
