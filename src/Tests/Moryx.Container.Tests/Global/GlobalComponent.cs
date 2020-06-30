// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Container.Tests
{
    [GlobalComponent(LifeCycle.Singleton)]
    internal class GlobalComponent
    {
    }

    [Model("Some Namespace")]
    internal class ModelComponent
    {
        
    }
}
