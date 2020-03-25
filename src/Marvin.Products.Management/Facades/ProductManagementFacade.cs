// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Products;
using Marvin.AbstractionLayer.Recipes;
using Marvin.Runtime.Modules;
using Marvin.Workflows;

namespace Marvin.Products.Management
{
    internal class ProductManagementFacade : IFacadeControl, IProductManagement
    {
        // Use this delegate in every call for clean health state management
        public Action ValidateHealthState { get; set; }

        #region Dependencies

        public IProductManager ProductManager { get; set; }

        public IRecipeManagement RecipeManagement { get; set; }

        public IWorkplans Workplans { get; set; }

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

        public IReadOnlyList<IProductType> LoadTypes(ProductQuery query)
        {
            ValidateHealthState();
            return ProductManager.LoadTypes(query);
        }

        public IProductType LoadType(long id)
        {
            ValidateHealthState();
            return ProductManager.LoadType(id);
        }

        public IProductType LoadType(ProductIdentity identity)
        {
            ValidateHealthState();
            return ProductManager.LoadType(identity);
        }

        private void OnTypeChanged(object sender, IProductType productType)
        {
            TypeChanged?.Invoke(this, productType);
        }
        public event EventHandler<IProductType> TypeChanged;

        public IProductType Duplicate(IProductType template, ProductIdentity newIdentity)
        {
            ValidateHealthState();
            return ProductManager.Duplicate(template.Id, newIdentity);
        }

        public long SaveType(IProductType modifiedInstance)
        {
            ValidateHealthState();
            return ProductManager.SaveType(modifiedInstance);
        }


        public IDictionary<string, IImportParameters> Importers
        {
            get
            {
                ValidateHealthState();
                return ProductManager.Importers.ToDictionary(i => i.Name, i => i.Parameters);
            }
        }

        public IReadOnlyList<IProductType> ImportTypes(string importerName, IImportParameters parameters)
        {
            ValidateHealthState();
            return ProductManager.ImportTypes(importerName, parameters);
        }

        public IRecipe LoadRecipe(long id)
        {
            ValidateHealthState();
            var recipe = RecipeManagement.Get(id);

            ValidateRecipe(recipe);

            return ReplaceOrigin(recipe);
        }

        public IReadOnlyList<IProductRecipe> GetRecipes(IProductType productType, RecipeClassification classification)
        {
            ValidateHealthState();
            var recipes = RecipeManagement.GetRecipes(productType, classification);
            return recipes.Select(ReplaceOrigin).ToArray();
        }

        private void OnRecipeChanged(object sender, IRecipe recipe)
        {
            RecipeChanged?.Invoke(this, recipe);
        }
        public event EventHandler<IRecipe> RecipeChanged;

        public long SaveRecipe(IProductRecipe recipe)
        {
            ValidateHealthState();
            var recipeId = RecipeManagement.Save(recipe);

            return recipeId;
        }

        public Workplan LoadWorkplan(long workplanId)
        {
            ValidateHealthState();
            var wp = Workplans.LoadWorkplan(workplanId);
            if(wp == null)
                throw new KeyNotFoundException($"No workplan with id '{workplanId}' found!");
            return wp;
        }

        public IReadOnlyList<Workplan> LoadAllWorkplans()
        {
            ValidateHealthState();
            return Workplans.LoadAllWorkplans();
        }

        public void DeleteWorkplan(long workplanId)
        {
            ValidateHealthState();
            Workplans.DeleteWorkplan(workplanId);
        }

        public long SaveWorkplan(Workplan workplan)
        {
            ValidateHealthState();
            return Workplans.SaveWorkplan(workplan);
        }

        public ProductInstance CreateInstance(IProductType productType)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(productType, false);
        }

        public ProductInstance CreateInstance(IProductType productType, bool save)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(productType, save);
        }

        public ProductInstance GetInstance(long id)
        {
            ValidateHealthState();
            return ProductManager.GetInstance(id);
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

        public IEnumerable<ProductInstance> GetInstances(ProductInstanceState state)
        {
            ValidateHealthState();
            return ProductManager.GetInstances(state);
        }

        public IEnumerable<ProductInstance> GetInstances(int state)
        {
            ValidateHealthState();
            return ProductManager.GetInstances(state);
        }

        private static void ValidateRecipe(IProductRecipe recipe)
        {
            if (recipe == null)
                throw new ArgumentException("Recipe could not be found");
        }

        private IProductRecipe ReplaceOrigin(IProductRecipe recipe)
        {
            recipe.Origin = this;
            return recipe;
        }
    }
}
