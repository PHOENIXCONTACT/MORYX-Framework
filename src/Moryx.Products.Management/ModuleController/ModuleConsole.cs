// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
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

        [EntrySerialize]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureType([PossibleTypes(typeof(ProductType), UseFullname = true)] string productType)
        {
            return Configurator.ConfigureType(productType);
        }

        [EntrySerialize]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureInstance([PossibleTypes(typeof(ProductInstance), UseFullname = true)] string instanceType)
        {
            return Configurator.ConfigureInstance(instanceType);
        }

        [EntrySerialize]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureRecipe([PossibleTypes(typeof(IProductRecipe), UseFullname = true)] string recipeType)
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
