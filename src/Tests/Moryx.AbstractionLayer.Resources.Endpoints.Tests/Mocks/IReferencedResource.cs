// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources.Endpoints.Tests.Mocks;

internal interface IReferencedResource : IResource
{
}

internal class ReferencedResource : Resource, IReferencedResource
{
}