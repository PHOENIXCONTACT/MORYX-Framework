// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Tools;
using Moryx.Serialization;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Moryx.AbstractionLayer.Products.Endpoints.Models;
using Moryx.AbstractionLayer.Products.Endpoints.Properties;
using Moryx.AspNetCore;
using Moryx.Configuration;
using Moryx.Runtime.Modules;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IProductManagement"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/products/")]
    [Produces("application/json")]
    public class ProductManagementController : ControllerBase
    {
        private readonly IProductManagement _productManagement;
        private readonly ProductConverter _productConverter;
        public ProductManagementController(IProductManagement productManagement,
            IModuleManager moduleManager,
            IServiceProvider serviceProvider)
        {
            _productManagement = productManagement;

            var module = moduleManager.AllModules.FirstOrDefault(module => module is IFacadeContainer<IProductManagement>);
            _productConverter = new ProductConverter(_productManagement, module.Container, serviceProvider);
        }

        #region importers
        [HttpGet]
        [Route("configuration")]
        [Authorize(Policy = ProductPermissions.CanViewTypes)]
        public ActionResult<ProductCustomization> GetProductCustomization()
        {
            var parameterSerialization = new PossibleValuesSerialization(_productConverter.ProductManagerContainer, _productConverter.GlobalContainer,
                new ValueProviderExecutor(new ValueProviderExecutorSettings().AddDefaultValueProvider()));
            return new ProductCustomization
            {
                ProductTypes = GetProductTypes(),
                RecipeTypes = GetRecipeTypes(),
                Importers = _productManagement.Importers.Select(i => new ProductImporter
                {
                    Name = i.Key,
                    Parameters = EntryConvert.EncodeObject(i.Value, parameterSerialization)
                }).ToArray()
            };
        }

        private ProductDefinitionModel[] GetProductTypes()
        {
            var types = _productManagement.ProductTypes;
            var typeModels = new List<ProductDefinitionModel>();
            foreach (var type in types)
                typeModels.Add(_productConverter.ConvertProductType(type));
            return typeModels.ToArray();
        }
        private RecipeDefinitionModel[] GetRecipeTypes()
        {
            var recipeTypes = _productManagement.RecipeTypes;
            var typeModels = new List<RecipeDefinitionModel>();
            foreach (var recipeType in recipeTypes)
                typeModels.Add(ProductConverter.ConvertRecipeType(recipeType));
            return typeModels.ToArray();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("importers/{importerName}")]
        [Authorize(Policy = ProductPermissions.CanImport)]
        public async Task<ActionResult<ProductModel[]>> Import(string importerName, Entry importParameters)
        {
            if (importParameters == null)
                return BadRequest($"Import parameters were null");
            var parameters = ConvertParametersBack(importerName, importParameters);
            if (parameters == null)
            {
                return BadRequest($"Importer with the name {importerName} was not found or had no parameters");
            }
            var importedTypes = (await _productManagement.ImportAsync(importerName, parameters)).ImportedTypes;
            var modelList = new List<ProductModel>();
            foreach (var t in importedTypes)
                modelList.Add(await _productConverter.ConvertProduct(t, false));
            return modelList.ToArray();
        }

        private object ConvertParametersBack(string importerName, Entry currentParameters)
        {
            var oldParameters = _productManagement.Importers.FirstOrDefault(i => i.Key == importerName).Value;
            if (oldParameters == null)
                return null;

            var parameters = EntryConvert.UpdateInstance(oldParameters, currentParameters);
            return parameters;
        }

        #endregion
        #region product type
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("types")]
        [Authorize(Policy = ProductPermissions.CanEditType)]
        public async Task<ActionResult<long>> SaveType(ProductModel newTypeModel)
        {
            if (newTypeModel == null)
                return BadRequest($"Modified type was null");
            var type = ReflectionTool.GetPublicClasses<ProductType>(t => t.Name == newTypeModel.Type)
                   .FirstOrDefault();
            if (type == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.ProductManagementController_TypeNotFound });
            var productType = (ProductType)Activator.CreateInstance(type);
            var newType = await _productConverter.ConvertProductBack(newTypeModel, productType);
            return await _productManagement.SaveTypeAsync(newType);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("types")]
        [Authorize(Policy = ProductPermissions.CanViewTypes)]
        public async Task<ActionResult<ProductModel[]>> GetTypeByIdentity(string identity = null)
        {
            if (identity == null)
            {
                var products = (await _productManagement.LoadTypesAsync(new ProductQuery { Selector = Selector.Direct, ExcludeDerivedTypes = false }))
                   .ToList();
                var productModels = new List<ProductModel>();
                foreach (var p in products)
                    productModels.Add(await _productConverter.ConvertProduct(p, false));
                return productModels.ToArray();
            }

            var identityArray = WebUtility.HtmlEncode(identity).Split('-');
            if (identityArray.Length != 2)
                return BadRequest($"Identity has wrong format. Must be identifier-revision");
            var productIdentity = new ProductIdentity(identityArray[0], Convert.ToInt16(identityArray[1]));
            var productType = await _productManagement.LoadTypeAsync(productIdentity);
            if (productType == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.ProductManagementController_TypeNotFound });
            return new ProductModel[]
            {
                await _productConverter.ConvertProduct(productType, false)
            };
        }

        [HttpPost]
        [Route("types/query")]
        [Authorize(Policy = ProductPermissions.CanViewTypes)]
        public async Task<ActionResult<ProductModel[]>> GetTypes(ProductQuery query)
        {
            var productTypes = await _productManagement.LoadTypesAsync(query);
            var productModels = new List<ProductModel>();
            foreach (var t in productTypes)
            {
                productModels.Add(await _productConverter.ConvertProduct(t, false));
            }
            return productModels.ToArray();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("types/{id}")]
        [Authorize(Policy = ProductPermissions.CanViewTypes)]
        public async Task<ActionResult<ProductModel>> GetTypeById(long id)
        {
            if (id == 0)
                return BadRequest($"Id was 0");
            ProductType productType = null;
            try
            {
                productType = await _productManagement.LoadTypeAsync(id);
            }
            catch (ProductNotFoundException)
            {
            }
            if (productType == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.ProductManagementController_TypeNotFound });
            return await _productConverter.ConvertProduct(productType, false);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("types/{id}")]
        [Authorize(Policy = ProductPermissions.CanDeleteType)]
        public async Task<ActionResult<bool>> DeleteType(long id)
        {
            var result = await _productManagement.DeleteTypeAsync(id);
            if (!result)
                return NotFound(new MoryxExceptionResponse { Title = Strings.ProductManagementController_TypeNotFound });
            return result;
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("types/{id}")]
        [Authorize(Policy = ProductPermissions.CanEditType)]
        public async Task<ActionResult<long>> UpdateType(long id, ProductModel modifiedType)
        {
            if (modifiedType == null)
                return BadRequest($"Modified product type was null");
            var type = await _productManagement.LoadTypeAsync(id);
            if (type == null)
                return BadRequest($"No product type with id {modifiedType.Id} was found");
            type = await _productConverter.ConvertProductBack(modifiedType, (ProductType)type);
            return await _productManagement.SaveTypeAsync(type);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Route("types/{id}")]
        [Authorize(Policy = ProductPermissions.CanDuplicateType)]
        public async Task<ActionResult<ProductModel>> Duplicate(long id, [FromBody] string newIdentity)
        {
            var template = await _productManagement.LoadTypeAsync(id);
            if (template == null)
                return BadRequest($"Producttype with id {id} not found");
            var identityArray = WebUtility.HtmlEncode(newIdentity).Split('-');
            if (identityArray.Length != 2)
                return BadRequest($"Identity has wrong format. Must be identifier-revision");
            var identity = new ProductIdentity(identityArray[0], Convert.ToInt16(identityArray[1]));
            ProductType newProductType;
            try
            {
                newProductType = await _productManagement.DuplicateTypeAsync(template, identity);
            }
            catch (IdentityConflictException ex)
            {
                return Conflict(ex.Message);
            }
            if (newProductType == null)
                return BadRequest($"Error while duplicating");
            return await _productConverter.ConvertProduct(newProductType, false);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("types/{id}/recipes/{classification}")]
        [Authorize(Policy = ProductPermissions.CanViewTypes)]
        public async Task<ActionResult<RecipeModel[]>> GetRecipes(long id, int classification)
        {
            var productType = await _productManagement.LoadTypeAsync(id);
            if (productType == null)
                return BadRequest($"ProductType is null");
            var recipes = await _productManagement.LoadRecipesAsync(productType, (RecipeClassification)classification);
            var recipeModels = new List<RecipeModel>();
            foreach (var recipe in recipes)
                recipeModels.Add(_productConverter.ConvertRecipe(recipe));
            return recipeModels.ToArray();
        }
        #endregion

        #region product instances
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("instances/{id}")]
        [Authorize(Policy = ProductPermissions.CanViewInstances)]
        public async Task<ActionResult<ProductInstanceModel>> GetInstance(long id)
        {
            if (id == 0)
                return BadRequest($"Id was 0");
            var productInstance = await _productManagement.LoadInstanceAsync(id);
            if (productInstance == null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ProductManagementController_InstanceNotFound, id) });
            return _productConverter.ConvertProductInstance(productInstance);
        }

        [HttpGet]
        [Route("instances")]
        [Authorize(Policy = ProductPermissions.CanViewInstances)]
        public async Task<ActionResult<ProductInstanceModel[]>> GetInstances([FromQuery] long[] ids)
        {
            var instances = await _productManagement.LoadInstancesAsync(ids);
            var modelList = new List<ProductInstanceModel>();
            foreach (var instance in instances)
                modelList.Add(_productConverter.ConvertProductInstance(instance));
            return modelList.ToArray();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("instances")]
        [Authorize(Policy = ProductPermissions.CanCreateInstances)]
        public async Task<ActionResult<ProductInstanceModel>> CreateInstance(string identifier, short revision, bool save)
        {
            var identity = new ProductIdentity(WebUtility.HtmlEncode(identifier), revision);
            var productType = await _productManagement.LoadTypeAsync(identity);
            if (productType == null)
                return BadRequest($"Product type not found");
            var instance = await _productManagement.CreateInstanceAsync(productType, save);
            return _productConverter.ConvertProductInstance(instance);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("instances")]
        [Authorize(Policy = ProductPermissions.CanCreateInstances)]
        public async Task<ActionResult> SaveInstance(ProductInstanceModel instanceModel)
        {
            if (instanceModel == null)
                return BadRequest($"Instance model was empty");
            var type = ReflectionTool.GetPublicClasses<ProductType>(t => t.Name == instanceModel.Type)
                    .FirstOrDefault();
            if (type == null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ProductManagementController_InstanceNotFound, "null") });
            var productType = (ProductType)Activator.CreateInstance(type);
            var productInstance = _productConverter.ConvertProductInstanceBack(instanceModel, productType);
            await _productManagement.SaveInstanceAsync(productInstance);
            return Ok();
        }

        #endregion
        #region recipes
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("recipes/{id}")]
        [Authorize(Policy = ProductPermissions.CanViewTypes)]
        public async Task<ActionResult<RecipeModel>> GetRecipe(long id)
        {
            if (id == 0)
                return BadRequest($"Id was 0");
            var recipe = await _productManagement.LoadRecipeAsync(id);
            if (recipe == null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ProductManagementController_RecipeNotFound, id) });
            return _productConverter.ConvertRecipe(recipe);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("recipes")]
        [Authorize(Policy = ProductPermissions.CanCreateAndEditRecipes)]
        public async Task<ActionResult<long>> SaveRecipe(RecipeModel recipe)
        {
            if (recipe == null)
                return BadRequest($"Recipe was null");
            var type = ReflectionTool.GetPublicClasses<IProductRecipe>(t => t.Name == recipe.Type)
                    .FirstOrDefault();
            if (type == null)
                return NotFound(new MoryxExceptionResponse { Title = string.Format(Strings.ProductManagementController_RecipeNotFound, "null") });
            var productRecipe = (IProductRecipe)Activator.CreateInstance(type);
            return await _productManagement.SaveRecipeAsync(await _productConverter.ConvertRecipeBack(recipe, productRecipe, null));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("recipes/{id}")]
        [Authorize(Policy = ProductPermissions.CanCreateAndEditRecipes)]
        public async Task<ActionResult<long>> UpdateRecipe(long id, RecipeModel recipeModel)
        {
            if (recipeModel == null)
                return BadRequest($"Recipe was null");
            var recipe = await _productManagement.LoadRecipeAsync(id);
            if (recipe == null)
                return BadRequest($"Recipe with id {id} not found");
            var productRecipe = recipe as IProductRecipe;
            if (productRecipe == null)
                return BadRequest($"Recipe with id {id} wasn't a IProductRecipe but a {nameof(recipe.GetType)}");
            var productionRecipe = await _productConverter.ConvertRecipeBack(recipeModel, productRecipe, null);
            return await _productManagement.SaveRecipeAsync(productionRecipe);
        }

        [HttpGet("recipe/construct/{recipeType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = ProductPermissions.CanCreateAndEditRecipes)]
        public ActionResult<RecipeModel> CreateRecipe(string recipeType)
        {
            var recipe = _productManagement.CreateRecipe(recipeType);
            if (recipe == null)
                return BadRequest($"Recipe of type {recipeType} could not created");

            return _productConverter.ConvertRecipe(recipe);
        }
        #endregion
    }
}
