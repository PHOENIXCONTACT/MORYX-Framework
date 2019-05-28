using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public class InterferenceResource : Resource
    {
        [ResourceReference(ResourceRelationType.CurrentExchangablePart)]
        public DerivedResource Derived { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart)]
        public IReferences<OtherResource> Others { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart)]
        public DifferentResource Different { get; set; }
    }
}