// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.AbstractionLayer.TestTools.Resources;

public interface INonPublicResource : IResource
{
}

public class NonPublicResource : Resource, INonPublicResource
{
}
