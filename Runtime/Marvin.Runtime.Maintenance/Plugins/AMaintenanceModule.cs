using Marvin.Runtime.Maintenance.Contracts;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins
{
    /// <summary>
    /// Base class for maintenace plugins.
    /// </summary>
    /// <typeparam name="TConf">Type of configuration.</typeparam>
    /// <typeparam name="TWcf">Type of Wcf service.</typeparam>
    public abstract class MaintenancePluginBase<TConf, TWcf> : IMaintenancePlugin where TConf : MaintenancePluginConfig
    {
        private IConfiguredServiceHost _host;

        /// <summary>
        /// Configuration of type TConf.
        /// </summary>
        protected TConf Config { get; set; }

        /// <summary>
        ///  Injected properties
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <summary>
        /// Initialize the plugin with the give configuration.
        /// </summary>
        /// <param name="config">A <see cref="MaintenancePluginConfig"/>.</param>
        public virtual void Initialize(MaintenancePluginConfig config)
        {
            Config = (TConf)config;
        }

        /// <summary>
        /// Start the plugin.
        /// </summary>
        public virtual void Start()
        {
            _host = HostFactory.CreateHost<TWcf>(Config.ProvidedEndpoint);
            _host.Start();
        }

        /// <summary>
        /// Disposes the plugin.
        /// </summary>
        public virtual void Dispose()
        {
            if (_host == null)
                return;

            _host.Dispose();
            _host = null;
        }
    }
}