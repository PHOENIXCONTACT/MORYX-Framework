// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.TestModule
{
    [ExpectedConfig(typeof(TestPluginConfig2))]
    [Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
    public class TestPlugin2 : ITestPlugin
    {
        public const string ComponentName = "TestPlugin2";

        public void Initialize(TestPluginConfig config)
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
