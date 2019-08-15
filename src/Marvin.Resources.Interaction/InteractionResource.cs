using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.Interaction
{
    /// <summary>
    /// Resources that host a WCF service to interact with related resources
    /// </summary>
    public abstract class InteractionResource<TService> : Resource, IServiceManager
    {
        /// <summary>
        /// Factory to create the web service
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <summary>
        /// Host config injected by resource manager
        /// </summary>
        [DataMember, EditorVisible]
        public HostConfig HostConfig { get; set; }

        /// <inheritdoc />
        public override object Descriptor => HostConfig;

        /// <summary>
        /// Current service host
        /// </summary>
        protected IConfiguredServiceHost Host { get; private set; }

        /// <summary>
        /// Registered service instances
        /// </summary>
        protected ICollection<TService> Clients { get; } = new SynchronizedCollection<TService>();

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            Host = HostFactory.CreateHost<TService>(HostConfig);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();

            Host.Start();
        }

        /// <inheritdoc />
        protected override void OnDispose()
        {
            Host.Stop();

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