// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container.TestTools;

namespace Moryx.Container.TestPlugin;

[Plugin(LifeCycle.Singleton, typeof(IConfiguredComponent), Name = PluginName)]
public class ForeignComponent : IConfiguredComponent
{
    public const string PluginName = "ForeignComponent";

    public string GetName()
    {
        return PluginName;
    }

    /// <inheritdoc />
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