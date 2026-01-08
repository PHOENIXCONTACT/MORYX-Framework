// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Assignment;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests;

[TestFixture]
public class NullOperationValidationTests
{
    private NullOperationValidation _nullOperationValidation;
    private IOperationLogger _operationLogger;

    [SetUp]
    public Task SetUp()
    {
        var operationLoggerMock = new Mock<IOperationLogger>();
        _operationLogger = operationLoggerMock.Object;
        _nullOperationValidation = new NullOperationValidation();
        return _nullOperationValidation.InitializeAsync(new OperationValidationConfig());
    }

    [Test(Description = "Validates that null is a valid parameter when calling validate.")]
    public async Task ValidateNull()
    {
        // Act
        var result = await _nullOperationValidation.ValidateAsync(null, _operationLogger, CancellationToken.None);

        //Assert
        Assert.That(result, "There should be a successful validation");
    }

    [Test(Description = "Validate that null is a valid parameter when calling the Validation of CreationContext.")]
    public async Task ValidateCreationContextNull()
    {
        // Act
        var result = await _nullOperationValidation.ValidateCreationContextAsync(null, CancellationToken.None);

        // Assert
        Assert.That(result, "There should be a successful validation");
    }
}