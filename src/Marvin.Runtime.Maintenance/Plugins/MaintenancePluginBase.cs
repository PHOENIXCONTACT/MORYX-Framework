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
        /// Factory to create WCF services
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <inheritdoc />
        public virtual void Initialize(MaintenancePluginConfig config)
        {
            Config = (TConf)config;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
            _host = HostFactory.CreateHost<TWcf>(Config.ProvidedEndpoint);
            _host.Start();
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
            _host?.Stop();
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            _host = null;
        }
    }
}