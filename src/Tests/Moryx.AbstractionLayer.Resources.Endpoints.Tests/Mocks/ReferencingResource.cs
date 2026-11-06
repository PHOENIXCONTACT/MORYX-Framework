// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.TestTools;

namespace Moryx.AbstractionLayer.Resources.Endpoints.Tests.Mocks;

internal sealed class ReferencingResource : Resource
{
    [ResourceReference(ResourceRelationType.Custom)]
    public ReferenceCollectionMock<IReferencedResource> References { get; set; } = new();
}
