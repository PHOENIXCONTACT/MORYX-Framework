// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Workplans;
using System.Collections;
using System.Reflection;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    internal class ProductConverter
    {
        private readonly IProductManagement _productManagement;

        // Null object pattern for identity
        private static readonly ProductIdentity EmptyIdentity = new(string.Empty, 0);

        private readonly ICustomSerialization _productSerialization;
        private readonly ICustomSerialization _recipeSerialization;

        public IContainer ProductManagerContainer { get; }
        public IServiceProvider GlobalContainer { get; }

        public ProductConverter(IProductManagement productManagement, IContainer localContainer, IServiceProvider globalContainer)
        {
            _productManagement = productManagement;
            ProductManagerContainer = localContainer;
            GlobalContainer = globalContainer;
            _productSerialization = new PartialSerialization<ProductType>(localContainer, globalContainer);
            _recipeSerialization = new PartialSerialization<ProductionRecipe>(localContainer, globalContainer);
        }

        public ProductDefinitionModel ConvertProductType(Type productType)
        {
            var baseType = productType.BaseType;
            var baseTypeName = baseType == typeof(ProductType)
                ? string.Empty : baseType.FullName;

            return new()
            {
                Name = productType.FullName,
                DisplayName = productType.GetDisplayName() ?? productType.Name,
                BaseDefinition = baseTypeName,
                Properties = EntryConvert.EncodeClass(productType, _productSerialization)
            };
        }

        public static RecipeDefinitionModel ConvertRecipeType(Type recipeType)
        {
            return new()
            {
                Name = recipeType.FullName,
                DisplayName = recipeType.GetDisplayName() ?? recipeType.Name,
                HasWorkplans = typeof(IWorkplanRecipe).IsAssignableFrom(recipeType)
            };
        }
        public async Task<ProductModel> ConvertProduct(ProductType productType, bool flat)
        {

            // Base object
            var identity = (ProductIdentity)productType.Identity ?? EmptyIdentity;
            var converted = new ProductModel
            {
                Id = productType.Id,
                Type = productType.GetType().FullName,
                Name = productType.Name,
                State = productType.State,
                Identifier = identity.Identifier,
                Revision = identity.Revision
            };

            if (flat)
                return converted;

            // Properties
            var typeWrapper = _productManagement.GetTypeWrapper(productType.GetType().FullName);
            var properties = typeWrapper != null ? typeWrapper.Properties.ToArray() : productType.GetType().GetProperties();
            converted.Properties = EntryConvert.EncodeObject(productType, _productSerialization);

            // Recipes
            var recipes = await _productManagement.LoadRecipesAsync(productType, RecipeClassification.CloneFilter);
            converted.Recipes = recipes.Select(ConvertRecipe).ToArray();

            // Parts
            await ConvertParts(productType, properties, converted);

            return converted;
        }

        private async Task ConvertParts(ProductType productType, IEnumerable<PropertyInfo> properties, ProductModel converted)
        {
            var connectors = new List<PartConnector>();
            foreach (var property in properties)
            {
                var displayName = property.GetDisplayName();

                if (typeof(ProductPartLink).IsAssignableFrom(property.PropertyType))
                {
                    var link = (ProductPartLink)property.GetValue(productType);
                    var partModel = await ConvertPart(link);
                    var connector = new PartConnector
                    {
                        Name = property.Name,
                        DisplayName = displayName,
                        Type = FetchProductType(property.PropertyType),
                        Parts = partModel is null ? [] : [partModel],
                        PropertyTemplates = EntryConvert.EncodeClass(property.PropertyType, _productSerialization)
                    };
                    connectors.Add(connector);
                }
                else if (typeof(IEnumerable<ProductPartLink>).IsAssignableFrom(property.PropertyType))
                {
                    var links = (IEnumerable<ProductPartLink>)property.GetValue(productType);
                    var linkType = property.PropertyType.GetGenericArguments()[0];
                    var connector = new PartConnector
                    {
                        IsCollection = true,
                        Name = property.Name,
                        DisplayName = displayName,
                        Type = FetchProductType(linkType),
                        PropertyTemplates = EntryConvert.EncodeClass(linkType, _productSerialization)
                    };

                    var convertPartTasks = links?.Select(ConvertPart);
                    if (convertPartTasks != null)
                    {
                        connector.Parts = await Task.WhenAll(convertPartTasks);
                    }

                    connectors.Add(connector);
                }
            }
            converted.Parts = connectors.ToArray();
        }

        private static string FetchProductType(Type linkType)
        {
            var partLinkBase = FindGenericBaseDefinition(linkType, typeof(ProductPartLink<>));
            if (partLinkBase == null)
            {
                throw new InvalidOperationException($"Cannot find product type for {linkType.FullName}");
            }

            var prodType = partLinkBase.GetGenericArguments()[0];
            return prodType.FullName;
        }

        private static Type FindGenericBaseDefinition(Type type, Type genericDefinition)
        {
            var current = type;
            while (current != null && current != typeof(object))
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == genericDefinition)
                    return current;

                current = current.BaseType;
            }

            return null;
        }

        private async Task<PartModel> ConvertPart(ProductPartLink link)
        {
            // No link, no DTO!
            if (link is null || link.Product is null)
                return null;

            var part = new PartModel
            {
                Id = link.Id,
                Product = await ConvertProduct(link.Product, true),
                Properties = EntryConvert.EncodeObject(link, _productSerialization)
            };
            return part;
        }

        public async Task<ProductType> ConvertProductBack(ProductModel source, ProductType converted)
        {
            // Copy base values
            converted.Identity = new ProductIdentity(source.Identifier, source.Revision);
            converted.Name = source.Name;
            converted.State = source.State;

            // Save recipes
            var recipes = new List<IProductRecipe>(source.Recipes?.Length ?? 0);
            foreach (var recipeModel in source.Recipes ?? Enumerable.Empty<RecipeModel>())
            {
                IProductRecipe productRecipe;
                if (recipeModel.Id == 0)
                {
                    var type = ReflectionTool.GetPublicClasses<IProductRecipe>(t => t.Name == recipeModel.Type).First();
                    productRecipe = (IProductRecipe)Activator.CreateInstance(type);
                }
                else
                    productRecipe = (IProductRecipe)await _productManagement.LoadRecipeAsync(recipeModel.Id);

                ConvertRecipeBack(recipeModel, productRecipe, converted);
                recipes.Add(productRecipe);
            }
            if (recipes.Any())
                foreach (var recipe in recipes)
                    await _productManagement.SaveRecipeAsync(recipe);

            // Product is flat
            if (source.Properties is null)
                return converted;

            // Delete recipes
            if (converted.Id != 0)
            {
                var recipesOfProduct = await _productManagement.LoadRecipesAsync(converted, RecipeClassification.CloneFilter);
                foreach (var recipe in recipesOfProduct)
                    if (recipes.FirstOrDefault(r => r.Id == recipe.Id) == null)
                        await _productManagement.DeleteRecipeAsync(recipe.Id);
            }

            // Copy extended properties
            var typeWrapper = _productManagement.GetTypeWrapper(converted.GetType().FullName);
            var properties = typeWrapper != null ? typeWrapper.Properties.ToArray() : converted.GetType().GetProperties();
            EntryConvert.UpdateInstance(converted, source.Properties, _productSerialization);

            // Convert parts
            foreach (var partConnector in source.Parts ?? Enumerable.Empty<PartConnector>())
            {
                if (partConnector.Parts is null)
                    continue;

                var prop = properties.First(p => p.Name == partConnector.Name);
                var value = prop.GetValue(converted);
                if (partConnector.IsCollection)
                {
                    if (value == null)
                    {
                        value = Activator.CreateInstance(typeof(List<>)
                            .MakeGenericType(prop.PropertyType.GetGenericArguments().First()));
                        prop.SetValue(converted, value);
                    }
                    await UpdateCollection((IList)value, partConnector.Parts);
                }
                else if (partConnector.Parts.Length == 1)
                {
                    if (value == null)
                    {
                        value = Activator.CreateInstance(prop.PropertyType);
                        prop.SetValue(converted, value);
                    }
                    await UpdateReference((ProductPartLink)value, partConnector.Parts[0]);
                }
                else if (partConnector.Parts.Length == 0)
                {
                    prop.SetValue(converted, null);
                }
            }

            return converted;
        }

        private async Task UpdateCollection(IList value, IEnumerable<PartModel> parts)
        {
            // Track which part links are still represented by the models
            var oldParts = new List<ProductPartLink>(value.OfType<ProductPartLink>());
            // Iterate over the part models
            // Create or update the part links
            var elemType = value.GetType().GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>))
                .Select(i => i.GetGenericArguments()[0]).Single();
            foreach (var partModel in parts)
            {
                if (partModel is null)
                    continue;

                var oldPartMatch = oldParts.Find(r => r.Id == partModel.Id);
                // new partlink
                if (oldPartMatch == null)
                {
                    oldPartMatch = (ProductPartLink)Activator.CreateInstance(elemType);
                    oldPartMatch.Product = await _productManagement.LoadTypeAsync(partModel.Product.Id);
                    value.Add(oldPartMatch);
                }
                //modified reference
                else if (oldPartMatch.Product.Id != partModel.Product.Id)
                    oldPartMatch.Product = await _productManagement.LoadTypeAsync(partModel.Product.Id);
                else
                    // existing unchanged partlink: do not delete at the end
                    oldParts.Remove(oldPartMatch);

                EntryConvert.UpdateInstance(oldPartMatch, partModel.Properties);
            }

            // Clear all values no longer present in the model
            foreach (var part in oldParts)
                value.Remove(part);
        }

        private async Task UpdateReference(ProductPartLink value, PartModel part)
        {
            EntryConvert.UpdateInstance(value, part.Properties);
            value.Product = part.Product is null ? null : await _productManagement.LoadTypeAsync(part.Product.Id);
        }

        public RecipeModel ConvertRecipe(IRecipe recipe) => ConvertRecipe(recipe, _recipeSerialization);

        private static RecipeModel ConvertRecipe(IRecipe recipe, ICustomSerialization serialization)
        {
            // Transform to DTO and transmit
            var converted = new RecipeModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Type = recipe.GetType().Name,
                State = recipe.State,
                Revision = recipe.Revision,
                Properties = EntryConvert.EncodeObject(recipe, serialization),
                IsClone = recipe.Classification.HasFlag(RecipeClassification.Clone)
            };

            switch (recipe.Classification & RecipeClassification.CloneFilter)
            {
                case RecipeClassification.Unset:
                    converted.Classification = RecipeClassificationModel.Unset;
                    break;
                case RecipeClassification.Default:
                    converted.Classification = RecipeClassificationModel.Default;
                    break;
                case RecipeClassification.Alternative:
                    converted.Classification = RecipeClassificationModel.Alternative;
                    break;
                case RecipeClassification.Intermediate:
                    converted.Classification = RecipeClassificationModel.Intermediate;
                    break;
                case RecipeClassification.Part:
                    converted.Classification = RecipeClassificationModel.Part;
                    break;
            }

            var wpRecipe = recipe as IWorkplanRecipe;
            if (wpRecipe?.Workplan != null)
            {
                converted.WorkplanModel = ConvertWorkplan(wpRecipe.Workplan);
            }

            return converted;
        }

        public async Task<IProductRecipe> ConvertRecipeBack(RecipeModel recipe, IProductRecipe productRecipe, ProductType productType)
        {
            productRecipe.Name = recipe.Name;
            productRecipe.Revision = recipe.Revision;
            productRecipe.State = recipe.State;

            // Only load workplan if it changed
            var workplanRecipe = productRecipe as IWorkplanRecipe;
            if (workplanRecipe != null && workplanRecipe.Workplan?.Id != recipe.WorkplanModel.Id)
                workplanRecipe.Workplan = await _productManagement.LoadWorkplanAsync(recipe.WorkplanModel.Id);

            if (productRecipe.Product == null)
            {
                productRecipe.Product = productType;
            }

            EntryConvert.UpdateInstance(productRecipe, recipe.Properties, _recipeSerialization);

            // Do not update a clones classification
            if (productRecipe.Classification.HasFlag(RecipeClassification.Clone))
                return productRecipe;

            switch (recipe.Classification)
            {
                case RecipeClassificationModel.Unset:
                    productRecipe.Classification = RecipeClassification.Unset;
                    break;
                case RecipeClassificationModel.Default:
                    productRecipe.Classification = RecipeClassification.Default;
                    break;
                case RecipeClassificationModel.Alternative:
                    productRecipe.Classification = RecipeClassification.Alternative;
                    break;
                case RecipeClassificationModel.Intermediate:
                    productRecipe.Classification = RecipeClassification.Intermediate;
                    break;
                case RecipeClassificationModel.Part:
                    productRecipe.Classification = RecipeClassification.Part;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return productRecipe;
        }

        public ProductInstanceModel ConvertProductInstance(ProductInstance instance)
        {
            var model = new ProductInstanceModel
            {
                Id = instance.Id,
                State = instance.State,
                Type = instance.Type.GetType().Name,
                Properties = EntryConvert.EncodeObject(instance)
            };
            return model;
        }

        public ProductInstance ConvertProductInstanceBack(ProductInstanceModel model, ProductType type)
        {
            var productInstance = type.CreateInstance();
            productInstance.Id = model.Id;
            productInstance.State = model.State;
            EntryConvert.UpdateInstance(productInstance, model.Properties);
            return productInstance;
        }

        public static WorkplanModel ConvertWorkplan(IWorkplan workplan)
        {
            return new WorkplanModel
            {
                Id = workplan.Id,
                Name = workplan.Name,
                Version = workplan.Version,
                State = workplan.State
            };
        }

        public static Workplan ConvertWorkplanBack(WorkplanModel model)
        {
            return new Workplan
            {
                Id = model.Id,
                Name = model.Name,
                Version = model.Version,
                State = model.State
            };
        }
    }
}
