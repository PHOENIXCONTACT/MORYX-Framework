// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests
{
    public class InterferenceResource : Resource
    {
        [ResourceReference(ResourceRelationType.CurrentExchangeablePart)]
        public DerivedResource Derived { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangeablePart)]
        public IReferences<OtherResource> Others { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangeablePart)]
        public DifferentResource Different { get; set; }
    }
}
