using System;
using Marvin.Container;
using Marvin.Modules;
using Marvin.Threading;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Base implementation of a WCF service plugin
    /// </summary>
    /// <typeparam name="TConfig">The plugin configuration class</typeparam>
    /// <typeparam name="TSvcMgr">The service manager interface</typeparam>
    public abstract class BasicWcfConnectorPlugin<TConfig, TSvcMgr> : IWcfConnector<TConfig>, IDisposable
        where TConfig : IWcfServiceConfig
        where TSvcMgr : class
    {
        /// <summary>
        /// The plugin's configuration
        /// </summary>
        protected TConfig Config { get; set; }

        /// <summary>
        /// The WCF service
        /// </summary>
        protected IConfiguredServiceHost Service { get; set; }

        #region Dependency Injection

        /// <summary>Injected property</summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <summary>Injected property</summary>
        public IParallelOperations ParallelOperations { get; set; }

        /// <summary>Injected property</summary>
        public IContainer ParentContainer { get; set; }

        /// <summary>Injected property</summary>
        public TSvcMgr ServiceManager { get; set; }

        #endregion

        /// <seealso cref="IConfiguredPlugin{T}.Initialize"/>
        public virtual void Initialize(TConfig config)
        {
            Config = config;
        }

        /// <inheritdoc />
        public abstract void Start();

        /// <inheritdoc />
        public abstract void Stop();

        /// <inheritdoc />
        public virtual void Dispose()
        {
            if (Service != null)
            {
                Service.Dispose();
                Service = null;
            }
        }
    }
}