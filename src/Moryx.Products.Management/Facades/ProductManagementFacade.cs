// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Workplans;

namespace Moryx.Products.Management
{
    internal class ProductManagementFacade : IFacadeControl, IProductManagement
    {
        // Use this delegate in every call for clean health state management
        public Action ValidateHealthState { get; set; }

        #region Dependencies

        public IProductManager ProductManager { get; set; }

        public IConfiguredTypesProvider TypesProvider { get; set; }

        public IRecipeManagement RecipeManagement { get; set; }

        public IWorkplans Workplans { get; set; }

        public ModuleConfig Config { get; set; }

        public IModuleLogger Logger { get; set; }

        #endregion

        public void Activate()
        {
            ProductManager.TypeChanged += OnTypeChanged;
            RecipeManagement.RecipeChanged += OnRecipeChanged;

        }

        public void Deactivate()
        {
            ProductManager.TypeChanged -= OnTypeChanged;
            RecipeManagement.RecipeChanged -= OnRecipeChanged;
        }

        public string Name => ModuleController.ModuleName;

        public IDictionary<string, object> Importers
        {
            get
            {
                ValidateHealthState();
                return ProductManager.Importers.ToDictionary(i => i.Name, i => i.Parameters);
            }
        }

        public IReadOnlyList<Type> ProductTypes
        {
            get
            {
                ValidateHealthState();
                return TypesProvider.ProductTypes;
            }
        }

        public IReadOnlyList<Type> RecipeTypes
        {
            get
            {
                ValidateHealthState();
                return TypesProvider.RecipeTypes;
            }
        }

        public IReadOnlyList<ProductType> LoadTypes(ProductQuery query)
        {
            ValidateHealthState();
            return ProductManager.LoadTypes(query);
        }

        public IReadOnlyList<TType> LoadTypes<TType>(Expression<Func<TType, bool>> selector)
        {
            ValidateHealthState();
            return ProductManager.LoadTypes(selector);
        }

        public ProductType LoadType(long id)
        {
            ValidateHealthState();
            var type = ProductManager.LoadType(id);
            if (type == null)
                throw new ProductNotFoundException(id);
            return type;
        }

        public ProductType LoadType(ProductIdentity identity)
        {
            ValidateHealthState();
            return ProductManager.LoadType(identity);
        }

        private void OnTypeChanged(object sender, ProductType productType)
        {
            TypeChanged?.Invoke(this, productType);
        }
        public event EventHandler<ProductType> TypeChanged;

        public ProductType Duplicate(ProductType template, ProductIdentity newIdentity)
        {
            ValidateHealthState();
            return ProductManager.Duplicate((ProductType)template, newIdentity);
        }

        public long SaveType(ProductType modifiedInstance)
        {
            ValidateHealthState();
            return ProductManager.SaveType(modifiedInstance);
        }

        public Task<ProductImportResult> Import(string importerName, object parameters)
        {
            ValidateHealthState();
            return ProductManager.Import(importerName, parameters);
        }

        public IRecipe LoadRecipe(long id)
        {
            ValidateHealthState();
            var recipe = RecipeManagement.Get(id);

            ValidateRecipe(recipe);

            return ReplaceOrigin(recipe);
        }

        public IReadOnlyList<IProductRecipe> GetRecipes(ProductType productType, RecipeClassification classification)
        {
            ValidateHealthState();
            var recipes = RecipeManagement.GetRecipes(productType, classification);
            if (recipes == null)
                return null;
            return recipes.Select(ReplaceOrigin).ToArray();
        }

        private void OnRecipeChanged(object sender, IRecipe recipe)
        {
            ReplaceOrigin(recipe);
            RecipeChanged?.Invoke(this, recipe);
        }
        public event EventHandler<IRecipe> RecipeChanged;

        public long SaveRecipe(IProductRecipe recipe)
        {
            ValidateHealthState();
            var recipeId = RecipeManagement.Save(recipe);
            ReplaceOrigin(recipe);

            return recipeId;
        }

        public Workplan LoadWorkplan(long workplanId)
        {
            ValidateHealthState();
            var wp = Workplans.LoadWorkplan(workplanId);
            if (wp == null)
                throw new KeyNotFoundException($"No workplan with id '{workplanId}' found!");
            return wp;
        }

        public IReadOnlyList<Workplan> LoadAllWorkplans()
        {
            ValidateHealthState();
            return Workplans.LoadAllWorkplans();
        }

        public bool DeleteWorkplan(long workplanId)
        {
            ValidateHealthState();
            return Workplans.DeleteWorkplan(workplanId);
        }

        public IReadOnlyList<Workplan> LoadVersions(long workplanId)
        {
            ValidateHealthState();
            return Workplans.LoadVersions(workplanId);
        }

        public long SaveWorkplan(Workplan workplan)
        {
            ValidateHealthState();
            return Workplans.SaveWorkplan(workplan);
        }

        public ProductInstance CreateInstance(ProductType productType)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(productType, false);
        }

        public ProductInstance CreateInstance(ProductType productType, bool save)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(productType, save);
        }

        public ProductInstance GetInstance(long id)
        {
            ValidateHealthState();
            return ProductManager.GetInstances(id).SingleOrDefault();
        }

        public ProductInstance GetInstance(IIdentity identity)
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(identity);

            var instances = ProductManager
                .GetInstances<IIdentifiableObject>(i => identity.Equals(i.Identity));
            if (instances.Count > 1)
            {
                var ex = new InvalidOperationException($"ProductManagement contains more than one {nameof(ProductInstance)} with the identity {identity}.");
                Logger.LogError(ex, "Please make sure that an identity is unique.");
                throw ex;
            }

            return (ProductInstance)instances.SingleOrDefault(); ;
        }

        public TInstance GetInstance<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : ProductInstance
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(selector);

            return ProductManager.GetInstances(selector).SingleOrDefault();
        }

        public void SaveInstance(ProductInstance productInstance)
        {
            ValidateHealthState();
            ProductManager.SaveInstances(productInstance);
        }

        public void SaveInstances(ProductInstance[] productInstance)
        {
            ValidateHealthState();
            ProductManager.SaveInstances(productInstance);
        }

        public IReadOnlyList<ProductInstance> GetInstances(long[] ids)
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(ids);

            return ProductManager.GetInstances(ids);
        }

        public IReadOnlyList<TInstance> GetInstances<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : ProductInstance
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(selector);

            return ProductManager.GetInstances(selector);
        }

        private static void ValidateRecipe(IProductRecipe recipe)
        {
            if (recipe == null)
                throw new ArgumentException("Recipe could not be found");
        }

        private TRecipe ReplaceOrigin<TRecipe>(TRecipe recipe) where TRecipe : IRecipe
        {
            recipe.Origin = this;
            return recipe;
        }

        public bool DeleteProduct(long id)
        {
            ValidateHealthState();
            return ProductManager.DeleteType(id);
        }

        public void RemoveRecipe(long recipeId)
        {
            ValidateHealthState();
            RecipeManagement.Remove(recipeId);
        }

        public IProductRecipe CreateRecipe(string recipeType)
        {
            ValidateHealthState();
            return RecipeManagement.CreateRecipe(recipeType);
        }

        public ProductTypeWrapper GetTypeWrapper(string typeName)
        {
            ValidateHealthState();
            return ProductManager.GetTypeWrapper(typeName);
        }
    }
}
