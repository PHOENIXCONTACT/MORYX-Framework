// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Products.Management.Model;
using Moryx.Workplans;

namespace Moryx.Products.Management
{
    [Component(LifeCycle.Singleton, typeof(IRecipeManagement), typeof(IWorkplans))]
    internal class RecipeManagement : IRecipeManagement, IWorkplans
    {
        #region Dependencies

        public IProductStorage Storage { get; set; }

        public IUnitOfWorkFactory<ProductsContext> ModelFactory { get; set; }

        #endregion

        public Task<IProductRecipe> Get(long recipeId)
        {
            var recipe = Storage.LoadRecipeAsync(recipeId);
            if (recipe == null)
                throw new RecipeNotFoundException(recipeId);

            return recipe;
        }

        public Task<IReadOnlyList<IProductRecipe>> GetRecipes(ProductType productType, RecipeClassification classification)
        {
            return Storage.LoadRecipesAsync(productType.Id, classification);
        }

        public async Task Save(IReadOnlyList<IProductRecipe> recipes)
        {
            await Storage.SaveRecipesAsync(recipes);
            foreach (var recipe in recipes)
            {
                RaiseRecipeChanged(recipe);
            }
        }

        public async Task<long> Save(IProductRecipe recipe)
        {
            var saved = await Storage.SaveRecipeAsync(recipe);
            RaiseRecipeChanged(recipe);
            return saved;
        }

        public Task Remove(long recipeId)
        {
            return Storage.RemoveRecipeAsync(recipeId);
        }

        public Task<IReadOnlyList<Workplan>> LoadAllWorkplansAsync()
        {
            using var uow = ModelFactory.Create();
            var repo = uow.GetRepository<IWorkplanRepository>();
            var workplans = (from entity in repo.Linq.Active()
                             select new Workplan
                             {
                                 Id = entity.Id,
                                 Name = entity.Name,
                                 Version = entity.Version,
                                 State = (WorkplanState)entity.State
                             }).ToArray();

            return Task.FromResult<IReadOnlyList<Workplan>>(workplans);
        }

        public Task<Workplan> LoadWorkplanAsync(long workplanId)
        {
            using var uow = ModelFactory.Create();
            return RecipeStorage.LoadWorkplanAsync(uow, workplanId);
        }

        public Task<IReadOnlyList<Workplan>> LoadVersionsAsync(long workplanId)
        {
            using var uow = ModelFactory.Create();

            var versions = new List<Workplan>();
            var workplanRepo = uow.GetRepository<IWorkplanRepository>();
            var referenceRepo = uow.GetRepository<IWorkplanReferenceRepository>();

            var currentVersion = workplanRepo.GetByKey(workplanId);
            if (currentVersion == null)
                return Task.FromResult<IReadOnlyList<Workplan>>(versions);

            // Convert current version
            versions.Add(new Workplan
            {
                Id = currentVersion.Id,
                Name = currentVersion.Name,
                Version = currentVersion.Version,
                State = (WorkplanState)currentVersion.State
            });

            // First fetch all previous versions
            var currentId = workplanId;
            do
            {
                var result = (from reference in referenceRepo.Linq
                              where reference.TargetId == currentId && reference.ReferenceType == (int)WorkplanReferenceType.NewVersion
                              let entity = reference.Source
                              select new Workplan
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  Version = entity.Version,
                                  State = (WorkplanState)entity.State
                              }).FirstOrDefault();

                if (result == null)
                    break;

                versions.Add(result);
                currentId = result.Id;
            } while (currentId > 0);

            // Now use a similar method to fetch all target versions
            currentId = workplanId;
            do
            {
                var result = (from reference in referenceRepo.Linq
                              where reference.SourceId == currentId && reference.ReferenceType == (int)WorkplanReferenceType.NewVersion
                              let entity = reference.Target
                              select new Workplan
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  Version = entity.Version,
                                  State = (WorkplanState)entity.State
                              }).FirstOrDefault();

                if (result == null)
                    break;

                versions.Add(result);
                currentId = result.Id;
            } while (currentId > 0);

            // Sort the versions
            versions = versions.OrderBy(v => v.Version).ToList();

            return Task.FromResult<IReadOnlyList<Workplan>>(versions);
        }

        public async Task<long> SaveWorkplanAsync(Workplan workplan)
        {
            using var uow = ModelFactory.Create();
            var recipeRepo = uow.GetRepository<IProductRecipeRepository>();

            // Update all non-clone recipes of that workplan
            var affectedRecipes = recipeRepo.Linq
                .Where(r => r.WorkplanId == workplan.Id && r.Classification > 0).ToList();

            var entity = RecipeStorage.ToWorkplanEntity(uow, workplan);
            foreach (var recipe in affectedRecipes)
            {
                recipe.Workplan = entity;
            }

            uow.SaveChanges();

            workplan.Id = entity.Id;

            foreach (var recipeEntity in affectedRecipes)
            {
                var recipe = await Storage.LoadRecipeAsync(recipeEntity.Id);
                RaiseRecipeChanged(recipe);
            }

            return entity.Id;
        }

        public Task<bool> DeleteWorkplanAsync(long workplanId)
        {
            using var uow = ModelFactory.Create();
            var repo = uow.GetRepository<IWorkplanRepository>();
            var workplan = repo.GetByKey(workplanId);
            if (workplan == null)
                return Task.FromResult(false);
            repo.Remove(workplan);
            uow.SaveChanges();
            return Task.FromResult(true);
        }

        private void RaiseRecipeChanged(IRecipe recipe)
        {
            // This must never be null
            // ReSharper disable once PossibleNullReferenceException
            RecipeChanged(this, recipe);
        }

        public IProductRecipe CreateRecipe(string recipeType)
        {
            return Storage.CreateRecipe(recipeType);
        }

        public event EventHandler<IRecipe> RecipeChanged;
    }
}
