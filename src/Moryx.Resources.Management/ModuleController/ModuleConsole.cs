// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Resources.Management;

[ServerModuleConsole]
internal class ModuleConsole : IServerModuleConsole
{
    #region Dependencies
    private readonly IModuleLogger _logger;

    public ModuleConsole(IModuleLogger logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Factory to create the resource initializer
    /// </summary>
    public IResourceInitializerFactory InitializerFactory { get; set; }

    /// <summary>
    /// Resource manager to execute initializer
    /// </summary>
    public IResourceManager ResourceManager { get; set; }

    #endregion

    [EntrySerialize, DisplayName("Initialize Resource"), Description("Calls the configured resource initializers")]
    public string CallResourceInitializer([PluginConfigs(typeof(IResourceInitializer), true)] ResourceInitializerConfig[] configs)
    {
        foreach (var config in configs)
        {
            try
            {
                var initializer = InitializerFactory.Create(config, CancellationToken.None).GetAwaiter().GetResult();
                ResourceManager.ExecuteInitializer(initializer, null).Wait();
                InitializerFactory.Destroy(initializer);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Resource initializer failed for plugin {Plugin}",config.PluginName);
                return $"{config.PluginName} failed to run: {e.Message}";
            }
        }

        return "Success";
    }
}
