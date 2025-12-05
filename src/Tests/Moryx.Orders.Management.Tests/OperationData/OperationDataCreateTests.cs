// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moq;
using Moryx.Users;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataCreateTests : OperationDataTestBase
    {
        [Test(Description = "Checks if the creation of the operation will call the needed components.")]
        public async Task CreateShouldCallOperationCreation()
        {
            // Arrange
            var operationData = await InitializeOperationData(10, false, 11, 9);

            // Act
            await operationData.Assign();

            // Assert
            Assert.DoesNotThrow(() => AssignmentMock.Verify(c => c.Assign(operationData), Times.Once));
        }

        [Test(Description = "Create should be possible if creation was failed.")]
        public async Task CreateAgainIfCreationFailed()
        {
            // Arrange
            var operationData = await InitializeOperationData(10, false, 11, 9);

            await operationData.Assign();
            await operationData.AssignCompleted(false);

            // Act - Assert
            Assert.That(operationData.State.IsFailed);
            Assert.DoesNotThrowAsync(operationData.Assign);
            await operationData.AssignCompleted(true);
            Assert.DoesNotThrow(() => operationData.Adjust(10, new User()));
        }

        [Test(Description = "Validates the BeginContext after the creation of an Operation")]
        public async Task ValidateBeginContextAfterCreation()
        {
            // Arrange
            const int amount = 10;
            var operationData = await GetReadyOperation(amount, false, amount, amount);

            // Act
            var beginContext = operationData.GetBeginContext();

            // Assert
            Assert.That(beginContext.SuccessCount, Is.EqualTo(0));
            Assert.That(beginContext.ScrapCount, Is.EqualTo(0));
            Assert.That(beginContext.ResidualAmount, Is.EqualTo(amount));
        }
    }
}

