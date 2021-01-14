// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Modules
{
    /// <summary>
    /// This generic interface is intended for all plugins that require a configuration for their initialization to work properly.
    /// This configuration is passed to the plugin via the Initialize(TConf config) method.
    /// </summary>
    public interface IConfiguredPlugin<in T> : IPlugin, IConfiguredInitializable<T>
        where T : IPluginConfig
    {
    }
}
