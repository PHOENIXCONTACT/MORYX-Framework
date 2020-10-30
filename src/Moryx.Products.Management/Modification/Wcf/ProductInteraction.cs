// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Web;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Workflows;

namespace Moryx.Products.Management.Modification
{
    [Plugin(LifeCycle.Singleton, typeof(IProductInteraction))]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    internal class ProductInteraction : IProductInteraction, ILoggingComponent
    {
        #region Dependencies

        public ModuleConfig Config { get; set; }

        public IProductManager ProductManager { get; set; }

        public IRecipeManagement RecipeManagement { get; set; }

        public IWorkplans WorkplanManagement { get; set; }

        public IProductConverter Converter { get; set; }

        public IModuleLogger Logger { get; set; }

        #endregion

        public ProductCustomization GetCustomization()
        {
            return ExecuteCall<ProductCustomization>(delegate
            {
                return new ProductCustomization
                {
                    ProductTypes = ReflectionTool
                        .GetPublicClasses<ProductType>(new IsConfiguredFilter(Config.TypeStrategies).IsConfigured)
                        .Select(pt => new ProductDefinitionModel
                        {
                            Name = pt.Name,
                            DisplayName = pt.GetDisplayName() ?? pt.Name,
                            BaseDefinition = pt.BaseType?.Name
                        }).ToArray(),
                    RecipeTypes = ReflectionTool
                        .GetPublicClasses<IProductRecipe>(new IsConfiguredFilter(Config.RecipeStrategies).IsConfigured)
                        .Select(rt => new RecipeDefinitionModel
                        {
                            Name = rt.Name,
                            DisplayName = rt.GetDisplayName() ?? rt.Name,
                            HasWorkplans = typeof(IWorkplanRecipe).IsAssignableFrom(rt)
                        }).ToArray(),
                    Importers = ProductManager.Importers.Select(i => new ProductImporter
                    {
                        Name = i.Name,
                        Parameters = ConvertParameters(i.Parameters)
                    }).ToArray()
                };
            });
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
            EntryConvert.EncodeObject(parametersObject, EntrySerializeSerialization.Instance);

        public Entry UpdateParameters(string importer, Entry importParameters)
        {
            return ExecuteCall(delegate
            {
                var parameters = ConvertParametersBack(importer, importParameters, true);
                if (parameters == null)
                    return RequestResult<Entry>.NotFound($"Importer '{importer}' not found!");
                return ConvertParameters(parameters);
            });
        }

        private IImportParameters ConvertParametersBack(string importerName, Entry currentParameters, bool updateFirst = false)
        {
            var importer = ProductManager.Importers.FirstOrDefault(i => i.Name == importerName);
            if (importer == null)
                return null;

            var parameters = (IImportParameters)EntryConvert.UpdateInstance(importer.Parameters, currentParameters);
            if (updateFirst)
                parameters = importer.Update(parameters);

            return parameters;
        }

        public ProductModel[] GetProducts(ProductQuery query)
        {
            return ExecuteCall<ProductModel[]>(delegate
            {
                var products = ProductManager.LoadTypes(query);
                return products.Select(p => Converter.ConvertProduct(p, true)).ToArray();
            });
        }

        public ProductModel CreateProduct(string type)
        {
            return ExecuteCall(delegate
            {
                var product = ProductManager.CreateType(type);
                if (product == null)
                    return RequestResult<ProductModel>.NotFound($"Product type '{type}' not found!");
                return Converter.ConvertProduct(product, true);
            });
        }

        public ProductModel GetProductDetails(string idString)
        {
            return ExecuteCall(delegate
            {
                IProductType product;
                if (long.TryParse(idString, out var id) && (product = ProductManager.LoadType(id)) != null)
                    return Converter.ConvertProduct(product, false);
                return RequestResult<ProductModel>.NotFound($"Product type '{idString}' not found!");
            });
        }

        public ProductModel SaveProduct(string idString, ProductModel instance)
        {
            return ExecuteCall(delegate
            {
                IProductType product;
                if (!long.TryParse(idString, out var id) || (product = ProductManager.LoadType(id)) == null)
                    return RequestResult<ProductModel>.NotFound($"Product type '{idString}' not found!");

                Converter.ConvertProductBack(instance, (ProductType)product);
                ProductManager.SaveType(product);
                return Converter.ConvertProduct(product, false);
            });
        }

        public DuplicateProductResponse DuplicateProduct(string idString, ProductModel productModel)
        {
            return ExecuteCall(delegate
            {
                var response = new DuplicateProductResponse();
                try
                {
                    IProductType product;
                    if (!long.TryParse(idString, out var id) || (product = ProductManager.LoadType(id)) == null)
                        return RequestResult<DuplicateProductResponse>.NotFound($"Source product type '{idString}' not found!");

                    var duplicate = ProductManager.Duplicate((ProductType)product, new ProductIdentity(productModel.Identifier, productModel.Revision));
                    response.Duplicate = Converter.ConvertProduct(duplicate, false);
                }
                catch (IdentityConflictException e)
                {
                    response.IdentityConflict = true;
                    response.InvalidSource = e.InvalidTemplate;
                }

                return response;
            });
        }

        public ProductModel ImportProduct(string importer, Entry importParameters)
        {
            return ExecuteCall(delegate
            {
                var parameters = ConvertParametersBack(importer, importParameters, true);
                if (parameters == null)
                    return RequestResult<ProductModel>.NotFound($"Importer '{importer}' not found!");

                var products = ProductManager.ImportTypes(importer, parameters);
                return Converter.ConvertProduct(products[0], false);
            });
        }

        public bool DeleteProduct(string idString)
        {
            return ExecuteCall<bool>(delegate
            {
                var id = long.Parse(idString);
                return ProductManager.DeleteType(id);
            });
        }

        public string GetRecipeProviderName()
        {
            return ModuleController.ModuleName;
        }

        public RecipeModel GetRecipe(string idString)
        {
            return ExecuteCall(delegate
            {
                IProductRecipe product;
                if (long.TryParse(idString, out var id) && (product = RecipeManagement.Get(id)) != null)
                    return Converter.ConvertRecipe(product);
                return RequestResult<RecipeModel>.NotFound($"Recipe '{idString}' not found!");
            });
        }

        public RecipeModel[] GetRecipes(string idString)
        {
            
            return ExecuteCall(delegate
            {
                IProductType product;
                if (long.TryParse(idString, out var id) && (product = ProductManager.LoadType(id)) != null)
                    return RecipeManagement.GetAllByProduct(product).Select(Converter.ConvertRecipe).ToArray();
                return RequestResult<RecipeModel[]>.NotFound($"Product type '{idString}' not found!");
            });
        }

        public RecipeModel CreateRecipe(string recipeType)
        {
            return ExecuteCall(delegate
            {
                // TODO: Use type wrapper
                var type = ReflectionTool.GetPublicClasses<IProductRecipe>(t => t.Name == recipeType).FirstOrDefault();
                if (type == null)
                    return RequestResult<RecipeModel>.NotFound($"Recipe type {recipeType} not found!");
                var recipe = (IProductRecipe) Activator.CreateInstance(type);
                return Converter.ConvertRecipe(recipe);
            });
        }

        public RecipeModel SaveRecipe(RecipeModel recipeModel)
        {
            return ExecuteCall(delegate
            {
                var type = ReflectionTool.GetPublicClasses<IProductRecipe>(t => t.Name == recipeModel.Type)
                    .FirstOrDefault();
                if (type == null)
                    return RequestResult<RecipeModel>.NotFound($"Recipe type {recipeModel.Type} not found!");
                var productRecipe = (IProductRecipe) Activator.CreateInstance(type);

                var productionRecipe = Converter.ConvertRecipeBack(recipeModel, productRecipe, null);
                var savedId = RecipeManagement.Save(productionRecipe);
                recipeModel.Id = savedId;

                return recipeModel;
            });
        }

        public RecipeModel UpdateRecipe(string idString, RecipeModel recipeModel)
        {
            return ExecuteCall(delegate
            {
                IProductRecipe productRecipe;
                if (!long.TryParse(idString, out var id) || (productRecipe = RecipeManagement.Get(id)) == null)
                    return RequestResult<RecipeModel>.NotFound($"Recipe {idString} not found!");

                var productionRecipe = Converter.ConvertRecipeBack(recipeModel, productRecipe, null);
                var savedId = RecipeManagement.Save(productionRecipe);
                recipeModel.Id = savedId;

                return recipeModel;
            });
        }

        public WorkplanModel[] GetWorkplans()
        {
            return ExecuteCall<WorkplanModel[]>(delegate
            {
                var workplans = WorkplanManagement.LoadAllWorkplans();
                return workplans.Select(Converter.ConvertWorkplan).ToArray();
            });
        }

        // TODO: Duplicate between resource and product service
        private T ExecuteCall<T>(Func<RequestResult<T>> request, [CallerMemberName]string method = "Unknown")
        {
            try
            {
                var result = request();
                if (result.AlternativeStatusCode.HasValue)
                {
                    Logger.Log(LogLevel.Error, result.ErrorLog);
                    WebOperationContext.Current.OutgoingResponse.StatusCode = result.AlternativeStatusCode.Value;
                    return default;
                }
                return result.Response;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Error, ex, "Exception during '{0}'", method);
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                return default;
            }
        }

        private class RequestResult<T>
        {
            public T Response { get; set; }

            public string ErrorLog { get; set; }

            public HttpStatusCode? AlternativeStatusCode { get; set; }

            public static implicit operator RequestResult<T>(T response)
            {
                return new RequestResult<T> { Response = response };
            }

            public static RequestResult<T> NotFound(string msg)
            {
                return new RequestResult<T>
                {
                    ErrorLog = msg,
                    AlternativeStatusCode = HttpStatusCode.NotFound
                };
            }
        }
    }
}
