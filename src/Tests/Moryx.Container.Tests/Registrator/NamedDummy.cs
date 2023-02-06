// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests
{
    [Registration(LifeCycle.Transient, Name = "Dummy")]
    internal class NamedDummy
    {
    }

    [Registration(LifeCycle.Transient)]
    internal class UnnamedDummy
    {
        
    }
}
