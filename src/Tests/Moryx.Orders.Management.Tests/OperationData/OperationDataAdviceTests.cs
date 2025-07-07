// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataAdviceTests : OperationDataTestBase
    {
        [Test(Description = "An advice should be possible after a successful creation until it is completed")]
        public void AdviceShouldBePossible()
        {
            // Arrange
            var ready = GetReadyOperation(1, false, 0, 0);
            var running = GetRunningOperation(1, false, 0, 0);
            var interrupting = GetInterruptingOperation(1, false, 0, 0);
            var interrupted = GetInterruptedOperation(1, false, 0, 0);
            var amountReached = GetAmountReachedOperation(1, false, 0, 0);
            var completed = GetCompletedOperation(1, false, 0, 0);

            // Assert
            Assert.That(ready.State.CanAdvice);
            Assert.That(running.State.CanAdvice);
            Assert.That(interrupting.State.CanAdvice);
            Assert.That(interrupted.State.CanAdvice);
            Assert.That(amountReached.State.CanAdvice);
            Assert.That(completed.State.CanAdvice);
        }

        [TestCase(Description = "There should be an event to inform about a performed advice ")]
        public void ShouldRaiseAnEventAfterAnAdvice()
        {
            // Arrange
            var operationData = GetReadyOperation(10, false, 0, 0);
            var advice = new OrderAdvice("Hosentasche", 5);
            var adviced = false;

            operationData.Adviced += delegate
            {
                adviced = true;
            };

            // Act
            operationData.Advice(advice);

            // Assert
            Assert.That(adviced, "There should be an event to inform about the advice");
            Assert.That(operationData.Operation.Advices.Count, Is.EqualTo(1), "There should be 1 advice stored");
        }

        [Test(Description = "An advice with a negative amount should not be possible")]
        public void ShouldThrowExceptionForNegativeAmountToAdvice()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 0, 0);
            var advice = new OrderAdvice("Kapuze", -1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => operationData.Advice(advice), "Negative amount should not be adviced");
            Assert.That(operationData.Operation.Advices.Count, Is.EqualTo(0), "There should be no stored advices");
        }
    }
}

