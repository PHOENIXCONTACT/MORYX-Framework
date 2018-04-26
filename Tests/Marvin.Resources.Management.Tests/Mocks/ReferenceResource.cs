using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public interface IReferenceResource : IPublicResource
    {
        ISimpleResource Reference { get; set; }

        IEnumerable<ISimpleResource> MoreReferences { get; }

        INonPublicResource NonPublic { get; }

        ISimpleResource GetReference();

        IReadOnlyList<ISimpleResource> GetReferences();

        void SetReference(ISimpleResource reference);

        event EventHandler<ISimpleResource> ReferenceChanged;

        event EventHandler<ISimpleResource[]> SomeChanged;
    }

    public class ReferenceResource : PublicResource, IReferenceResource
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

        [ResourceReference(ResourceRelationType.PossibleExchangablePart)]
        public IReferences<ISimpleResource> References { get; set; }

        [ReferenceOverride(nameof(Children), AutoSave = true)]
        internal IReferences<ISimpleResource> ChildReferences { get; set; }

        IEnumerable<ISimpleResource> IReferenceResource.MoreReferences => References;

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

        public INonPublicResource NonPublic { get; set; }

        public event EventHandler<ISimpleResource> ReferenceChanged;

        public event EventHandler<ISimpleResource[]> SomeChanged;
    }
}