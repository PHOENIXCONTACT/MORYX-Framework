// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Container;
using Marvin.Modules;

namespace Marvin.TestModule
{
    [ExpectedConfig(typeof(TestSubPluginConfig))]
    [Plugin(LifeCycle.Singleton, typeof(ITestSubPlugin), Name = ComponentName)]
    public class TestSubPlugin : ITestSubPlugin
    {
        public const string ComponentName = "TestSubPlugin";

        public void Initialize(TestSubPluginConfig config)
        {
        }

        public void Dispose()
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
