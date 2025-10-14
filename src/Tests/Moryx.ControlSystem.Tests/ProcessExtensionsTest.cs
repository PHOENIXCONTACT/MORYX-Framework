// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Tests.Mocks;
using NUnit.Framework;

namespace Moryx.ControlSystem.Tests
{
    [TestFixture]
    internal class ProcessExtensionsTest
    {
        [Test]
        public void ShouldReturnOrderNumber()
        {
            // Arrange
            var recipe = new DummyRecipe { OrderNumber = "O10", OperationNumber = "Op082" };
            var process = new Process { Recipe = recipe };

            // Act
            var orderNumber = process?.GetOrderNumber();

            // Assert
            Assert.That(orderNumber, Is.EqualTo(recipe.OrderNumber));
        }
        
        [Test]
        public void ShouldReturnOperationNumber()
        {
            // Arrange
            var recipe = new DummyRecipe { OrderNumber = "O10", OperationNumber = "Op082" };
            var process = new Process { Recipe = recipe };

            // Act
            var operationNumber = process?.GetOperationNumber();

            // Assert
            Assert.That(operationNumber, Is.EqualTo(recipe.OperationNumber));
        }
    }
}