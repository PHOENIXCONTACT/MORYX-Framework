// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.AbstractionLayer.Recipes;
using Marvin.Configuration;
using Marvin.Runtime.Modules;
using Marvin.Serialization;

namespace Marvin.Products.Management
{
    [ServerModuleConsole]
    internal class ModuleConsole : IServerModuleConsole
    {
        public AutoConfigurator Configurator { get; set; }

        public string ExportDescription(DescriptionExportFormat format)
        {
            switch (format)
            {
                case DescriptionExportFormat.Console:
                    return ExportConsoleDescription();
                case DescriptionExportFormat.Documentation:
                    return ExportHtmlDescription();
            }
            return string.Empty;
        }

        // Export your desription for the developer console here
        // This should represent the current state
        private static string ExportConsoleDescription()
        {
            return "";
        }

        // Export your desription for the supervisor or maintenance
        // This should be a static explaination of the plugin
        private static string ExportHtmlDescription()
        {
            return "";
        }

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

        [EditorVisible]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureType([PossibleTypes(typeof(ProductType))] string productType)
        {
            return Configurator.ConfigureType(productType);
        }

        [EditorVisible]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureInstance([PossibleTypes(typeof(ProductInstance))] string instanceType)
        {
            return Configurator.ConfigureInstance(instanceType);
        }

        [EditorVisible]
        [Description("Automatically configure the necessary strategies for a product type.")]
        public string ConfigureRecipe([PossibleTypes(typeof(IProductRecipe))] string recipeType)
        {
            return Configurator.ConfigureRecipe(recipeType);
        }
    }
}
