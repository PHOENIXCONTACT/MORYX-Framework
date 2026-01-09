// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container.TestTools;

namespace Moryx.Container.TestRootPlugin;

[Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
public class ForeignDefaultComponent : IConfiguredComponent
{
    public const string PluginName = "ForeignDefaultComponent";

    public string GetName()
    {
        return PluginName;
    }

    /// <summary>
    /// Initialize this component with its config
    /// </summary>
    /// <param name="config">Config of this module plugin</param>
    public void Initialize(ComponentConfig config)
    {
    }

    /// <inheritdoc />
    public void Start()
    {
    }

    /// <inheritdoc />
    public void Stop()
    {
    }
}