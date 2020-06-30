// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Products.Management.Modification
{
    [Plugin(LifeCycle.Transient, typeof(IProductInteraction))]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    internal class ProductInteraction : IProductInteraction
    {
        #region Dependencies

        public ModuleConfig Config { get; set; }

        public IProductConverterFactory ConverterFactory { get; set; }

        public IProductManager Manager { get; set; }

        #endregion

        public ProductCustomization GetCustomization()
        {
            return new ProductCustomization
            {
                ProductTypes = ReflectionTool.GetPublicClasses<ProductType>(new IsConfiguredFilter(Config.TypeStrategies).IsConfigured)
                    .Select(pt => new ProductDefinitionModel
                    {
                        Name = pt.Name,
                        DisplayName = pt.GetDisplayName() ?? pt.Name,
                        BaseDefinition = pt.BaseType?.Name
                    }).ToArray(),
                RecipeTypes = ReflectionTool.GetPublicClasses<IProductRecipe>(new IsConfiguredFilter(Config.RecipeStrategies).IsConfigured)
                    .Select(rt => new RecipeDefinitionModel
                    {
                        Name = rt.Name,
                        DisplayName = rt.GetDisplayName() ?? rt.Name,
                        HasWorkplans = typeof(IWorkplanRecipe).IsAssignableFrom(rt)
                    }).ToArray(),
                Importers = Manager.Importers.Select(i => new ProductImporter
                {
                    Name = i.Name,
                    Parameters = ConvertParameters(i.Parameters)
                }).ToArray()
            };
        }

        private class IsConfiguredFilter
        {
            private readonly IReadOnlyList<IProductStrategyConfiguation> _configurations;

            public IsConfiguredFilter(IReadOnlyList<IProductStrategyConfiguation> configurations)
            {
                _configurations = configurations;
            }

            public bool IsConfigured(Type candidate)
            {
                return _configurations.Any(config => config.TargetType == candidate.Name);
            }
        }

        private static Entry ConvertParameters(IImportParameters parametersObject) =>
            EntryConvert.EncodeObject(parametersObject, new PartialSerialization<IImportParameters>());

        public Entry UpdateParameters(string importerName, Entry currentParameters)
        {
            var parameters = ConvertParametersBack(importerName, currentParameters, true);
            return ConvertParameters(parameters);
        }

        private IImportParameters ConvertParametersBack(string importerName, Entry currentParameters, bool updateFirst = false)
        {
            var importer = Manager.Importers.First(i => i.Name == importerName);
            var parameters = (IImportParameters)EntryConvert.UpdateInstance(importer.Parameters, currentParameters);
            if (updateFirst)
                parameters = importer.Update(parameters);
            return parameters;
        }

        public ProductModel[] GetProducts(ProductQuery query)
        {
            return UseConverter(c => c.GetTypes(query));
        }

        public ProductModel CreateProduct(string type)
        {
            return UseConverter(c => c.Create(type));
        }

        public ProductModel GetProductDetails(long id)
        {
            return UseConverter(c => c.GetProduct(id));
        }

        public ProductModel SaveProduct(ProductModel instance)
        {
            return UseConverter(c => c.Save(instance));
        }

        public DuplicateProductResponse DuplicateProduct(long sourceId, string identifier, short revisionNo)
        {
            return UseConverter(c => c.Duplicate(sourceId, identifier, revisionNo));
        }

        public ProductModel ImportProduct(string importerName, Entry parametersModel)
        {
            var parameters = ConvertParametersBack(importerName, parametersModel);
            return UseConverter(c => c.ImportProduct(importerName, parameters));
        }

        public bool DeleteProduct(long id)
        {
            return UseConverter(c => c.DeleteProduct(id));
        }

        public string GetRecipeProviderName()
        {
            return ModuleController.ModuleName;
        }

        public RecipeModel GetRecipe(long recipeId)
        {
            return UseConverter(c => c.GetRecipe(recipeId));
        }

        public RecipeModel[] GetRecipes(long productId)
        {
            return UseConverter(c => c.GetRecipes(productId));
        }

        public RecipeModel CreateRecipe(string recipeType)
        {
            return UseConverter(c => c.CreateRecipe(recipeType));
        }

        public RecipeModel SaveRecipe(RecipeModel recipe)
        {
            return UseConverter(c => c.SaveRecipe(recipe));
        }

        public WorkplanModel[] GetWorkplans()
        {
            return UseConverter(c => c.GetWorkplans());
        }

        private TResult UseConverter<TResult>(Func<IProductConverter, TResult> call)
        {
            var converter = ConverterFactory.Create();
            var converted = call(converter);
            ConverterFactory.Destroy(converter);
            return converted;
        }
    }
}
