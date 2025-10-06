// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.TestModule
{
    [DataContract]
    public class TestSubPluginConfig2 : TestSubPluginConfig
    {
        public override string PluginName { get { return TestSubPlugin2.ComponentName; } }

    }
}
