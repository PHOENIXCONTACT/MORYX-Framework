#if HAVE_WCF
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Communication.Endpoints;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Samples
{
    [DependencyRegistration(typeof(IInteractionService))]
    [ResourceRegistration(typeof(InstructionServiceHost))]
    public class InstructionServiceHost : Resource, IServiceManager
    {
        /// <summary>
        /// Factory to create the web service
        /// </summary>
        public IConfiguredHostFactory HostFactory { get; set; }

        /// <summary>
        /// Host config injected by resource manager
        /// </summary>
        [DataMember, EntrySerialize]
        public HostConfig HostConfig { get; set; }

        /// <inheritdoc />
        public override object Descriptor => HostConfig;

        [ReferenceOverride(nameof(Children), AutoSave = true)]
        public IReferences<IVisualInstructor> Instructors { get; set; }

        [ResourceConstructor(IsDefault = true)]
        public void DefaultConstructor()
        {
            HostConfig = new HostConfig
            {
                Endpoint = "AssemblyInstructions",
                BindingType = ServiceBindingType.NetTcp,
                MetadataEnabled = true,
            };
        }

        [ResourceConstructor, DisplayName("Create Clients")]
        public void CreateHost([Description("Number of clients")]int clientCount,
            [ResourceTypes(typeof(VisualInstructor))]string instructorType)
        {
            DefaultConstructor();

            for (int clientNumber = 1; clientNumber <= clientCount; clientNumber++)
            {
                var instructor = Graph.Instantiate<VisualInstructor>(instructorType);
                instructor.Name = instructor.ClientId = $"Client{clientNumber}";
                Instructors.Add(instructor);
            }
        }

        /// <summary>
        /// Current service host
        /// </summary>
        private IConfiguredServiceHost _host;

        /// <summary>
        /// Registered service instances
        /// </summary>
        private ICollection<IInteractionService> _clients = new SynchronizedCollection<IInteractionService>();

        /// <inheritdoc />
        protected override void OnInitialize()
        {
            base.OnInitialize();

            _host = HostFactory.CreateHost<IInteractionService>(HostConfig);
        }

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();

            _host.Start();
        }

        /// <inheritdoc />
        protected override void OnDispose()
        {
            _host.Stop();

            base.OnDispose();
        }

        /// <inheritdoc />
        void IServiceManager.Register(ISessionService service)
        {
            _clients.Add((IInteractionService)service);
        }

        /// <inheritdoc />
        void IServiceManager.Unregister(ISessionService service)
        {
            _clients.Remove((IInteractionService)service);
        }

        public void DisplayFoo(string client, string message)
        {
        }
    }


    [ServiceContract]
    [Endpoint(Name = nameof(IInteractionService), Version = "1.0.0")]
    internal interface IInteractionService : ISessionService
    {
    }

    [Plugin(LifeCycle.Transient, typeof(IInteractionService))]
    internal class InteractionService : SessionService<InstructionServiceHost>, IInteractionService
    {

    }
}
#endif