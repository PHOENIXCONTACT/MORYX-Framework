// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;

namespace Moryx.Startup.Hooks;

public class ProductHookConfig
{
    public class ImporterConfig
    {
        /// <summary>
        /// Allows disabling this config entry
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Only run this hook, when the database contains no products
        /// </summary>
        public bool OnlyOnFreshDb { get; set; }

        /// <summary>
        /// Name of the product importer to run
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Name of the importer parameter type
        /// </summary>
        public string? ConfigType { get; set; }

        /// <summary>
        /// Importer Parameters
        /// </summary>
        public IConfigurationSection? Parameters { get; set; }
    }
    public ImporterConfig[] Importers { get; set; } = [];
}
