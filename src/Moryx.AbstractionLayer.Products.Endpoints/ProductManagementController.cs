using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Recipes;
using System;
using System.Collections.Generic;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="IProductManagementModification"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/products/")]

    public class ProductManagementController: ControllerBase
    {
        private readonly IProductManagementModification _productManagement;
        public ProductManagementController(IProductManagementModification productManagement)
            => _productManagement = productManagement;

        #region Get-Requests
        [HttpGet]
        [Route("[action]")]
        public string GetName()
        {
            return _productManagement.Name;
        }

        [HttpGet]
        [Route("[action]")]
        public IReadOnlyList<IProductType> GetTypes (ProductQuery query)
        {         
            return _productManagement.LoadTypes(query);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<IProductType> GetType(long id)
        {
            if (id == 0)
                return BadRequest($"Id was 0");
            var productType = _productManagement.LoadType(id);
            if(productType == null)
                return NotFound();
            return Ok(productType);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<IProductType> GetType(ProductIdentity identity)
        {
            if (identity == null)
                return BadRequest($"Identity was null");
            var productType = _productManagement.LoadType(identity);
            if (productType == null)
                return NotFound();
            return Ok(productType);
        }

        [HttpGet]
        [Route("[action]")]
        public IDictionary<string, object> GetImporters()
        {
            return _productManagement.Importers;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("[action]")]
        public ActionResult<IReadOnlyList<IProductRecipe>> GetRecipes(IProductType productType, 
            RecipeClassification classification)
        {
            if (productType == null)
                return BadRequest($"ProductType is null");
            return Ok(_productManagement.GetRecipes(productType, classification));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<ProductInstance> GetInstance (long id)
        {
            if (id == 0)
                return BadRequest($"Id was 0");
            var productInstance = _productManagement.GetInstance(id);
            if (productInstance == null)
                return NotFound();
            return Ok(productInstance);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<ProductInstance> GetInstance (IIdentity identity)
        {
            if (identity == null)
                return BadRequest($"Identity was null");
            var productInstance = _productManagement.GetInstance(identity);
            if (productInstance == null)
                return NotFound();
            return Ok(productInstance);
        }

        [HttpGet]
        [Route("[action]")]
        public IReadOnlyList<ProductInstance> GetInstances(long [] ids)
        {
            return _productManagement.GetInstances(ids);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("[action]")]
        public ActionResult<IRecipe> GetRecipe(long id)
        {
            if (id == 0)
                return BadRequest($"Id was 0");
            var recipe = _productManagement.LoadRecipe(id);
            if (recipe == null)
                return NotFound();
            return Ok(recipe);
        }

        [HttpGet]
        [Route("[action]")]
        public IReadOnlyList<Type> GetProductTypes()
        {
            return _productManagement.ProductTypes;
        }

        [HttpGet]
        [Route("[action]")]
        public IReadOnlyList<Type> GetRecipeTypes()
        {
            return _productManagement.RecipeTypes;
        }
        #endregion

        #region Post-Requests

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("[action]")]
        public ActionResult<IProductType> Duplicate(IProductType template, string newIdentity)
        {
            if (template == null)
                return BadRequest($"Template was null");
            if (newIdentity == null)
                return BadRequest($"New Identity was null");
            var identityArray = newIdentity.Split('-'); 
            var identity = new ProductIdentity(identityArray[0], Convert.ToInt16(identityArray[1]));
            return Ok(_productManagement.Duplicate(template, identity));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("[action]")]
        public ActionResult<long> SaveType(IProductType modifiedInstance)
        {
            if(modifiedInstance == null)
                return BadRequest($"Modified instance was null");
            return Ok(_productManagement.SaveType(modifiedInstance));
        }

        [HttpPost]
        [Route("[action]")]
        public ProductImportResult Import(string importerName, object parameters)
        {
            return _productManagement.Import(importerName, parameters).Result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("[action]")]
        public ActionResult<long> SaveRecipe(IProductRecipe recipe)
        {
            if(recipe == null)
                return BadRequest($"Recipe was null");
            return _productManagement.SaveRecipe(recipe);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("[action]")]
        public ActionResult<ProductInstance> CreateInstance(IProductType productType)
        {
            if (productType == null)
                return BadRequest($"Product type was null");
            return _productManagement.CreateInstance(productType);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("[action]")]
        public ActionResult<ProductInstance> CreateInstance(IProductType productType, bool save)
        {
            if (productType == null)
                return BadRequest($"Product type was null");
            return _productManagement.CreateInstance(productType,save);
        }

        [HttpPost]
        [Route("[action]")]
        public void SaveInstance(ProductInstance productInstance)
        {
            _productManagement.SaveInstance(productInstance);
        }

        [HttpPost]
        [Route("[action]")]
        public void SaveInstances(ProductInstance[] productInstances)
        {
            _productManagement.SaveInstances(productInstances);
        }

        [HttpPost]
        [Route("[action]")]
        public ActionResult<bool> DeleteProduct(long id)
        {
            return _productManagement.DeleteProduct(id);
        }

        #endregion
    }
}
