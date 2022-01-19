// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
#if USE_WCF
using System.ServiceModel;
using System.ServiceModel.Web;
#else
using Microsoft.AspNetCore.Mvc;
using Moryx.Communication.Endpoints;
#endif
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Workflows;

namespace Moryx.Products.Management.Modification
{
    [Plugin(LifeCycle.Transient, typeof(IProductInteraction))]
#if USE_WCF
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    internal class ProductInteraction : IProductInteraction, ILoggingComponent
#else
    [ApiController, Route(Endpoint), Produces("application/json")]
    [Endpoint(Name = nameof(IProductInteraction), Version = "5.1.0")]
    internal class ProductInteraction : Controller, IProductInteraction, ILoggingComponent
#endif
    {
        internal const string Endpoint = "products";
        #region Dependencies

        public ModuleConfig Config { get; set; }

        public IProductManager ProductManager { get; set; }

        public IRecipeManagement RecipeManagement { get; set; }

        public IWorkplans WorkplanManagement { get; set; }

        public IProductConverter Converter { get; set; }

        public IModuleLogger Logger { get; set; }

        #endregion

#if !USE_WCF
        [HttpGet("customization")]
#endif
        public ProductCustomization GetCustomization()
        {
            return ExecuteCall<ProductCustomization>(delegate
            {
                return new ProductCustomization
                {
                    ProductTypes = ReflectionTool
                        .GetPublicClasses<ProductType>(new IsConfiguredFilter(Config.TypeStrategies).IsConfigured)
                        .Select(Converter.ConvertProductType).ToArray(),
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

        private static Entry ConvertParameters(object parametersObject) =>
            EntryConvert.EncodeObject(parametersObject, new PossibleValuesSerialization(null, new ValueProviderExecutor(new ValueProviderExecutorSettings().AddDefaultValueProvider())));

#if !USE_WCF
        [HttpPut("import/{importer}/parameters")]
#endif
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

        private object ConvertParametersBack(string importerName, Entry currentParameters, bool updateFirst = false)
        {
            var importer = ProductManager.Importers.FirstOrDefault(i => i.Name == importerName);
            if (importer == null)
                return null;

            var parameters = EntryConvert.UpdateInstance(importer.Parameters, currentParameters);
            if (updateFirst)
                parameters = importer.Update(parameters);

            return parameters;
        }

#if !USE_WCF
        [HttpPost("query")]
#endif
        public ProductModel[] GetProducts(ProductQuery query)
        {
            return ExecuteCall<ProductModel[]>(delegate
            {
                var products = ProductManager.LoadTypes(query);
                return products.Select(p => Converter.ConvertProduct(p, true)).ToArray();
            });
        }

#if !USE_WCF
        [HttpPost("construct/{type}")]
#endif
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

#if !USE_WCF
        [HttpGet("product/{idString}")]
#endif
        public ProductModel GetProductDetails(string idString)
        {
            return ExecuteCall(delegate
            {
                IProductType product;
                if (long.TryParse(idString, out var id) && (product = ProductManager.LoadType(id)) != null)
                    return Converter.ConvertProduct(product, false);
                return RequestResult<ProductModel>.NotFound($"Product type '{id}' not found!");
            });
        }

#if !USE_WCF
        [HttpPut("product/{idString}")]
#endif
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

#if !USE_WCF
        [HttpPost("product/{idString}/duplicate")]
#endif
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

#if !USE_WCF
        [HttpPost("import/{importer}")]
#endif
        public ImportStateModel Import(string importer, Entry importParameters)
        {
            return ExecuteCall(delegate
            {
                var parameters = ConvertParametersBack(importer, importParameters, true);
                if (parameters == null)
                    return RequestResult<ImportStateModel>.NotFound($"Importer '{importer}' not found!");

                var state = ProductManager.ImportParallel(importer, parameters);
                return new ImportStateModel(state);
            });
        }

#if !USE_WCF
        [HttpGet("import/session/{guid}")]
#endif
        public ImportStateModel FetchImportProgress(string guid)
        {
            return ExecuteCall(delegate
            {
                if (!Guid.TryParse(guid, out var session))
                    return RequestResult<ImportStateModel>.NotFound($"Guid '{guid}' invalid!");

                var state = ProductManager.ImportProgress(session);
                if (state == null)
                    return RequestResult<ImportStateModel>.NotFound($"Session '{guid}' not found!");

                return new ImportStateModel(state);
            });
        }

#if !USE_WCF
        [HttpDelete("product/{idString}")]
#endif
        public bool DeleteProduct(string idString)
        {
            return ExecuteCall<bool>(delegate
            {
                var id = long.Parse(idString);
                return ProductManager.DeleteType(id);
            });
        }

#if !USE_WCF
        [HttpGet("recipe/provider")]
#endif
        public string GetRecipeProviderName()
        {
            return ModuleController.ModuleName;
        }

#if !USE_WCF
        [HttpGet("recipe/{idString}")]
#endif
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

#if !USE_WCF
        [HttpGet("recipes")] // Product parameter resolved by model binding
#endif
        public RecipeModel[] GetRecipes(string product)
        {
            return ExecuteCall(delegate
            {
                IProductType productType;
                if (long.TryParse(product, out var id) && (productType = ProductManager.LoadType(id)) != null)
                    return RecipeManagement.GetAllByProduct(productType).Select(Converter.ConvertRecipe).ToArray();
                return RequestResult<RecipeModel[]>.NotFound($"Product type '{product}' not found!");
            });
        }

#if !USE_WCF
        [HttpPost("recipe/construct/{recipeType}")]
#endif
        public RecipeModel CreateRecipe(string recipeType)
        {
            return ExecuteCall(delegate
            {
                // TODO: Use type wrapper
                var type = ReflectionTool.GetPublicClasses<IProductRecipe>(t => t.Name == recipeType).FirstOrDefault();
                if (type == null)
                    return RequestResult<RecipeModel>.NotFound($"Recipe type {recipeType} not found!");
                var recipe = (IProductRecipe)Activator.CreateInstance(type);
                return Converter.ConvertRecipe(recipe);
            });
        }

#if !USE_WCF
        [HttpPost("recipe")]
#endif
        public RecipeModel SaveRecipe(RecipeModel recipeModel)
        {
            return ExecuteCall(delegate
            {
                var type = ReflectionTool.GetPublicClasses<IProductRecipe>(t => t.Name == recipeModel.Type)
                    .FirstOrDefault();
                if (type == null)
                    return RequestResult<RecipeModel>.NotFound($"Recipe type {recipeModel.Type} not found!");
                var productRecipe = (IProductRecipe)Activator.CreateInstance(type);

                var productionRecipe = Converter.ConvertRecipeBack(recipeModel, productRecipe, null);
                var savedId = RecipeManagement.Save(productionRecipe);
                recipeModel.Id = savedId;

                return recipeModel;
            });
        }

#if !USE_WCF
        [HttpPut("recipe/{idString}")]
#endif
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

#if !USE_WCF
        [HttpGet("workplans")]
#endif
        public WorkplanModel[] GetWorkplans()
        {
            return ExecuteCall<WorkplanModel[]>(delegate
            {
                var workplans = WorkplanManagement.LoadAllWorkplans();
                return workplans.Select(Converter.ConvertWorkplan).ToArray();
            });
        }

        // TODO: Duplicate between resource and product service
        private T ExecuteCall<T>(Func<RequestResult<T>> request, [CallerMemberName] string method = "Unknown")
        {
            try
            {
                var result = request();
                if (result.AlternativeStatusCode.HasValue)
                {
                    Logger.Log(LogLevel.Error, result.ErrorLog);
#if USE_WCF
                    WebOperationContext.Current.OutgoingResponse.StatusCode = result.AlternativeStatusCode.Value;
#else
                    Response.StatusCode = (int)result.AlternativeStatusCode.Value;
#endif
                    return default;
                }
                return result.Response;
            }
            catch (Exception ex)
            {
                Logger.LogException(LogLevel.Error, ex, "Exception during '{0}'", method);
#if USE_WCF
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
#else
                Response.StatusCode = 500;
#endif
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
