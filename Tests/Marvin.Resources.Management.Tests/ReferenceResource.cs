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
            }
        }

        [ResourceReference(ResourceRelationType.PossibleExchangablePart)]
        public IEnumerable<IMyResource> MoreReferences { get; set; }

        public IMyResource GetReference()
        {
            return Reference;
        }

        public void SetReference(IMyResource reference)
        {
            MoreReferences = MoreReferences.Concat(new [] { reference }).ToArray();
        }

        public INonPublicResource NonPublic { get; set; }

        public event EventHandler<IMyResource> ReferenceChanged;
    }
}