// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Model;
using Moryx.Model.Attributes;
using Moryx.Model.Repositories;
using Moryx.Products.Management;
using Moryx.Products.Management.Model;
using Moryx.Workplans;

namespace Moryx.Benchmarking.Setups;

[ModelSetup(typeof(ProductsContext))]
public class ProductSetup : IModelSetup
{
    /// <summary>
    /// For this data model unique setup id
    /// </summary>
    public int SortOrder => 1;

    /// <summary>
    /// Display name of this setup
    /// </summary>
    public string Name => "BenchmarkProduct";

    /// <summary>
    /// Short description what data this setup contains
    /// </summary>
    public string Description => "Creates product and workplan for benchmarks";

    /// <summary>
    /// Filetype supported by this setup
    /// </summary>
    public string SupportedFileRegex => null;

    /// <summary>
    /// Execute setup in this context
    /// </summary>
    /// <param name="openContext">Context for db access</param>
    /// <param name="setupData">Any data for the setup, excel or sql etc</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    public Task ExecuteAsync(IUnitOfWork openContext, string setupData, CancellationToken cancellationToken)
    {
        var prodRepo = openContext.GetRepository<IProductTypeRepository>();
        var propRepo = openContext.GetRepository<IProductPropertiesRepository>();

        // Create product
        var prod = prodRepo.Create("123456", 1, "DummyProduct", typeof(BenchmarkProduct).FullName);
        var prop = propRepo.Create();
        prod.SetCurrentVersion(prop);

        // Create some recipes
        ProductRecipeEntity recipe = null;
        for (var i = 5; i < 30; i += 4)
        {
            recipe = CreateRecipe(openContext, prod, i);
        }

        openContext.SaveChanges();

        recipe.Classification = (int)RecipeClassification.Default;

        openContext.SaveChanges();

        return Task.CompletedTask;
    }

    private static ProductRecipeEntity CreateRecipe(IUnitOfWork openContext, ProductTypeEntity product, int stepCount)
    {
        var recipeRepo = openContext.GetRepository<IProductRecipeRepository>();

        // Create workplan
        var workplan = new Workplan
        {
            Name = "Workplan-Steps:" + stepCount,
            Version = 1,
            State = WorkplanState.Released
        };
        var start = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
        var end = WorkplanInstance.CreateConnector("End", NodeClassification.End);
        var error = WorkplanInstance.CreateConnector("Oops");
        var failed = WorkplanInstance.CreateConnector("Shazam!", NodeClassification.Failed);
        workplan.Add(start, end, error, failed);

        var connector = start;
        for (var i = 0; i < stepCount; i++)
        {
            var step = new BenchmarkStep
            {
                Parameters = new BenchmarkParameters
                {
                    Step = i + 1
                },
                Inputs =
                {
                    [0] = connector
                }
            };
            if (i < stepCount - 1)
            {
                connector = WorkplanInstance.CreateConnector(i.ToString("D2"));
                workplan.Add(connector);
            }
            else
            {
                connector = end;
            }
            step.Outputs[0] = step.Outputs[1] = connector;
            step.Outputs[2] = error;
            workplan.Add(step);
        }

        var final = new BenchmarkStep
        {
            Parameters = new BenchmarkParameters
            {
                Step = 30,
                Timeout = 5
            },
            Inputs =
            {
                [0] = error
            },
            Outputs =
            {
                [0] = failed,
                [1] = failed,
                [2] = failed
            }
        };
        workplan.Add(final);

        var workplanEntity = RecipeStorage.ToWorkplanEntity(openContext, workplan);

        // Create recipe
        var recipe = recipeRepo.Create();
        recipe.Name = "Recipe-Steps:" + stepCount;
        recipe.Classification = (int)RecipeClassification.Alternative;
        recipe.TypeName = typeof(ProductionRecipe).FullName;
        recipe.State = (int)RecipeState.Released;
        recipe.Workplan = workplanEntity;
        recipe.Product = product;

        return recipe;
    }
}