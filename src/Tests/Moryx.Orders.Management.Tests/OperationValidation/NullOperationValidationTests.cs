﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moq;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Assignment;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class NullOperationValidationTests
    {
        private NullOperationValidation _nullOperationValidation;
        private IOperationLogger _operationLogger;

        [SetUp]
        public void SetUp()
        {
            var operationLoggerMock = new Mock<IOperationLogger>();
            _operationLogger = operationLoggerMock.Object;
            _nullOperationValidation = new NullOperationValidation();
            _nullOperationValidation.Initialize(new OperationValidationConfig());
        }

        [Test(Description = "Validates that null is a valid parameter when calling validate.")]
        public async Task ValidateNull()
        {
            // Act
            var result = await _nullOperationValidation.Validate(null, _operationLogger);

            //Assert
            Assert.That(result, "There should be a successful validation");
        }

        [Test(Description="Validate that null is a valid parameter when calling the Validation of CreationContext.")]
        public void ValidateCreationContextNull()
        {
            //Assert
            Assert.That(_nullOperationValidation.ValidateCreationContext(null), "There should be a successful validation");
        }
    }
}

