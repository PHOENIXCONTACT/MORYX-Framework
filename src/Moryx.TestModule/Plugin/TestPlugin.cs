// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Samples.Strategies;

namespace Moryx.TestModule;

[ExpectedConfig(typeof(TestPluginConfig))]
[Plugin(LifeCycle.Singleton, typeof(ITestPlugin), Name = ComponentName)]
public class TestPlugin : ITestPlugin
{
    public const string ComponentName = "TestPlugin";

    [UseChild("Plugin")]
    public ILogger Logger { get; set; }

    public ModuleConfig Config { get; set; }

    public IWhiteSpaceRemovingStrategyFactory StrategyFactory { get; set; }

    private IWhiteSpaceRemovingStrategy _strategy;

    public void Initialize(TestPluginConfig config)
    {
    }

    public void Start()
    {
        // Config changes restart the module, so the strategy is created once per start
        _strategy = StrategyFactory.Create(Config.WhiteSpaceRemovingStrategy);
        Logger.LogDebug("Strategy {Strategy} result: {Result}", Config.WhiteSpaceRemovingStrategy,
            _strategy.DropSpaces("Hello MORYX"));
    }

    public void Stop()
    {
        if (_strategy != null)
        {
            StrategyFactory.Destroy(_strategy);
        }

        _strategy = null;
    }
}
