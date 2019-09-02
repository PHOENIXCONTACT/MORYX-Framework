using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.AbstractionLayer;
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
            ProductManager.ProductChanged += OnProductChanged;
            RecipeManagement.RecipeChanged += OnRecipeChanged;
        }

        public void Deactivate()
        {
            ProductManager.ProductChanged -= OnProductChanged;
            RecipeManagement.RecipeChanged -= OnRecipeChanged;
        }

        public string Name => ModuleController.ModuleName;

        public IReadOnlyList<IProduct> GetProducts(ProductQuery query)
        {
            ValidateHealthState();
            return ProductManager.GetProducts(query);
        }

        public IProduct GetProduct(long id)
        {
            ValidateHealthState();
            return ProductManager.GetProduct(id);
        }

        public IProduct GetProduct(ProductIdentity identity)
        {
            ValidateHealthState();
            return ProductManager.GetProduct(identity);
        }

        private void OnProductChanged(object sender, IProduct product)
        {
            ProductChanged?.Invoke(this, product);
        }
        public event EventHandler<IProduct> ProductChanged;

        public IProduct Duplicate(IProduct template, ProductIdentity newIdentity)
        {
            ValidateHealthState();
            return ProductManager.Duplicate(template.Id, newIdentity);
        }

        public long SaveProduct(IProduct modifiedInstance)
        {
            ValidateHealthState();
            return ProductManager.Save(modifiedInstance);
        }


        public IDictionary<string, IImportParameters> Importers
        {
            get
            {
                ValidateHealthState();
                return ProductManager.Importers.ToDictionary(i => i.Name, i => i.Parameters);
            }
        }

        public IReadOnlyList<IProduct> ImportProducts(string importerName, IImportParameters parameters)
        {
            ValidateHealthState();
            return ProductManager.ImportProducts(importerName, parameters);
        }

        public IRecipe LoadRecipe(long id)
        {
            ValidateHealthState();
            var recipe = RecipeManagement.Get(id);

            ValidateRecipe(recipe);

            return ReplaceOrigin(recipe);
        }

        public IReadOnlyList<IProductRecipe> GetRecipes(IProduct product, RecipeClassification classification)
        {
            ValidateHealthState();
            var recipes = RecipeManagement.GetRecipes(product, classification);
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

        public Article CreateInstance(IProduct product)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(product, false);
        }

        public Article CreateInstance(IProduct product, bool save)
        {
            ValidateHealthState();
            return ProductManager.CreateInstance(product, save);
        }

        public Article GetArticle(long id)
        {
            ValidateHealthState();
            return ProductManager.GetArticle(id);
        }

        public void SaveArticle(Article article)
        {
            ValidateHealthState();
            ProductManager.SaveArticles(article);
        }

        public void SaveArticles(Article[] article)
        {
            ValidateHealthState();
            ProductManager.SaveArticles(article);
        }

        public IEnumerable<Article> GetArticles(ArticleState state)
        {
            ValidateHealthState();
            return ProductManager.GetArticles(state);
        }

        public IEnumerable<Article> GetArticles(int state)
        {
            ValidateHealthState();
            return ProductManager.GetArticles(state);
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