// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools.Wcf;

namespace Moryx.Resources.Samples
{
    public interface IVisualInstructor : IResource
    {
        void Show(string foo);
    }

    [ResourceRegistration]
    public class VisualInstructor : Resource, IVisualInstructor
    {
        [DataMember]
        public string ClientId { get; set; }

        [ReferenceOverride(nameof(Parent))]
        public InstructionServiceHost ServiceHost
        {
            get { return (InstructionServiceHost) Parent; }
            set { Parent = value; }
        }

        public VisualInstructor()
        {
            Descriptor = new InstructorDescriptor(this);
        }

        public void Show(string foo)
        {
            ServiceHost.DisplayFoo(ClientId, foo);
        }

        public override object Descriptor { get; }

        [EntrySerialize]
        protected class InstructorDescriptor
        {
            private readonly VisualInstructor _instructor;

            public InstructorDescriptor(VisualInstructor instructor)
            {
                _instructor = instructor;
            }

            [EntrySerialize]
            public string ClientId
            {
                get { return _instructor.ClientId; }
                set { _instructor.ClientId = value; }
            }

            public void Show(string foo)
            {
                _instructor.Show(foo);
            }

            [DisplayName("Clear Instructions")]
            public void Clear()
            {
                _instructor.Show(string.Empty);
            }
        }
    }

    [ResourceRegistration]
    public class AwesomeInstructor : VisualInstructor
    {

    }

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
    [ServiceVersion("1.0.0")]
    internal interface IInteractionService : ISessionService
    {
    }

    [Plugin(LifeCycle.Transient, typeof(IInteractionService))]
    internal class InteractionService : SessionService<InstructionServiceHost>, IInteractionService
    {

    }
}
