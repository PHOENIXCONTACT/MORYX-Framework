// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Serialization;

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

#if HAVE_WCF
        [ReferenceOverride(nameof(Parent))]
        public InstructionServiceHost ServiceHost
        {
            get { return (InstructionServiceHost) Parent; }
            set { Parent = value; }
        }
#endif

        [ResourceReference(ResourceRelationType.Extension, ResourceReferenceRole.Source)]
        public IReferences<Resource> Users { get; set; }

        public VisualInstructor()
        {
            Descriptor = new InstructorDescriptor(this);
        }

        public void Show(string foo)
        {
#if HAVE_WCF
            ServiceHost.DisplayFoo(ClientId, foo);
#endif
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
}
