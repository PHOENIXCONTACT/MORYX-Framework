// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
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

namespace Moryx.Products.Management;

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

    public Task<IReadOnlyList<ProductType>> LoadTypesAsync(ProductQuery query, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.LoadTypes(query);
    }

    public Task<IReadOnlyList<TType>> LoadTypesAsync<TType>(Expression<Func<TType, bool>> selector, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.LoadTypes(selector);
    }

    public async Task<ProductType> LoadTypeAsync(long id, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        var type = await ProductManager.LoadType(id);
        if (type == null)
            throw new ProductNotFoundException(id);
        return type;
    }

    public Task<ProductType> LoadTypeAsync(IIdentity identity, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.LoadType(identity);
    }

    private void OnTypeChanged(object sender, ProductType productType)
    {
        TypeChanged?.Invoke(this, productType);
    }
    public event EventHandler<ProductType> TypeChanged;

    public Task<ProductType> DuplicateTypeAsync(ProductType template, IIdentity newIdentity, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.DuplicateType(template, newIdentity);
    }

    public Task<long> SaveTypeAsync(ProductType modifiedInstance, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.SaveType(modifiedInstance);
    }

    public Task<ProductImportResult> ImportAsync(string importerName, object parameters, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.Import(importerName, parameters);
    }

    public async Task<IRecipe> LoadRecipeAsync(long id, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        var recipe = await RecipeManagement.Get(id);

        ValidateRecipe(recipe);

        ReplaceOrigin(recipe);

        return recipe;
    }

    public async Task<IReadOnlyList<IProductRecipe>> LoadRecipesAsync(ProductType productType, RecipeClassification classification,
        CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        var recipes = await RecipeManagement.LoadRecipes(productType, classification);
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

    public async Task<long> SaveRecipeAsync(IProductRecipe recipe, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        var recipeId = await RecipeManagement.Save(recipe);
        ReplaceOrigin(recipe);

        return recipeId;
    }

    public Task SaveRecipesAsync(IReadOnlyList<IProductRecipe> recipes, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return RecipeManagement.Save(recipes);
    }

    public async Task<Workplan> LoadWorkplanAsync(long workplanId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        var wp = await Workplans.LoadWorkplanAsync(workplanId, cancellationToken);
        if (wp == null)
            throw new KeyNotFoundException($"No workplan with id '{workplanId}' found!");
        return wp;
    }

    public Task<IReadOnlyList<Workplan>> LoadAllWorkplansAsync(CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return Workplans.LoadAllWorkplansAsync(cancellationToken);
    }

    public Task<bool> DeleteWorkplanAsync(long workplanId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return Workplans.DeleteWorkplanAsync(workplanId, cancellationToken);
    }

    public Task<IReadOnlyList<Workplan>> LoadVersionsAsync(long workplanId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return Workplans.LoadVersionsAsync(workplanId, cancellationToken);
    }

    public Task<long> SaveWorkplanAsync(Workplan workplan, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return Workplans.SaveWorkplanAsync(workplan, cancellationToken);
    }

    public Task<ProductInstance> CreateInstanceAsync(ProductType productType, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.CreateInstance(productType, false);
    }

    public Task<ProductInstance> CreateInstanceAsync(ProductType productType, bool save, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.CreateInstance(productType, save);
    }

    public async Task<ProductInstance> LoadInstanceAsync(long id, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return (await ProductManager.LoadInstances([id])).SingleOrDefault();
    }

    public async Task<ProductInstance> LoadInstanceAsync(IIdentity identity, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();

        ArgumentNullException.ThrowIfNull(identity);

        var instances = await ProductManager
            .LoadInstances<IIdentifiableObject>(i => identity.Equals(i.Identity));
        if (instances.Count > 1)
        {
            var ex = new InvalidOperationException($"ProductManagement contains more than one {nameof(ProductInstance)} with the identity {identity}.");
            Logger.LogError(ex, "Please make sure that an identity is unique.");
            throw ex;
        }

        return (ProductInstance)instances.SingleOrDefault();
    }

    public async Task<TInstance> LoadInstanceAsync<TInstance>(Expression<Func<TInstance, bool>> selector, CancellationToken cancellationToken = default)
        where TInstance : ProductInstance
    {
        ValidateHealthState();

        ArgumentNullException.ThrowIfNull(selector);

        return (await ProductManager.LoadInstances(selector)).SingleOrDefault();
    }

    public Task SaveInstanceAsync(ProductInstance productInstance, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.SaveInstances(productInstance);
    }

    public Task SaveInstancesAsync(ProductInstance[] productInstance, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.SaveInstances(productInstance);
    }

    public Task<IReadOnlyList<ProductInstance>> LoadInstancesAsync(long[] ids, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();

        ArgumentNullException.ThrowIfNull(ids);

        return ProductManager.LoadInstances(ids);
    }

    public Task<IReadOnlyList<TInstance>> LoadInstancesAsync<TInstance>(Expression<Func<TInstance, bool>> selector,
        CancellationToken cancellationToken = default)
        where TInstance : ProductInstance
    {
        ValidateHealthState();

        ArgumentNullException.ThrowIfNull(selector);

        return ProductManager.LoadInstances(selector);
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

    public Task<bool> DeleteTypeAsync(long productTypeId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return ProductManager.DeleteType(productTypeId);
    }

    public Task DeleteRecipeAsync(long recipeId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return RecipeManagement.Delete(recipeId);
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