using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Model;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Management
{
    /// <summary>
    /// Resources that host a WCF service to interact with related resources
    /// </summary>
    public abstract class InteractionResource<TService> : Resource, IServiceManager
    {
        #region Dependency Injection

        /// <summary>
        /// Injected by caslte.
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        #endregion

        #region Config

        /// <summary>
        /// Host config injected by resource manager
        /// </summary>
        [DataMember, EditorVisible]
        public HostConfig HostConfig { get; set; }

        /// <inheritdoc />
        public override object Descriptor => HostConfig;

        #endregion

        /// <summary>
        /// Current service host
        /// </summary>
        protected IConfiguredServiceHost Host { get; private set; }

        /// <summary>
        /// Registered service instances
        /// </summary>
        protected List<TService> Clients { get; } = new List<TService>();

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Host = HostFactory.CreateHost<TService>(HostConfig);
        }

        /// 
        protected override void OnStart()
        {
            base.OnStart();

            Host.Start();
        }

        /// 
        protected override void OnDispose()
        {
            Host.Dispose();

            base.OnDispose();
        }

        /// <inheritdoc />
        void IServiceManager.Register(ISessionService service)
        {
            Clients.Add((TService)service);
        }

        /// <inheritdoc />
        void IServiceManager.Unregister(ISessionService service)
        {
            Clients.Remove((TService) service);
        }
    }
}