// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication.Endpoints;
using Moryx.Modules;

#if USE_WCF
using Moryx.Tools.Wcf;
#endif

namespace Moryx.Runtime.Maintenance
{
    /// <summary>
    /// Base class for maintenance plugins.
    /// </summary>
    /// <typeparam name="TConf">Type of configuration.</typeparam>
    /// <typeparam name="TEndpoint">Type of Wcf service.</typeparam>
    public abstract class MaintenancePluginBase<TConf, TEndpoint> : IMaintenancePlugin where TConf : MaintenancePluginConfig
    {
        private IPlugin _host;

        /// <summary>
        /// Configuration of type TConf.
        /// </summary>
        protected TConf Config { get; set; }

        /// <summary>
        /// Factory to create endpoint services
        /// </summary>
#if USE_WCF
        public IConfiguredHostFactory HostFactory { get; set; }
#else
        public IEndpointHostFactory HostFactory { get; set; }
#endif

        /// <summary>
        /// Factory to create endpoint services
        /// </summary>
        public IEndpointHostFactory EndpointHostFactory { get; set; }

        /// <inheritdoc />
        public virtual void Initialize(MaintenancePluginConfig config)
        {
            Config = (TConf)config;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
#if USE_WCF
            _host = HostFactory.CreateHost(typeof(TEndpoint), Config.ProvidedEndpoint);
#else
            _host = EndpointHostFactory.CreateHost(typeof(TEndpoint), null);
#endif
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
