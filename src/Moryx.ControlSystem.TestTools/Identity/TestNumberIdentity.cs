// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;

namespace Moryx.ControlSystem.TestTools.Identity;

public class TestNumberIdentity : NumberIdentity
{
    public TestNumberIdentity(int numberType) : base(numberType)
    {
    }

    public TestNumberIdentity(int numberType, string identifier) : base(numberType, identifier)
    {
    }
}