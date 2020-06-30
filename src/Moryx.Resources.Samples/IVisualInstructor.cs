// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Resources.Interaction;
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

        [EditorBrowsable]
        protected class InstructorDescriptor
        {
            private readonly VisualInstructor _instructor;

            public InstructorDescriptor(VisualInstructor instructor)
            {
                _instructor = instructor;
            }

            [EditorBrowsable]
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
    public class InstructionServiceHost : InteractionResource<IInteractionService>
    {
        [ReferenceOverride(nameof(Children), AutoSave = true)]
        public IReferences<IVisualInstructor> Instructors { get; set; }

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

        public void DisplayFoo(string client, string message)
        {
        }
    }


    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.0.0", MinClientVersion = "1.0.0")]
    public interface IInteractionService : ISessionService
    {
    }

    [Plugin(LifeCycle.Transient, typeof(IInteractionService))]
    public class InteractionService : SessionService<InstructionServiceHost>, IInteractionService
    {

    }
}
