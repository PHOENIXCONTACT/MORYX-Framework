// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataAdviceTests : OperationDataTestBase
    {
        [Test(Description = "An advice should be possible after a successful creation until it is completed")]
        public async Task AdviceShouldBePossible()
        {
            // Arrange
            var ready = await GetReadyOperation(1, false, 0, 0);
            var running = await GetRunningOperation(1, false, 0, 0);
            var interrupting = await GetInterruptingOperation(1, false, 0, 0);
            var interrupted = await GetInterruptedOperation(1, false, 0, 0);
            var amountReached = await GetAmountReachedOperation(1, false, 0, 0);
            var completed = await GetCompletedOperation(1, false, 0, 0);

            // Assert
            Assert.That(ready.State.CanAdvice);
            Assert.That(running.State.CanAdvice);
            Assert.That(interrupting.State.CanAdvice);
            Assert.That(interrupted.State.CanAdvice);
            Assert.That(amountReached.State.CanAdvice);
            Assert.That(completed.State.CanAdvice);
        }

        [TestCase(Description = "There should be an event to inform about a performed advice ")]
        public async Task ShouldRaiseAnEventAfterAnAdvice()
        {
            // Arrange
            var operationData = await GetReadyOperation(10, false, 0, 0);
            var advice = new OrderAdvice("Hosentasche", 5);
            var adviced = false;

            operationData.Adviced += delegate
            {
                adviced = true;
            };

            // Act
            await operationData.Advice(advice);

            // Assert
            Assert.That(adviced, "There should be an event to inform about the advice");
            Assert.That(operationData.Operation.Advices.Count, Is.EqualTo(1), "There should be 1 advice stored");
        }

        [Test(Description = "An advice with a negative amount should not be possible")]
        public async Task ShouldThrowExceptionForNegativeAmountToAdvice()
        {
            // Arrange
            var operationData = await GetRunningOperation(10, false, 0, 0);
            var advice = new OrderAdvice("Kapuze", -1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => operationData.Advice(advice), "Negative amount should not be adviced");
            Assert.That(operationData.Operation.Advices.Count, Is.EqualTo(0), "There should be no stored advices");
        }
    }
}

