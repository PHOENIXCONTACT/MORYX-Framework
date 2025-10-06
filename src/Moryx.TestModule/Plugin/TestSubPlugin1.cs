// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.TestModule
{
    [ExpectedConfig(typeof(TestSubPluginConfig1))]
    [Plugin(LifeCycle.Singleton, typeof(ITestSubPlugin), Name = ComponentName)]
    public class TestSubPlugin1 : ITestSubPlugin
    {
        public const string ComponentName = "TestSubPlugin1";

        public void Initialize(TestSubPluginConfig config)
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
