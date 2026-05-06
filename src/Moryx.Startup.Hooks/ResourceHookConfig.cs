// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;

namespace Moryx.Startup.Hooks;

public class ResourceHookConfig
{
    public class InitializerConfig
    {
        /// <summary>
        /// Allows disabling this config entry
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Only run this hook, when the database contains no resources
        /// </summary>
        public bool OnlyOnFreshDb { get; set; }
        /// <summary>
        /// Name of the ResourceInitializer to run
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Type name of the iInitializer parameters
        /// </summary>
        public string? ConfigType { get; set; }
        /// <summary>
        /// Initializer parameters
        /// </summary>
        public IConfigurationSection? Parameters { get; set; }
    }

    /// <summary>
    /// List of initializers to run
    /// </summary>
    public InitializerConfig[] Initializers { get; set; } = [];
}
