// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests
{
    public interface IReferenceResource : IResource
    {
        ISimpleResource Reference { get; set; }

        DerivedResource Reference2 { get; set; }

        IEnumerable<ISimpleResource> MoreReferences { get; }

        IEnumerable<ISimpleResource> EvenMoreReferences { get; set; }

        INonPublicResource NonPublic { get; }

        ISimpleResource GetReference();

        IReadOnlyList<ISimpleResource> GetReferences();

        void SetReference(ISimpleResource reference);

        void SetMany(IReadOnlyList<ISimpleResource> references);

        event EventHandler<ISimpleResource> ReferenceChanged;

        event EventHandler<ISimpleResource[]> SomeChanged;
    }

    public class RequiredReferenceResource : Resource
    {
        [ResourceReference(ResourceRelationType.Extension)]
        public SimpleResource NotRequired { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart, IsRequired = true)]
        public SimpleResource Reference { get; set; }

        [ResourceReference(ResourceRelationType.TransportSystem)]
        public IReferences<SimpleResource> NotRequiredReferences { get; set; }

        [ResourceReference(ResourceRelationType.PossibleExchangablePart, IsRequired = true)]
        public IReferences<SimpleResource> References { get; set; }
    }

    public class ReferenceResource : Resource, IReferenceResource
    {
        private ISimpleResource _reference;

        [ResourceReference(ResourceRelationType.CurrentExchangablePart, nameof(Reference))]
        public ISimpleResource Reference
        {
            get { return _reference; }
            set
            {
                _reference = value;
                ReferenceChanged?.Invoke(this, value);
                SomeChanged?.Invoke(this, new[] { value });
            }
        }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart)]
        public DerivedResource Reference2 { get; set; }

        [ResourceReference(ResourceRelationType.Extension, ResourceReferenceRole.Target, nameof(TargetReference))]
        public BidirectionalReferenceResource TargetReference { get; set; }

        [ResourceReference(ResourceRelationType.Extension, ResourceReferenceRole.Target, nameof(NewTargetReference))]
        public BidirectionalReferenceResource NewTargetReference { get; set; }

        [ResourceReference(ResourceRelationType.PossibleExchangablePart)]
        public IReferences<ISimpleResource> References { get; set; }

        [ReferenceOverride(nameof(Children), AutoSave = true)]
        internal IReferences<ISimpleResource> ChildReferences { get; set; }

        IEnumerable<ISimpleResource> IReferenceResource.MoreReferences => References;

        public IEnumerable<ISimpleResource> EvenMoreReferences { get; set; }

        public ISimpleResource GetReference()
        {
            return Reference;
        }

        public IReadOnlyList<ISimpleResource> GetReferences()
        {
            return References.ToArray();
        }

        public void SetReference(ISimpleResource reference)
        {
            References.Add(reference);
        }

        public void SetMany(IReadOnlyList<ISimpleResource> references)
        {
            foreach (var reference in references)
                References.Add(reference);
        }

        public INonPublicResource NonPublic { get; set; }

        public event EventHandler<ISimpleResource> ReferenceChanged;

        public event EventHandler<ISimpleResource[]> SomeChanged;
    }

    public class BidirectionalReferenceResource : PublicResource
    {
        [ResourceReference(ResourceRelationType.Extension, ResourceReferenceRole.Source, nameof(SourceReference))]
        public ReferenceResource SourceReference { get; set; }
    }
}
