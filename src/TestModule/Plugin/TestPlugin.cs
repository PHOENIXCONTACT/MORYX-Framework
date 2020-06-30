// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.TestModule
{
    [ExpectedConfig(typeof(TestPluginConfig))]
    [Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
    public class TestPlugin : ITestPlugin
    {
        public const string ComponentName = "TestPlugin";

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
