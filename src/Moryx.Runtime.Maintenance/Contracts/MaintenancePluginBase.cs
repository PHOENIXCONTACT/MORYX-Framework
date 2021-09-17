// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication.Endpoints;
using Moryx.Tools.Wcf;

namespace Moryx.Runtime.Maintenance
{
    /// <summary>
    /// Base class for maintenance plugins.
    /// </summary>
    /// <typeparam name="TConf">Type of configuration.</typeparam>
    /// <typeparam name="TWcf">Type of Wcf service.</typeparam>
    public abstract class MaintenancePluginBase<TConf, TWcf> : IMaintenancePlugin where TConf : MaintenancePluginConfig
    {
        private IEndpointHost _host;

        /// <summary>
        /// Configuration of type TConf.
        /// </summary>
        protected TConf Config { get; set; }

        /// <summary>
        /// Factory to create WCF services
        /// </summary>
        public IEndpointHostFactory HostFactory { get; set; }

        /// <inheritdoc />
        public virtual void Initialize(MaintenancePluginConfig config)
        {
            Config = (TConf)config;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
            _host = HostFactory.CreateHost(typeof(TWcf), Config.ProvidedEndpoint);
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
