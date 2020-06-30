// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests
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
