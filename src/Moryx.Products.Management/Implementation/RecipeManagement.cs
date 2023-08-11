// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Container;
using Moryx.Model;
using Moryx.Model.Repositories;
using Moryx.Products.Model;
using Moryx.Tools;
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

        public IProductRecipe Get(long recipeId)
        {
            var recipe = Storage.LoadRecipe(recipeId);
            if (recipe == null)
                throw new RecipeNotFoundException(recipeId);

            return recipe;
        }

        public IReadOnlyList<IProductRecipe> GetAllByProduct(IProductType productType)
        {
            return Storage.LoadRecipes(productType.Id, RecipeClassification.CloneFilter);
        }

        public IReadOnlyList<IProductRecipe> GetRecipes(IProductType productType, RecipeClassification classification)
        {
            return Storage.LoadRecipes(productType.Id, classification);
        }

        public long Save(IProductRecipe recipe)
        {
            var saved = Storage.SaveRecipe(recipe);
            RaiseRecipeChanged(recipe);
            return saved;
        }

        public void Save(long productId, ICollection<IProductRecipe> recipes)
        {
            Storage.SaveRecipes(productId, recipes);
            foreach (var recipe in recipes)
                RaiseRecipeChanged(recipe);
        }

        public void Remove(long recipeId)
        {
            Storage.RemoveRecipe(recipeId);
        }

        public IReadOnlyList<Workplan> LoadAllWorkplans()
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
            return workplans;
        }

        public Workplan LoadWorkplan(long workplanId)
        {
            using var uow = ModelFactory.Create();
            return RecipeStorage.LoadWorkplan(uow, workplanId);
        }

        public IReadOnlyList<Workplan> LoadVersions(long workplanId)
        {
            using var uow = ModelFactory.Create();

            var repo = uow.GetRepository<IWorkplanRepository>();
            var currentWorkplan = repo.GetByKey(workplanId);

            if (currentWorkplan == null) return new List<Workplan>();

            List<WorkplanReferenceEntity> backwardReferences = new List<WorkplanReferenceEntity>();
            List<WorkplanReferenceEntity> forwardReferences = new List<WorkplanReferenceEntity>();

            currentWorkplan.TargetReferences.ForEach(reference =>
            {
                backwardReferences.AddRange(GetAllReferences(reference));
            });

            currentWorkplan.SourceReferences.ForEach(reference =>
            {
                forwardReferences.AddRange(GetAllReferences(reference,false));
            });

            // concact backward search and forward search results
            var outputReferences = backwardReferences.Concat(forwardReferences);

            //flatten the list of target and sources into a single list
            var references = outputReferences
                .Select(x => x.Target)
                .Concat(outputReferences
                .Select(x => x.Source));

            var versions = references
                .Select(x => new Workplan
                {
                    Id = x.Id,
                    Name = x.Name,
                    Version = x.Version,
                    State = (WorkplanState)x.State
                })
                .OrderBy(x => x.Version)
                .ToList();
            return versions;
        }

        private ICollection<WorkplanReferenceEntity> GetAllReferences(WorkplanReferenceEntity reference,bool backward = true)
        {
            var results = new List<WorkplanReferenceEntity>();
            if(backward)
            {
                foreach (var item in reference.Source.TargetReferences)
                {
                    if (item.Source.TargetReferences.Count == 0)
                        results.Add(item);
                    else
                    {
                        var founds = GetAllReferences(item);
                        if (founds is not null && founds.Count > 0)
                            results.AddRange(founds);
                    }
                }
            }
            else
            {
                foreach (var item in reference.Target.SourceReferences)
                {
                    if (item.Target.SourceReferences.Count == 0)
                        results.Add(item);
                    else
                    {
                        var founds = GetAllReferences(item,false);
                        if (founds is not null && founds.Count > 0)
                            results.AddRange(founds);
                    }
                }
            }
            //add the current reference
            results.Add(reference);

            return results;
        }

        public long SaveWorkplan(Workplan workplan)
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
                var recipe = Storage.LoadRecipe(recipeEntity.Id);
                RaiseRecipeChanged(recipe);
            }

            return entity.Id;
        }

        public bool DeleteWorkplan(long workplanId)
        {
            using var uow = ModelFactory.Create();
            var repo = uow.GetRepository<IWorkplanRepository>();
            var workplan = repo.GetByKey(workplanId);
            if (workplan == null)
                return false;
            repo.Remove(workplan);
            uow.SaveChanges();
            return true;
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
