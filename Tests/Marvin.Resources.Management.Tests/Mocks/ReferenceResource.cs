using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public class ReferenceResource : PublicResource, IReferenceResource
    {
        private IMyResource _reference;

        [ResourceReference(ResourceRelationType.CurrentExchangablePart)]
        public IMyResource Reference
        {
            get { return _reference; }
            set
            {
                _reference = value;
                ReferenceChanged?.Invoke(this, value);
                SomeChanged?.Invoke(this, new[] { value });
            }
        }

        [ResourceReference(ResourceRelationType.PossibleExchangablePart)]
        public IReferences<IMyResource> References { get; set; }

        [ReferenceOverride(nameof(Children), AutoSave = true)]
        internal IReferences<IMyResource> ChildReferences { get; set; }

        IEnumerable<IMyResource> IReferenceResource.MoreReferences => References;

        public IMyResource GetReference()
        {
            return Reference;
        }

        public IReadOnlyList<IMyResource> GetReferences()
        {
            return References.ToArray();
        }

        public void SetReference(IMyResource reference)
        {
            References.Add(reference);
        }

        public INonPublicResource NonPublic { get; set; }

        public event EventHandler<IMyResource> ReferenceChanged;

        public event EventHandler<IMyResource[]> SomeChanged;
    }
}