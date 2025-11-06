// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Products.Samples;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class ProductConstraintTest
    {
        [TestCase("10101", 2, true, Description = "Constraint product id is the same as the process product id")]
        [TestCase("9999", 2, false, Description = "Constraint product id is different as the process product id")]
        public void CheckProductIdOfConstraintMatches(string identifier, short revision, bool expectedResult)
        {
            // Arrange
            var ident = new ProductIdentity(identifier, revision);
            var constraint = ExpressionConstraint.Equals<IProcess>(p => ((IProductRecipe)p.Recipe).Product.Identity, ident);
            // Act Assert
            Assert.That(constraint.Check(CreateProcess()), Is.EqualTo(expectedResult));
        }

        private static IProcess CreateProcess()
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
}
