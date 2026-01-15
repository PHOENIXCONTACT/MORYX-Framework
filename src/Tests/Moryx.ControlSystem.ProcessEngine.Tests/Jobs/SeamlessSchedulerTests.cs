// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.ControlSystem.TestTools;
using Moryx.Logging;
using Moryx.TestTools.UnitTest;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs;

[TestFixture]
public class SeamlessSchedulerTests : SchedulerTestBase
{
    protected override IJobScheduler CreateScheduler()
    {
        var scheduler = new SeamlessScheduler
        {
            JobList = JobListMock.Object,
            Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
        };
        scheduler.Initialize(new SeamlessSchedulerConfig());
        return scheduler;
    }

    public void SchedulableJobs_WithMultipleJobs_ReturnsOne()
    {
        // Arrange
        var jobs = CreateProductionJobs(5);

        // Act
        var slots = JobScheduler.SchedulableJobs(jobs).ToArray();

        // Assert
        Assert.That(slots.Length, Is.EqualTo(1));
    }

    [Test]
    public void JobsReady_WithCompletingJob_SchedulesReadyJob()
    {
        // Arrange
        var waiting = ArrangeWaitingJobAfterCompletingWithCleanup();

        // Act
        JobScheduler.JobsReady([waiting]);

        // Assert
        Assert.That(waiting, Is.EqualTo(ScheduledJob));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob (Completing) -> CleanupJob (Waiting) -> WaitingJob (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: ProductionJob (Completing)
    /// CompletingSlots: -empty-
    /// </summary>
    private Job ArrangeWaitingJobAfterCompletingWithCleanup()
    {
        Recipe.Id = 42;

        var job = new Job(Recipe, 1)
        {
            Classification = JobClassification.Waiting
        };
        var cleanup = new Job(new SetupRecipe
        {
            Execution = SetupExecution.AfterProduction,
            TargetRecipe = Recipe
        }, 1)
        {
            Classification = JobClassification.Waiting
        };
        var waiting = new Job(Recipe, 1)
        {
            Classification = JobClassification.Waiting
        };

        JobListMock.Setup(j => j.Previous(cleanup)).Returns(job);
        JobListMock.Setup(j => j.Next(job)).Returns(cleanup);
        JobScheduler.SlotAvailable += (sender, args) => { };

        // The initial newJob is started and than completing BEFORE the other one is added
        JobScheduler.SchedulableJobs([job, waiting]);
        JobScheduler.JobsReady([job, cleanup]);
        job.Classification = JobClassification.Completing;
        JobScheduler.JobUpdated(job, JobClassification.Completing);
        return waiting;
    }

    [Test]
    public void JobsUpdated_CompletingFollowUpsInReverseOrder_DoesNotScheduleCleanUp()
    {
        var jobs = ArrangeCompletingFollowUpsAndCleanUpAndSeperateRunningJob();

        jobs[1].Classification = JobClassification.Completed;
        JobScheduler.JobUpdated(jobs[1], JobClassification.Completed);

        Assert.That(ScheduledJob, Is.Not.EqualTo(jobs[2]));
    }

    [Test]
    public void JobsUpdated_CompletedFollowUpsInReverseOrder_ScheduleCleanUp()
    {
        var jobs = ArrangeCompletingFollowUpsAndCleanUpAndSeperateRunningJob();
        jobs[1].Classification = JobClassification.Completed;
        JobScheduler.JobUpdated(jobs[1], JobClassification.Completed);

        jobs[0].Classification = JobClassification.Completed;
        JobScheduler.JobUpdated(jobs[0], JobClassification.Completed);

        Assert.That(ScheduledJob, Is.EqualTo(jobs[2]));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob 1 (Completing) -> ProductionJob 2 (Completing) -> CleanupJob 1 (Waiting) -> ProductionJob 3 (Running) -> CleanupJob 2 (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: ProductionJob 3 (Running)
    /// CompletingSlots: ProductionJob 2 (Completing)
    /// </summary>
    private List<Job> ArrangeCompletingFollowUpsAndCleanUpAndSeperateRunningJob()
    {
        Recipe.Id = 42;
        var jobs = new List<Job>
        {
            new(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            },
            // Validate Id instead of reference equality checks
            new(DummyRecipe.BuildRecipe(Recipe.Id), 1)
            {
                Classification = JobClassification.Waiting
            },
            new(CleanUpRecipe, 1)
            {
                Classification = JobClassification.Waiting
            },
            new(AnotherRecipe, 1)
            {
                Classification = JobClassification.Waiting
            },new(CleanUpRecipe, 1)
            {
                Classification = JobClassification.Waiting
            },
        };
        SetupJobListFor(jobs);

        JobScheduler.JobsReady(jobs.Take(3));
        jobs[0].Classification = JobClassification.Completing;
        JobScheduler.JobUpdated(jobs[0], JobClassification.Completing);
        jobs[1].Classification = JobClassification.Completing;
        JobScheduler.JobUpdated(jobs[1], JobClassification.Completing);

        JobScheduler.JobsReady(jobs.Skip(3));
        jobs[3].Classification = JobClassification.Running;
        JobScheduler.JobUpdated(jobs[0], JobClassification.Running);

        return jobs;
    }

    [Test]
    public void JobUpdated_ToCompletedFromResumedWithRelatedFollowUpJob_DoesNotScheduleCleanUp()
    {
        var jobs = ArrangeResumedCompletingWithRunningFollowUpJobAndCleanUp();

        // Act
        JobScheduler.JobUpdated(jobs[0], JobClassification.Completed);

        // Assert
        Assert.That(ScheduledJob, Is.Not.EqualTo(jobs[2]));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob (Completing) -> ProductionJob (Running) -> CleanupJob (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: ProductionJob (Running)
    /// CompletingSlots: -empty-
    /// </summary>
    private List<Job> ArrangeResumedCompletingWithRunningFollowUpJobAndCleanUp()
    {
        // Arrange
        var scheduledJobs = new List<Job>();
        var jobs = new List<Job>
        {
            new EngineJob(Recipe, 20) // interrupted completing job
            {
                Classification = JobClassification.Idle,
                SuccessCount = 10,
                Running = { new Process { Id = 1 } }
            },
            new EngineJob(Recipe, 5) // interrupted running job
            {
                Classification = JobClassification.Idle,
                FailureCount = 2,
            },
            new(CleanUpRecipe, 1)
            {
                Classification = JobClassification.Waiting
            },
        };

        SetupJobListFor(jobs);

        JobScheduler.Scheduled += (sender, job) =>
        {
            scheduledJobs.Add(job);
            if (job == jobs[0])
            {
                jobs[0].Classification = JobClassification.Completing;
            }
            else if (job == jobs[1])
            {
                jobs[1].Classification = JobClassification.Running;
            }
        };
        JobScheduler.JobsReady(jobs);
        JobScheduler.JobUpdated(jobs[0], jobs[0].Classification);
        JobScheduler.JobUpdated(jobs[1], jobs[1].Classification);
        return jobs;
    }

    [Test]
    public void ResumeJobsAfterRestart()
    {
        ResumeJobsAfterRestart(2);
    }

    [Test]
    public void JobsReady_WithMultipleJobs_SchedulesFirstJob()
    {
        var jobs = ArrangeWaitingJobWithTwoFollowUpsAndCleanUp();

        JobScheduler.JobsReady(jobs);

        Assert.That(jobs[0], Is.EqualTo(ScheduledJob));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob (Waiting) -> ProductionJob (Waiting) -> ProductionJob (Waiting) -> CleanupJob (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: -empty-
    /// CompletingSlots: -empty-
    /// </summary>
    private List<Job> ArrangeWaitingJobWithTwoFollowUpsAndCleanUp()
    {
        Recipe.Id = 42;
        var jobs = new List<Job>
        {
            new(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            },
            new(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            },
            new(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            },
            new(CleanUpRecipe, 1)
            {
                Classification = JobClassification.Waiting
            }
        };
        SetupJobListFor(jobs);
        return jobs;
    }

    [Test]
    public void JobUpdated_ToCompleting_SchedulesFollowUpJob()
    {
        var jobs = ArrangeCompletingJobWithTwoFollowUpsAndCleanUp();

        JobScheduler.JobUpdated(jobs[0], JobClassification.Completing);

        Assert.That(jobs[1], Is.EqualTo(ScheduledJob));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob 1 (Completing) -> ProductionJob 2 (Waiting) -> ProductionJob 3 (Waiting) -> CleanupJob (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: ProductionJob 1
    /// CompletingSlots: -empty-
    /// </summary>
    private List<Job> ArrangeCompletingJobWithTwoFollowUpsAndCleanUp()
    {
        var jobs = ArrangeWaitingJobWithTwoFollowUpsAndCleanUp();
        // Prepare first newJob as completing
        JobScheduler.JobsReady(jobs);
        jobs[0].Classification = JobClassification.Completing;
        return jobs;
    }

    [Test]
    public void JobUpdated_ToCompletedWithCanceledFollowUps_SchedulesCleanUp()
    {
        var jobs = ArrangeCompletingJobWithTwoCanceledFollowUpsAndCleanUp();

        JobScheduler.JobUpdated(jobs[0], JobClassification.Completed);

        Assert.That(jobs[3], Is.EqualTo(ScheduledJob));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob 1 (Completing) -> ProductionJob 2 (Completed) -> ProductionJob 3 (Completed) -> CleanupJob (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: ProductionJob 1
    /// CompletingSlots: -empty-
    /// </summary>
    private List<Job> ArrangeCompletingJobWithTwoCanceledFollowUpsAndCleanUp()
    {
        //Arrange
        var jobs = ArrangeCompletingJobWithTwoFollowUpsAndCleanUp();

        // Now we abort second and third newJob while the scheduler still processes events
        jobs[1].Classification = jobs[2].Classification = JobClassification.Completed;
        JobScheduler.JobUpdated(jobs[1], JobClassification.Completing);
        JobScheduler.JobUpdated(jobs[1], JobClassification.Completed);
        JobScheduler.JobUpdated(jobs[2], JobClassification.Completed);
        return jobs;
    }

    [Test]
    public void JobUpdated_ToCompletedWithCanceledFollowUps_RaisesClotsAvailable()
    {
        var jobs = ArrangeCompletingJobWithTwoCanceledFollowUpsAndCleanUp();

        JobScheduler.JobUpdated(jobs[0], JobClassification.Completed);

        Assert.That(SlotsAvailableCalled);
    }

    [Test]
    public void SchedulableJobs_ForNewJobWhileCleanUpIsScheduled_ReturnsNewJob()
    {
        ArrangeScheduledCleanUpAfterCompletedAndCanceledJobs();
        var newJob = new Job(DummyRecipe.BuildRecipe(43), 1) { Classification = JobClassification.Idle };

        var schedulable = JobScheduler.SchedulableJobs([newJob]);

        Assert.That(new Job[] { newJob }, Is.EquivalentTo(schedulable));
    }

    /// <summary>
    /// Arranges the scheduler in the following way:
    /// Jobs: ProductionJob 1 (Completed) -> ProductionJob 2 (Completed) -> ProductionJob 3 (Completed) -> CleanupJob (Waiting)
    /// 
    /// ResumeSlots: -empty-
    /// RunningSlot: -empty-
    /// CompletingSlots: CleanupJob
    /// </summary>
    private void ArrangeScheduledCleanUpAfterCompletedAndCanceledJobs()
    {
        var jobs = ArrangeCompletingJobWithTwoCanceledFollowUpsAndCleanUp();
        JobScheduler.JobUpdated(jobs[0], JobClassification.Completed);
    }
}
