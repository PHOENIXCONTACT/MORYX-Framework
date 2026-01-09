// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Tests.TestData;
using Moryx.Products.Samples;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class ProductConstraintTests
{
    [TestCase("10101", 2, true, Description = "Constraint product id is the same as the process product id")]
    [TestCase("9999", 2, false, Description = "Constraint product id is different as the process product id")]
    public void CheckProductIdOfConstraintMatches(string identifier, short revision, bool expectedResult)
    {
        // Arrange
        var ident = new ProductIdentity(identifier, revision);
        var constraint = ExpressionConstraint.Equals<ActivityConstraintContext>(p => ((IProductRecipe)p.Process.Recipe).Product.Identity, ident);

        // Act
        var constraintContext = new ActivityConstraintContext(CreateActivity());

        // Assert
        Assert.That(constraint.Check(constraintContext), Is.EqualTo(expectedResult));
    }

    private static Activity CreateActivity()
    {
        var activityMock = new TestActivity();
        activityMock.Process = CreateProcess();
        return activityMock;
    }

    private static ProductionProcess CreateProcess()
    {
        return new ProductionProcess
        {
            Recipe = new ProductRecipe
            {
                Product = new WatchType
                {
                    Identity = new ProductIdentity("10101", 2)
                }
            }
        };
    }
}