// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.ControlSystem.TestTools;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    public abstract class SchedulerTestBase
    {
        protected IJobScheduler JobScheduler;
        protected Mock<IJobList> JobListMock;

        protected IProductionRecipe Recipe;

        protected Job ScheduledJob;

        [SetUp]
        public void SetUp()
        {
            Recipe = DummyRecipe.BuildRecipe();
            JobListMock = new Mock<IJobList>();
            JobScheduler = CreateScheduler();
            JobScheduler.Scheduled += OnJobScheduled;
            JobScheduler.Start();
        }

        protected abstract IJobScheduler CreateScheduler();

        [TearDown]
        public void Clear()
        {
            ScheduledJob = null;
        }

        private void OnJobScheduled(object sender, Job e)
        {
            ScheduledJob = e;
        }

        [Test]
        public void ShouldNotAssignOrStartAWaitingJobIfThereAreDependencies()
        {
            // Arrange
            var job = new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            };
            var otherJobs = CreateProductionJobs(4);

            // Act
            var setup = new Job(new SetupRecipe
            {
                TargetRecipe = Recipe,
                Execution = SetupExecution.BeforeProduction
            }, 1)
            {
                Classification = JobClassification.Waiting
            };
            JobListMock.Setup(j => j.Previous(job)).Returns(setup);
            JobScheduler.JobsReady([setup, job]);
            var slots = JobScheduler.SchedulableJobs(otherJobs).ToArray();

            // Assert
            Assert.That(ScheduledJob, Is.Not.EqualTo(job));
        }

        [Test]
        public void JobAbortedDuringSetup()
        {
            // Arrange
            Recipe.Id = 42;
            var setup = new Job(new SetupRecipe
            {
                Id = 16,
                TargetRecipe = Recipe,
                Execution = SetupExecution.BeforeProduction
            }, 1)
            {
                Classification = JobClassification.Waiting
            };
            var waiting = new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            };
            var cleanup = new Job(new SetupRecipe
            {
                Id = 17,
                TargetRecipe = Recipe,
                Execution = SetupExecution.AfterProduction
            }, 1)
            {
                Classification = JobClassification.Waiting
            };
            var slotAvailableCalled = false;
            JobScheduler.SlotAvailable += (sender, args) => slotAvailableCalled = true;
            JobListMock.Setup(j => j.Previous(waiting)).Returns(setup);
            JobListMock.Setup(j => j.Forward(setup)).Returns([waiting]);
            JobListMock.Setup(j => j.Previous(cleanup)).Returns(waiting);
            JobListMock.Setup(j => j.Forward(waiting)).Returns([cleanup]);

            // Act
            JobScheduler.JobsReady([setup, waiting, cleanup]);
            Assert.That(setup, Is.EqualTo(ScheduledJob));
            setup.Classification = JobClassification.Running;
            JobScheduler.JobUpdated(setup, JobClassification.Running);
            // Abort production
            waiting.Classification = JobClassification.Completed;
            JobListMock.Setup(j => j.Next(waiting)).Returns(cleanup);
            JobListMock.Setup(j => j.Forward(setup)).Returns([cleanup]);
            JobScheduler.JobUpdated(waiting, JobClassification.Completed);
            // Setup manager aborts setup
            setup.Classification = JobClassification.Completing;
            JobListMock.Setup(j => j.Next(setup)).Returns(cleanup);
            JobScheduler.JobUpdated(setup, JobClassification.Completing);
            // Clean-up MUST NOT be scheduled
            Assert.That(ScheduledJob, Is.Not.EqualTo(cleanup));
            setup.Classification = JobClassification.Completed;
            JobScheduler.JobUpdated(setup, JobClassification.Completed);
            // Clean-up should be scheduled and completed
            Assert.That(cleanup, Is.EqualTo(ScheduledJob));
            cleanup.Classification = JobClassification.Completed;
            JobScheduler.JobUpdated(cleanup, JobClassification.Completed);
            Assert.That(slotAvailableCalled);

            // Assert
            // Make sure the scheduler is ready for another job
            var anotherJob = new Job(Recipe, 1)
            {
                Classification = JobClassification.Idle
            };
            var schedulable = JobScheduler.SchedulableJobs([anotherJob]);
            Assert.That(schedulable.FirstOrDefault(), Is.EqualTo(anotherJob));
        }

        [Test]
        public void ShouldStartTheNextWaitingAfterAJobIsCompleting()
        {
            // Arrange
            Recipe.Id = 42;
            var job = new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            };
            var waiting = new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            };
            JobListMock.Setup(j => j.Forward(job)).Returns([waiting]);

            // Act
            JobScheduler.SchedulableJobs([job, waiting]);
            JobScheduler.JobsReady([job, waiting]);
            job.Classification = JobClassification.Completing;
            JobScheduler.JobUpdated(job, JobClassification.Completing);

            // Assert
            Assert.That(waiting, Is.EqualTo(ScheduledJob));
        }

        [Test]
        public void ShouldNotStartNextInitialAfterAJobIsCompleting()
        {
            // Arrange
            Recipe.Id = 42;

            var job = new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            };

            var idleJob = new Job(Recipe, 1)
            {
                Classification = JobClassification.Idle
            };
            JobListMock.Setup(j => j.Forward(It.IsAny<Job>())).Returns([idleJob]);

            // Act
            JobScheduler.JobsReady([job]);
            job.Classification = JobClassification.Completing;
            JobScheduler.JobUpdated(job, JobClassification.Completing);

            // Assert
            Assert.That(ScheduledJob, Is.Not.EqualTo(idleJob));
        }

        [Test]
        public void ShouldRemoveDependenciesIfAJobIsCompletingAndStartTheNextOne()
        {
            // Arrange
            var completedJob = new Job(Recipe, 1)
            {
                Classification = JobClassification.Idle
            };
            var otherJob = new Job(Recipe, 1)
            {
                Classification = JobClassification.Idle
            };
            JobScheduler.SchedulableJobs([completedJob, otherJob]);

            completedJob.Classification = JobClassification.Waiting;
            JobListMock.Setup(j => j.Previous(otherJob)).Returns(completedJob);
            JobScheduler.JobsReady([completedJob, otherJob]);
            completedJob.Classification = JobClassification.Completing;

            otherJob.Classification = JobClassification.Waiting;
            JobListMock.Setup(j => j.Next(completedJob)).Returns(otherJob);
            JobListMock.Setup(j => j.Forward(completedJob)).Returns([otherJob]);

            // Act
            JobScheduler.JobUpdated(completedJob, JobClassification.Completing);

            // Assert
            Assert.That(otherJob, Is.EqualTo(ScheduledJob));
        }

        //--

        [Test]
        public void ShouldRemoveJobFromSlotAndRaiseSlotAvailable()
        {
            // Arrange
            var slotAvailableCalled = false;
            var completedJob = new Job(Recipe, 1)
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
            JobListMock.Setup(j => j.Previous(cleanup)).Returns(completedJob);
            JobScheduler.JobsReady([completedJob, cleanup]);

            completedJob.Classification = JobClassification.Completed;
            JobListMock.Setup(j => j.Next(completedJob)).Returns(cleanup);
            JobListMock.Setup(j => j.Forward(completedJob)).Returns([cleanup]);

            JobScheduler.SlotAvailable += (sender, args) => slotAvailableCalled = true;

            // Act
            JobScheduler.JobUpdated(completedJob, JobClassification.Completed);
            cleanup.Classification = JobClassification.Completed;
            JobListMock.Setup(j => j.Next(cleanup)).Returns((Job)null);
            JobScheduler.JobUpdated(cleanup, JobClassification.Completed);

            // Assert
            Assert.That(slotAvailableCalled, "The slot available event was not called.");
        }

        protected void ResumeJobsAfterRestart(int expectedSchedulable)
        {
            // Arrange: 2 Interrupted jobs, first running and second completing
            var runningInterrupted = new EngineJob(DummyRecipe.BuildRecipe(2), 20)
            {
                Classification = JobClassification.Idle,
                SuccessCount = 10,
                Running = { new Process { Id = 1 } }
            };
            var runningCleanup = new Job(new SetupRecipe
            {
                Execution = SetupExecution.AfterProduction,
                TargetRecipe = (IProductRecipe)runningInterrupted.Recipe
            }, 1)
            {
                Classification = JobClassification.Waiting
            };
            var completingInterrupted = new EngineJob(DummyRecipe.BuildRecipe(3), 20)
            {
                Classification = JobClassification.Idle,
                SuccessCount = 10,
                Running = { new Process { Id = 1 } }
            };
            var completingInterruptedFollowUp = new EngineJob(DummyRecipe.BuildRecipe(3), 5)
            {
                Classification = JobClassification.Idle,
            };
            var completingCleanup = new Job(new SetupRecipe
            {
                Execution = SetupExecution.AfterProduction,
                TargetRecipe = (IProductRecipe)completingInterrupted.Recipe
            }, 1)
            {
                Classification = JobClassification.Waiting
            };
            JobScheduler.Scheduled += (sender, job) =>
            {
                if (job == runningInterrupted)
                    runningInterrupted.Classification = JobClassification.Running;
                else if (job == completingInterrupted)
                    completingInterrupted.Classification = JobClassification.Completing;
            };

            // Act
            var schedulable = JobScheduler.SchedulableJobs([runningInterrupted, completingInterrupted, completingInterruptedFollowUp]).ToList();
            runningInterrupted.Classification = JobClassification.Waiting;
            completingInterrupted.Classification = JobClassification.Waiting;
            JobScheduler.JobsReady(new List<Job> { runningInterrupted, runningCleanup, completingInterrupted, completingCleanup });
            JobScheduler.JobUpdated(completingInterrupted, JobClassification.Completing);
            JobScheduler.JobUpdated(runningInterrupted, JobClassification.Running);

            // Assert
            Assert.That(schedulable.Count, Is.EqualTo(expectedSchedulable));
            Assert.That(runningInterrupted.Classification, Is.EqualTo(JobClassification.Running));
            Assert.That(completingInterrupted.Classification, Is.EqualTo(JobClassification.Completing));
        }

        protected static IReadOnlyList<Job> CreateProductionJobs(int amount)
        {
            var jobs = new Job[amount];
            for (var i = 0; i < amount; i++)
            {
                var recipeId = i + 2;
                jobs[i] = new Job(DummyRecipe.BuildRecipe(recipeId), 1);
            }
            return jobs;
        }
    }
}
