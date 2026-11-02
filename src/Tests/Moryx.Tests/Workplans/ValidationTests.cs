// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using Moryx.Workplans.Validation;
using NUnit.Framework;

namespace Moryx.Tests.Workplans;

[TestFixture]
public class ValidationTests
{
    [Test]
    public void ValidateFullSuccess()
    {
        // Arrange
        var workplan = WorkplanDummy.CreateFull();

        // Act
        var validation = WorkplanInstance.Validate(workplan, ValidationAspect.DeadEnd | ValidationAspect.LoneWolf);

        // Assert
        Assert.That(validation.Success, "Validation did not return success for a valid workplan!");
        Assert.That(validation.Errors.Length, Is.EqualTo(0), "Valid workplan must not report errors!");
    }

    [Test]
    public void ValidateLoneWolf()
    {
        // Arrange
        var workplan = WorkplanDummy.CreateLoneWolf();

        // Act
        var validation = WorkplanInstance.Validate(workplan, ValidationAspect.LoneWolf);

        // Assert
        Assert.That(validation.Success, Is.False, "Validation did not detect error!");
        Assert.That(validation.Errors.Length, Is.EqualTo(1), "Validation should have found one error!");
        Assert.That(validation.Errors[0], Is.InstanceOf<LoneWolfValidationError>(), $"Error should be of type {nameof(LoneWolfValidationError)}");

        var expected = workplan.Steps.First(s => s.Name == "LoneWolf");
        Assert.That(validation.Errors[0].PositionId, Is.EqualTo(expected.Id));

        var error = validation.Errors[0].Print(workplan);
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public void ValidateDeadEnd()
    {
        // Arrange
        var workplan = WorkplanDummy.CreateDeadEnd();

        // Act
        var validation = WorkplanInstance.Validate(workplan, ValidationAspect.DeadEnd);

        // Assert
        Assert.That(validation.Success, Is.False, "Validation did not detect error!");
        Assert.That(validation.Errors.Length, Is.EqualTo(1), "Validation should have found one error!");
        Assert.That(validation.Errors[0], Is.InstanceOf<DeadEndValidationError>(), $"Error should be of type {nameof(DeadEndValidationError)}");

        var expected = workplan.Connectors.First(c => c.Name == "DeadEnd");
        Assert.That(validation.Errors[0].PositionId, Is.EqualTo(expected.Id));

        var error = validation.Errors[0].Print(workplan);
        Assert.That(error, Is.Not.Null);
    }

    [Test(Description = "Validate that a missing success path is detected in the workplan")]
    public void ValidateLuckyStreak()
    {
        // Arrange
        var workplan = WorkplanDummy.CreateNoLuckyStreak();

        // Act
        var validation = WorkplanInstance.Validate(workplan, ValidationAspect.LuckyStreak);

        // Assert
        Assert.That(validation.Success, Is.False, "Validation did not detect error!");
        Assert.That(validation.Errors.Length, Is.EqualTo(1), "Validation should have found one error!");
        Assert.That(validation.Errors[0], Is.InstanceOf<LuckyStreakValidationError>(), $"Error should be of type {nameof(LuckyStreakValidationError)}");

        var expected = workplan.Steps.First(s => s.Name == "FailOnly");
        Assert.That(validation.Errors[0].PositionId, Is.EqualTo(expected.Id));

        var error = validation.Errors[0].Print(workplan);
        Assert.That(error, Is.Not.Null);
    }

    [Test(Description = "Validate that a cycle on the success-only path is detected")]
    public void ValidateLuckyStreakSuccessPathCycle()
    {
        // Arrange
        var workplan = WorkplanDummy.CreateSuccessPathCycle();

        // Act
        var validation = WorkplanInstance.Validate(workplan, ValidationAspect.LuckyStreak);

        // Assert
        Assert.That(validation.Success, Is.False, "Validation did not detect error!");
        Assert.That(validation.Errors, Is.All.InstanceOf<SuccessPathCycleValidationError>(), $"Errors should be of type {nameof(SuccessPathCycleValidationError)}");
        Assert.That(validation.Errors.Length, Is.EqualTo(3), "All three cycle steps should be reported!");

        var error = validation.Errors[0].Print(workplan);
        Assert.That(error, Is.Not.Null);
    }

    [Test(Description = "Validate that an infinite loop is detected in the workplan")]
    public void ValidateInfiniteLoop()
    {
        // Arrange
        var workplan = WorkplanDummy.CreateInfiniteLoop();

        // Act
        var validation = WorkplanInstance.Validate(workplan, ValidationAspect.InfiniteLoop);

        // Assert
        Assert.That(validation.Success, Is.False, "Validation did not detect error!");
        Assert.That(validation.Errors.Length, Is.EqualTo(3), "Validation should have found three errors!");
        Assert.That(validation.Errors, Is.All.InstanceOf<InfiniteLoopValidationError>(), $"Errors should be of type {nameof(InfiniteLoopValidationError)}");

        var expectedIds = workplan.Steps.Select(s => s.Id).ToArray();
        Assert.That(validation.Errors.Select(e => e.PositionId), Is.EquivalentTo(expectedIds));

        var error = validation.Errors[0].Print(workplan);
        Assert.That(error, Is.Not.Null);
    }
}
