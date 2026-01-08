// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Container.Tests
{
    [Component(LifeCycle.Transient, Name = "Dummy")]
    public class NamedDummy
    {
    }

    [Component(LifeCycle.Transient)]
    public class UnnamedDummy
    {

    }
}
