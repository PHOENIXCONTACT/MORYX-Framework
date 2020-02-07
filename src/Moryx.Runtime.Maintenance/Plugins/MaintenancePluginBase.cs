// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Maintenance.Contracts;

namespace Moryx.Runtime.Maintenance.Plugins
{
    /// <summary>
    /// Base class for maintenance plugins.
    /// </summary>
    /// <typeparam name="TConf">Type of configuration.</typeparam>
    public abstract class MaintenancePluginBase<TConf> : IMaintenancePlugin where TConf : MaintenancePluginConfig
    {
        /// <summary>
        /// Configuration of type TConf.
        /// </summary>
        protected TConf Config { get; set; }

        /// <inheritdoc />
        public virtual void Initialize(MaintenancePluginConfig config)
        {
            Config = (TConf)config;
        }

        /// <inheritdoc />
        public virtual void Start()
        {
        }

        /// <inheritdoc />
        public virtual void Stop()
        {
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }
    }
}
