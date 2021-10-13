// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Communication.Endpoints;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Threading;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Base implementation of a WCF service plugin
    /// </summary>
    /// <typeparam name="TConfig">The plugin configuration class</typeparam>
    /// <typeparam name="TSvcMgr">The service manager interface</typeparam>
    public abstract class BasicWcfConnectorPlugin<TConfig, TSvcMgr> : IWcfConnector<TConfig>
        where TConfig : IWcfServiceConfig
        where TSvcMgr : class
    {
        /// <summary>
        /// The plugin configuration
        /// </summary>
        protected TConfig Config { get; set; }

        /// <summary>
        /// The WCF service
        /// </summary>
        protected IEndpointHost Service { get; set; }

        #region Dependency Injection

        /// <summary>Injected property</summary>
        public IEndpointHostFactory HostFactory { get; set; }

        /// <summary>Injected property</summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>Injected property</summary>
        public IContainer ParentContainer { get; set; }

        /// <summary>Injected property</summary>
        public TSvcMgr ServiceManager { get; set; }

        #endregion

        /// <seealso cref="IConfiguredInitializable{T}.Initialize"/>
        public virtual void Initialize(TConfig config)
        {
            Config = config;
        }

        /// <inheritdoc />
        public abstract void Start();

        /// <inheritdoc />
        public virtual void Stop()
        {
            if (Service != null)
            {
                Service.Stop();
                Service = null;
            }
        }
    }
}
