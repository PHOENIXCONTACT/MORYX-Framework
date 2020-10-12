// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Configuration;
using Moryx.Runtime.Modules;
using Moryx.Serialization;

namespace Moryx.Products.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public AutoConfigurator Configurator { get; set; }

        public IConfigManager ConfigManager { get; set; }

        public void ExecuteCommand(string[] args, Action<string> outputStream)
        {
            if (!args.Any())
                outputStream("ProductManagement console requires arguments");

            switch (args[0])
            {
                case "configure":
                    var product = args.Length >= 2 ? args[1] : null;
                    outputStream($"Running configurator for {product}");
                    var result = Configurator.ConfigureType(product);
                    outputStream(result);
                    break;
            }
        }

        [EntrySerialize]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureType([PossibleTypes(typeof(ProductType))] string productType)
        {
            return Configurator.ConfigureType(productType);
        }

        [EntrySerialize]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureInstance([PossibleTypes(typeof(ProductInstance))] string instanceType)
        {
            return Configurator.ConfigureInstance(instanceType);
        }

        [EntrySerialize]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureRecipe([PossibleTypes(typeof(IProductRecipe))] string recipeType)
        {
            return Configurator.ConfigureRecipe(recipeType);
        }

        [EntrySerialize]
        [Description("Save latest changes to config")]
        public void SaveConfig()
        {
            ConfigManager.SaveConfiguration(Configurator.Config);
        }
    }
}
