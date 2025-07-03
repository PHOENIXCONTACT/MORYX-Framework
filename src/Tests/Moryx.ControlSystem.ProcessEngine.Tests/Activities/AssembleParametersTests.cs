using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.VisualInstructions;
using NUnit.Framework;
using Moryx.ControlSystem.Assemble;
using Moryx.ControlSystem.TestTools.Identity;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Activities
{
    [TestFixture]
    public class AssembleParametersTests
    {
        [Test]
        public void ResolveParameterBinding()
        {
            // Arrange
            var parameters = new AssembleParameters
            {
                Instructions = new[]
                {
                    new VisualInstruction
                    {
                        Content = "The name of this Product was {Product.Name}",
                        Type = InstructionContentType.Text
                    }
                }
            };

            // Act
            IParameters resolved = parameters.Bind(DummyProcess());

            // Assert
            Assert.That(resolved, Is.Not.Null);
            Assert.That(resolved, Is.InstanceOf<AssembleParameters>());
            AssembleParameters resolveAssemble = (AssembleParameters) resolved;
            Assert.That(resolveAssemble.Instructions.Length, Is.EqualTo(1));
            Assert.That(resolveAssemble.Instructions[0].Content, Is.EqualTo("The name of this Product was Hugonotte"));
        }

        private static IProcess DummyProcess()
        {
            var product = new DummyProductType
            {
                Id = 42,
                Name = "Hugonotte",
                Identity = new ProductIdentity("4712", 01),
            };
            var process = new ProductionProcess
            {
                Id = 42,
                Recipe = new DummyRecipe
                {
                    Id = 42,
                    Name = "TheOneAndOnlyRecipe",
                    Product = product,

                },
                ProductInstance = product.CreateInstance()
            };
            process.ProductInstance.Id = 42;
            if(process.ProductInstance is IIdentifiableObject identifiableObject)
                identifiableObject.Identity = new TestNumberIdentity(0, "0816");

            return process;
        }
    }
}