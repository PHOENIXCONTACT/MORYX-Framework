// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Recipes;
using Moq;
using Moryx.Orders.Management.Assignment;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class RegexOperationValidationTests
    {
        private RegexOperationValidation _regexOperationValidation;
        private OrderCreationContext _orderCreationContext;
        private IOperationLogger _operationLogger;
        private InternalOperation _operation;

        [SetUp]
        public async Task SetUp()
        {
            var operationLoggerMock = new Mock<IOperationLogger>();
            _operationLogger = operationLoggerMock.Object;

            _regexOperationValidation = new RegexOperationValidation();
            await _regexOperationValidation.InitializeAsync(new RegexOperationValidationConfig());

            _operation = new InternalOperation
            {
                Number = "0020",
                TotalAmount = 10,
                Recipes = new List<IProductRecipe>(1) { new ProductRecipeReference(0) }
            };

            _orderCreationContext = new OrderCreationContext();
        }

        [TestCase("0040", Description = "Test that a numeric 4 character long string is valid with default config.")]
        [TestCase("ABCD", Description = "Test that a alphabetic 4 character long string is valid with default config.")]
        [TestCase("10AG", Description = "Test that a mix of numeric and alphabetic 4 character long string is valid with default config.")]
        public async Task ValidateGoodOperationDataNumber(string number)
        {
            // Arrange
            _operation.Number = number;

            // Act
            var result = await _regexOperationValidation.Validate(_operation, _operationLogger);

            // Assert
            Assert.That(result, "There should be a successful validation");
        }

        [TestCase("00400", Description = "Test that a string with length > 4 will be not valid with default config.")]
        [TestCase("103", Description = "Test that a string with length < 4 will be not valid with default config.")]
        [TestCase(" 456", Description = "Test that a string with whitespace at position 1 will be not valid with default config.")]
        [TestCase("93 3", Description = "Test that a string with whitespace at position 3 will be not valid with default config.")]
        [TestCase("", Description = "Test that an empty string is not valid with default config.")]
        public async Task ValidateBadOperationDataNumber(string number)
        {
            // Arrange
            _operation.Number = number;

            // Act
            var result = await _regexOperationValidation.Validate(_operation, _operationLogger);

            // Assert
            Assert.That(result, Is.False, "There should be an unsuccessful validation");
        }

        [Test(Description = "Test that the OperationData value total amount of >=1 is valid ")]
        public async Task ValidateGoodOperationDataTotalAmount()
        {
            // Arrange
            _operation.TotalAmount = 1;

            // Act
            var result = await _regexOperationValidation.Validate(_operation, _operationLogger);

            // Assert
            Assert.That(result, "There should be a successful validation");
        }

        [TestCase(0, Description = "Test that an amount of 0 is not valid.")]
        [TestCase(-1, Description = "Test that a negative amount is not valid.")]
        public async Task ValidateBadOperationDataTotalAmount(int totalAmount)
        {
            // Arrange
            _operation.TotalAmount = totalAmount;

            // Act
            var result = await _regexOperationValidation.Validate(_operation, _operationLogger);

            // Assert
            Assert.That(result, Is.False, "There should be an unsuccessful validation");
        }

        [Test(Description = "Test that will verify that a non null value for recipe is valid.")]
        public async Task ValidateRecipeExists()
        {
            // Act
            var result = await _regexOperationValidation.Validate(_operation, _operationLogger);

            // Assert
            Assert.That(result, "There should be a successful validation");
        }

        [Test(Description = "Test that a null value is not valid for a recipe")]
        public async Task ValidateNoRecipeSet()
        {
            // Arrange
            _operation.Recipes = new List<IProductRecipe>();

            // Act
            var result = await _regexOperationValidation.Validate(_operation, _operationLogger);

            // Assert
            Assert.That(result, Is.False, "There should be an unsuccessful validation");
        }

        [Test(Description = "Test that a CreationContext without OperationContext is valid.")]
        public void ValidateOrderCreationContextWithoutOperationContext()
        {
            // Assert
            Assert.That(_regexOperationValidation.ValidateCreationContext(_orderCreationContext), "There should be a successful validation");
        }

        [Test(Description = "Test that a CreationContext with a OperationContext has valid number and amount values.")]
        public void ValidateOrderCreationContextWithValidOperationContext()
        {
            // Arrange
            _orderCreationContext.Operations.Add(new OperationCreationContext
            {
                Number = "0010",
                TotalAmount = 10,
            });

            // Assert
            Assert.That(_regexOperationValidation.ValidateCreationContext(_orderCreationContext), "There should be a successful validation");
        }

        [TestCase("00107", 10, Description = "Test that will prove that a OperationContext number with 5 characters is not valid.")]
        [TestCase("0010", 0, Description = "Test that will prove that a OperationContext amount of 0 is not valid.")]
        public void ValidateOrderCreationContextWithBadOperationContext(string number, int amount)
        {
            // Arrange
            _orderCreationContext.Operations.Add(new OperationCreationContext
            {
                Number = number,
                TotalAmount = amount,
            });

            // Assert
            Assert.That(_regexOperationValidation.ValidateCreationContext(_orderCreationContext), Is.False, "There should be an unsuccessful validation");
        }
    }
}
