// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Users;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataCreateTests : OperationDataTestBase
    {
        [Test(Description = "Checks if the creation of the operation will call the needed components.")]
        public void CreateShouldCallOperationCreation()
        {
            // Arrange
            var operationData = InitializeOperationData(10, false, 11, 9);

            // Act
            operationData.Assign();

            // Assert
            Assert.DoesNotThrow(() => AssignmentMock.Verify(c => c.Assign(operationData), Times.Once));
        }

        [Test(Description = "Create should be possible if creation was failed.")]
        public void CreateAgainIfCreationFailed()
        {
            // Arrange
            var operationData = InitializeOperationData(10, false, 11, 9);

            operationData.Assign();
            operationData.AssignCompleted(false);

            // Act - Assert
            Assert.That(operationData.State.IsFailed);
            Assert.DoesNotThrow(operationData.Assign);
            operationData.AssignCompleted(true);
            Assert.DoesNotThrow(() => operationData.Adjust(10, new User()));
        }

        [Test(Description = "Validates the BeginContext after the creation of an Operation")]
        public void ValidateBeginContextAfterCreation()
        {
            // Arrange
            const int amount = 10;
            var operationData = GetReadyOperation(amount, false, amount, amount);

            // Act
            var beginContext = operationData.GetBeginContext();

            // Assert
            Assert.That(beginContext.SuccessCount, Is.EqualTo(0));
            Assert.That(beginContext.ScrapCount, Is.EqualTo(0));
            Assert.That(beginContext.ResidualAmount, Is.EqualTo(amount));
        }
    }
}

