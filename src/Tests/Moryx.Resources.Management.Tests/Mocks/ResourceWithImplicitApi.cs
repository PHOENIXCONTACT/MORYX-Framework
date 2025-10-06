// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Management.Tests
{
    public interface IExtension
    {
        int Add(int value);
    }

    public interface IResourceWithImplicitApi : IResource, IExtension
    {

    }

    public class ResourceWithImplicitApi : Resource, IResourceWithImplicitApi
    {
        public int Add(int value)
        {
            return value + 10;
        }
    }
}
