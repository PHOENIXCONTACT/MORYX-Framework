// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Tools;
using Moryx.Workplans;

namespace Moryx.Products.Management
{
    internal class ProductManagementFacade : FacadeBase, IProductManagement
    {
        #region Dependencies

        public IProductManager ProductManager { get; set; }

        public IConfiguredTypesProvider TypesProvider { get; set; }

        public IRecipeManagement RecipeManagement { get; set; }

        public IWorkplans Workplans { get; set; }

        public IModuleLogger Logger { get; set; }

        #endregion

        public override void Activate()
        {
            base.Activate();

            ProductManager.TypeChanged += OnTypeChanged;
            RecipeManagement.RecipeChanged += OnRecipeChanged;
        }

        public override void Deactivate()
        {
            ProductManager.TypeChanged -= OnTypeChanged;
            RecipeManagement.RecipeChanged -= OnRecipeChanged;

            base.Deactivate();
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

        public Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query)
        {
            ValidateHealthState();
            return ProductManager.LoadTypes(query);
        }

        public Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector)
        {
            ValidateHealthState();
            return ProductManager.LoadTypes(selector);
        }

        public async Task<ProductType> LoadTypeAsync(long id)
        {
            ValidateHealthState();
            var type = await ProductManager.LoadType(id);
            if (type == null)
                throw new ProductNotFoundException(id);
            return type;
        }

        public Task<ProductType> LoadTypeAsync(IIdentity identity)
        {
            ValidateHealthState();
            return ProductManager.LoadType(identity);
        }

        private void OnTypeChanged(object sender, ProductType productType)
        {
            TypeChanged?.Invoke(this, productType);
        }
        public event EventHandler<ProductType> TypeChanged;

        public Task<ProductType> DuplicateAsync(ProductType template, IIdentity newIdentity)
        {
            ValidateHealthState();
            return ProductManager.Duplicate(template, newIdentity);
        }

        public Task<long> SaveTypeAsync(ProductType modifiedInstance)
        {
            ValidateHealthState();
            return ProductManager.SaveType(modifiedInstance);
        }

        public Task<ProductImportResult> ImportAsync(string importerName, object parameters)
        {
            ValidateHealthState();
            return ProductManager.Import(importerName, parameters);
        }

        public async Task<IRecipe> LoadRecipeAsync(long id)
        {
            ValidateHealthState();
            var recipe = await RecipeManagement.Get(id);

            ValidateRecipe(recipe);

            ReplaceOrigin(recipe);

            return recipe;
        }

        public async Task<IReadOnlyList<IProductRecipe>> GetRecipesAsync(ProductType productType, RecipeClassification classification)
        {
            ValidateHealthState();
            var recipes =await RecipeManagement.GetRecipes(productType, classification);
            if (recipes == null)
                return null;

            recipes.ForEach(ReplaceOrigin);

            return recipes;
        }

        private void OnRecipeChanged(object sender, IRecipe recipe)
        {
            ReplaceOrigin(recipe);
            RecipeChanged?.Invoke(this, recipe);
        }
        public event EventHandler<IRecipe> RecipeChanged;

        public async Task<long> SaveRecipeAsync(IProductRecipe recipe)
        {
            ValidateHealthState();
            var recipeId = await RecipeManagement.Save(recipe);
            ReplaceOrigin(recipe);

            return recipeId;
        }

        public Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes)
        {
            ValidateHealthState();
            return RecipeManagement.Save(recipes);
        }

        public async Task<Workplan> LoadWorkplanAsync(long workplanId)
        {
            ValidateHealthState();
            var wp = await Workplans.LoadWorkplanAsync(workplanId);
            if (wp == null)
                throw new KeyNotFoundException($"No workplan with id '{workplanId}' found!");
            return wp;
        }

        public Task<IReadOnlyList<Workplan>> LoadAllWorkplansAsync()
        {
            ValidateHealthState();
            return Workplans.LoadAllWorkplansAsync();
        }

        public Task<bool> DeleteWorkplanAsync(long workplanId)
        {
            ValidateHealthState();
            return Workplans.DeleteWorkplanAsync(workplanId);
        }

        public Task<IReadOnlyList<Workplan>> LoadVersionsAsync(long workplanId)
        {
            ValidateHealthState();
            return Workplans.LoadVersionsAsync(workplanId);
        }

        public Task<long> SaveWorkplanAsync(Workplan workplan)
        {
            ValidateHealthState();
            return Workplans.SaveWorkplanAsync(workplan);
        }

        public Task<ProductInstance> CreateInstanceAsync(ProductType productType)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(productType, false);
        }

        public Task<ProductInstance> CreateInstanceAsync(ProductType productType, bool save)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(productType, save);
        }

        public async Task<ProductInstance> GetInstanceAsync(long id)
        {
            ValidateHealthState();
            return (await ProductManager.GetInstances(id)).SingleOrDefault();
        }

        public async Task<ProductInstance> GetInstanceAsync(IIdentity identity)
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(identity);

            var instances = await ProductManager
                .GetInstances<IIdentifiableObject>(i => identity.Equals(i.Identity));
            if (instances.Count > 1)
            {
                var ex = new InvalidOperationException($"ProductManagement contains more than one {nameof(ProductInstance)} with the identity {identity}.");
                Logger.LogError(ex, "Please make sure that an identity is unique.");
                throw ex;
            }

            return (ProductInstance)instances.SingleOrDefault();
        }

        public async Task<TInstance> GetInstanceAsync<TInstance>(Expression<Func<TInstance, bool>> selector)
            where TInstance : ProductInstance
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(selector);

            return (await ProductManager.GetInstances(selector)).SingleOrDefault();
        }

        public Task SaveInstanceAsync(ProductInstance productInstance)
        {
            ValidateHealthState();
            return ProductManager.SaveInstances(productInstance);
        }

        public Task SaveInstancesAsync(ProductInstance[] productInstance)
        {
            ValidateHealthState();
            return ProductManager.SaveInstances(productInstance);
        }

        public Task<IReadOnlyList<ProductInstance>> GetInstancesAsync(long[] ids)
        {
            ValidateHealthState();

            ArgumentNullException.ThrowIfNull(ids);

            return ProductManager.GetInstances(ids);
        }

        public Task<IReadOnlyList<TInstance>> GetInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector)
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

        private void ReplaceOrigin<TRecipe>(TRecipe recipe)
            where TRecipe : IRecipe
        {
            recipe.Origin = this;
        }

        public Task<bool> DeleteProductAsync(long id)
        {
            ValidateHealthState();
            return ProductManager.DeleteType(id);
        }

        public Task RemoveRecipeAsync(long recipeId)
        {
            ValidateHealthState();
            return RecipeManagement.Remove(recipeId);
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
