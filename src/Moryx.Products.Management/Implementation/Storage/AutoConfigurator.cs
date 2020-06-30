// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Modules;
using Moryx.Products.Model;
using Moryx.Runtime.Configuration;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Component that can automatically configure the strategies 
    /// </summary>
    [Component(LifeCycle.Singleton)]
    internal class AutoConfigurator
    {
        /// <summary>
        /// As a meta-component the configurator can gain access to 
        /// </summary>
        public IContainer Container { get; set; }

        /// <summary>
        /// Configuration of the module
        /// </summary>
        public ModuleConfig Config { get; set; }

        /// <summary>
        /// Add all necessary config entries for the product
        /// </summary>
        public string ConfigureType(string productType)
        {
            var productTypes = ReflectionTool.GetPublicClasses<ProductType>();
            var product = productTypes.FirstOrDefault(p => p.Name == productType);
            if (product == null)
                return $"Found no product type {productType ?? string.Empty}!\n" +
                    "Available types: " + string.Join(", ", productTypes.Select(t => t.Name));

            var result = string.Empty;
            // First try to select the best type strategy
            if (Config.TypeStrategies.Any(s => s.TargetType == productType))
            {
                result += $"Product {productType} already has a TypeStrategy configured. Overwriting configs is not supported!\n";
            }
            else
            {
                var typeConfig = StrategyConfig<IProductTypeStrategy, ProductTypeConfiguration, ProductType>(product);
                if (typeConfig == null)
                {
                    result += $"Found no matching type strategy for {productType}. Make sure you declared the '{nameof(StrategyConfigurationAttribute)}'!\n";
                }
                else
                {
                    Config.TypeStrategies.Add(typeConfig);
                    result += $"Selected TypeStrategy {typeConfig.PluginName} for {productType}.";
                }
            }

            // Configure part links
            // TODO: Use type wrapper
            var links = product.GetProperties()
                .Where(p => typeof(IProductPartLink).IsAssignableFrom(p.PropertyType) || typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(p.PropertyType));
            foreach (var link in links)
            {
                if (Config.LinkStrategies.Any(s => s.TargetType == productType && s.PartName == link.Name))
                {
                    result += $"Product {productType} already has a LinkStrategy for {link.Name}. \n";
                }
                else
                {
                    var linkType = link.PropertyType;
                    if (typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(linkType))
                        linkType = linkType.GetGenericArguments()[0];
                    var linkConfig = StrategyConfig<IProductLinkStrategy, ProductLinkConfiguration, ProductPartLink>(linkType);
                    if (linkConfig == null)
                    {
                        result += $"Found no matching instance strategy for {productType}. Make sure you declared the '{nameof(StrategyConfigurationAttribute)}'!\n";
                    }
                    else
                    {
                        linkConfig.TargetType = productType;
                        linkConfig.PartName = link.Name;
                        Config.LinkStrategies.Add(linkConfig);
                        result += $"Selected LinkStrategy {linkConfig.PluginName} for {productType}.{link.Name}.";
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Configure storage for <see cref="ProductInstance"/>
        /// </summary>
        public string ConfigureInstance(string instanceType)
        {
            var types = ReflectionTool.GetPublicClasses<ProductInstance>();
            var instance = types.FirstOrDefault(p => p.Name == instanceType);
            if (instance == null)
                return $"Found no instance type {instanceType ?? string.Empty}!\n" +
                       "Available types: " + string.Join(", ", types.Select(t => t.Name));

            var result = string.Empty;
            if (Config.InstanceStrategies.Any(s => s.TargetType == instanceType))
            {
                result += $"Product {instanceType} already has an InstanceStrategy configured. Overwriting configs is not supported!\n";
            }
            else
            {
                var instanceConfig = StrategyConfig<IProductInstanceStrategy, ProductInstanceConfiguration, ProductInstance>(instance);
                if (instanceConfig == null)
                {
                    result += $"Found no matching instance strategy for {instanceType}. Make sure you declared the '{nameof(StrategyConfigurationAttribute)}'!\n";
                }
                else
                {
                    Config.InstanceStrategies.Add(instanceConfig);
                    result += $"Selected InstanceStrategy {instanceConfig.PluginName} for {instanceType}.";
                }
            }

            return result;
        }

        /// <summary>
        /// Configure storage for <see cref="ProductRecipe"/>
        /// </summary>
        public string ConfigureRecipe(string recipeType)
        {
            var types = ReflectionTool.GetPublicClasses<IProductRecipe>();
            var recipe = types.FirstOrDefault(p => p.Name == recipeType);
            if (recipe == null)
                return $"Found no recipe type {recipeType ?? string.Empty}!\n" +
                       "Available types: " + string.Join(", ", types.Select(t => t.Name));

            var result = string.Empty;
            if (Config.RecipeStrategies.Any(s => s.TargetType == recipeType))
            {
                result += $"Recipe {recipeType} already has a RecipeStrategy configured. Overwriting configs is not supported!\n";
            }
            else
            {
                // ProductionRecipe is technically not the correct base type, BUT it defines all relevant properties, which is the only thing that matters
                var recipeConfig = StrategyConfig<IProductRecipeStrategy, ProductRecipeConfiguration, ProductionRecipe>(recipe);
                if (recipeConfig == null)
                {
                    result += $"Found no matching recipe strategy for {recipeType}. Make sure you declared the '{nameof(StrategyConfigurationAttribute)}'!\n";
                }
                else
                {
                    Config.RecipeStrategies.Add(recipeConfig);
                    result += $"Selected RecipeStrategy {recipeConfig.PluginName} for {recipeType}.";
                }
            }

            return result;
        }

        private TConfig StrategyConfig<TStrategy, TConfig, TBaseType>(Type targetType)
            where TConfig : class, IProductStrategyConfiguation
        {
            var tuple = CreateConfig<TStrategy, TConfig>(targetType);
            if (tuple == null)
                return null;

            var config = tuple.Item2;
            config.TargetType = targetType.Name;
            config.PluginName = tuple.Item1.GetCustomAttribute<RegistrationAttribute>().Name;

            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Optionally try to configure property mappers
            var propertyMapperConfig = config as IPropertyMappedConfiguration;
            if (propertyMapperConfig != null)
            {
                var remainingColumns = typeof(IGenericColumns).GetProperties()
                    .OrderBy(p => p.Name).ToList();
                // TODO: Use type wrapper
                var baseProperties = typeof(TBaseType).GetProperties();
                foreach (var property in targetType.GetProperties().Where(p => baseProperties.All(bp => bp.Name != p.Name))
                    .Where(p => !typeof(IProductPartLink).IsAssignableFrom(p.PropertyType) & !typeof(IEnumerable<IProductPartLink>).IsAssignableFrom(p.PropertyType))
                    .Where(p => !typeof(ProductInstance).IsAssignableFrom(p.PropertyType) & !typeof(IEnumerable<ProductInstance>).IsAssignableFrom(p.PropertyType)))
                {
                    var propertyTuple = CreateConfig<IPropertyMapper, PropertyMapperConfig>(property.PropertyType);
                    if (propertyTuple == null)
                        continue;

                    var strategy = propertyTuple.Item1;
                    var propertyConfig = propertyTuple.Item2;
                    propertyConfig.PropertyName = property.Name;
                    propertyConfig.PluginName = strategy.GetCustomAttribute<RegistrationAttribute>().Name;

                    var columnType = strategy.GetCustomAttribute<PropertyStrategyConfigurationAttribute>()?.ColumnType ?? typeof(string);
                    var column = remainingColumns.FirstOrDefault(rc => rc.PropertyType == columnType);
                    if (column == null)
                        continue;

                    remainingColumns.Remove(column);
                    propertyConfig.Column = column.Name;

                    ValueProviderExecutor.Execute(propertyConfig, new ValueProviderExecutorSettings().AddDefaultValueProvider());
                    propertyMapperConfig.PropertyConfigs.Add(propertyConfig);
                }
            }

            return config;
        }

        private Tuple<Type, TConfig> CreateConfig<TStrategy, TConfig>(Type targetType)
        {
            var strategies = Container.GetRegisteredImplementations(typeof(TStrategy));
            var typeStrategy = SelectStrategy(strategies, targetType);
            if (typeStrategy == null)
                return null;

            var configType = typeStrategy.GetCustomAttribute<ExpectedConfigAttribute>()?.ExcpectedConfigType
                             ?? typeof(TConfig);
            var config = (TConfig)Activator.CreateInstance(configType);

            return Tuple.Create(typeStrategy, config);
        }

        /// <summary>
        /// Select the best strategy based on the <see cref="StrategyConfigurationAttribute"/>
        /// </summary>
        private Type SelectStrategy(IEnumerable<Type> strategies, Type targetType)
        {
            Type currentMatch = null;
            var currentCompliance = StrategyConfigurationAttribute.BadCompliance;

            foreach (var strategy in strategies)
            {
                var att = strategy.GetCustomAttribute<StrategyConfigurationAttribute>();
                // Find match by comparing TypeCompliance index
                var compliance = att?.TypeCompliance(targetType) ?? StrategyConfigurationAttribute.BadCompliance;
                if (compliance < currentCompliance)
                {
                    currentCompliance = compliance;
                    currentMatch = strategy;
                }
            }

            return currentMatch;
        }
    }
}
