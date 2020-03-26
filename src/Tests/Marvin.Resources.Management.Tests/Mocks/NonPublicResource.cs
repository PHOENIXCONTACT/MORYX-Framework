// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Management.Tests
{
    public interface INonPublicResource : IResource
    {
    }

    public class NonPublicResource : Resource, INonPublicResource
    {
    }
}
