// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.AbstractionLayer.Resources;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Resources.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        #region Dependencies

        /// <summary>
        /// Factory to create the resource initializer
        /// </summary>
        public IResourceInitializerFactory InitializerFactory { get; set; }

        /// <summary>
        /// Config to load all configured initializer
        /// </summary>
        public ModuleConfig ModuleConfig { get; set; }

        /// <summary>
        /// Resource manager to execute initializer
        /// </summary>
        public IResourceManager ResourceManager { get; set; }

        #endregion

        [EntrySerialize, DisplayName("Initialize Resource"), Description("Calls the configured resource initializer")]
        public string CallResourceInitializer([PluginConfigs(typeof(IResourceInitializer), true)] ResourceInitializerConfig[] configs)
        {
            foreach (var config in configs)
            {
                try
                {
                    var initializer = InitializerFactory.Create(config);
                    ResourceManager.ExecuteInitializer(initializer);
                    InitializerFactory.Destroy(initializer);
                }
                catch (Exception e)
                {
                    return $"{config.PluginName} failed to run: {e.Message}";
                }
            }

            return "Success";
        }

    }
}
