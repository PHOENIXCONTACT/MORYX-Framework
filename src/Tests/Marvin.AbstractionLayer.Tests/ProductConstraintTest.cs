using Marvin.Products.Samples;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.Tests
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
            var constraint = ExpressionConstraint.Equals<IProcess>(p => ((IProductRecipe) p.Recipe).Product.Identity, ident);
            // Act Assert
            Assert.AreEqual(expectedResult, constraint.Check(CreateProcess()));            
        }

       private static IProcess CreateProcess()
        {
            return new ProductionProcess
            {
                Recipe = new ProductRecipe
                {
                    Product = new WatchProduct
                    {
                        Identity = new ProductIdentity("10101", 2)
                    }
                }
            };
        }
    }
}
