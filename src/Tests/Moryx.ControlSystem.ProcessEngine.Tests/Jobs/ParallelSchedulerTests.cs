// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs;

[TestFixture]
public class ParallelSchedulerTests : SchedulerTestBase
{
    private const int MaxActive = 3;

    protected override IJobScheduler CreateScheduler()
    {
        var scheduler = new ParallelScheduler
        {
            JobList = JobListMock.Object
        };
        scheduler.Initialize(new ParallelSchedulerConfig { MaxActiveJobs = MaxActive });

        return scheduler;
    }

    [Test(Description = "Should return the configured value for active jobs initially")]
    public void ReturnsTheParallelSlots()
    {
        // Arrange
        var jobs = CreateProductionJobs(5);

        // Act
        var slots = JobScheduler.SchedulableJobs(jobs).ToArray();

        // Assert
        Assert.That(slots.Length, Is.EqualTo(MaxActive), "There should be three slots");
    }

    [Test()]
    public void ShouldAssignAndStartWaitingJob()
    {
        // Arrange
        var job = new Job(Recipe, 1)
        {
            Classification = JobClassification.Waiting
        };
        var otherJobs = CreateProductionJobs(4);

        // Act
        JobScheduler.JobsReady([job]);
        var slots = JobScheduler.SchedulableJobs(otherJobs).ToArray();

        // Assert
        Assert.That(slots.Length, Is.EqualTo(2));
        Assert.That(ScheduledJob, Is.Not.Null);
    }

    [Test]
    public void ResumeJobsAfterRestart()
    {
        ResumeJobsAfterRestart(3);
    }
}