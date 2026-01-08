// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;

namespace Moryx.TestModule;

[ExpectedConfig(typeof(TestPluginConfig1))]
[Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
public class TestPlugin1 : ITestPlugin
{
    public const string ComponentName = "TestPlugin1";

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