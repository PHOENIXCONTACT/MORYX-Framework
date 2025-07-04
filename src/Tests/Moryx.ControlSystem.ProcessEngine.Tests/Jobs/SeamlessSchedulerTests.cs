// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Moq;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.Recipes;
using Moryx.ControlSystem.Setups;
using Moryx.ControlSystem.TestTools;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class SeamlessSchedulerTests : SchedulerTestBase
    {
        protected override IJobScheduler CreateScheduler()
        {
            var scheduler =  new SeamlessScheduler
            {
                JobList = JobListMock.Object
            };
            scheduler.Initialize(new SeamlessSchedulerConfig());
            return scheduler;
        }

        [Test(Description = "Should return the configured value for active jobs initially")]
        public void ProvidesASingleRunningSlot()
        {
            // Arrange
            var jobs = CreateProductionJobs(5);

            // Act
            var slots = JobScheduler.SchedulableJobs(jobs).ToArray();

            // Assert
            Assert.That(slots.Length, Is.EqualTo(1), "There should be three slots");
        }



        [Test]
        public void StartNextJobOnCompleting()
        {
            // Arrange
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

            // Act
            // The initial job is started and than completing BEFORE the other one is added
            JobScheduler.SchedulableJobs(new[] { job, waiting });
            JobScheduler.JobsReady(new[] { job, cleanup });
            job.Classification = JobClassification.Completing;
            JobScheduler.JobUpdated(job, JobClassification.Completing);
            JobScheduler.JobsReady(new[] { waiting });

            // Assert
            Assert.That(waiting, Is.EqualTo(ScheduledJob));
        }

        [Test]
        public void ResumeJobsAfterRestart()
        {
            ResumeJobsAfterRestart(2);
        }

        [Test]
        public void FollowUpsCancelled()
        {
            //Arrange
            Recipe.Id = 42;
            var jobs = new List<Job>();
            jobs.Add(new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            });
            jobs.Add(new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            });
            jobs.Add(new Job(Recipe, 1)
            {
                Classification = JobClassification.Waiting
            });
            jobs.Add(new Job(new SetupRecipe
            {
                Execution = SetupExecution.AfterProduction,
                TargetRecipe = Recipe
            }, 1){Classification = JobClassification.Waiting});

            JobListMock.Setup(j => j.Previous(It.IsAny<Job>())).Returns<Job>(j =>
            {
                var index = jobs.IndexOf(j) - 1;
                return index >= 0 ? jobs[index] : null;
            });
            JobListMock.Setup(j => j.Forward(It.IsAny<Job>())).Returns<Job>(j =>
            {
                var index = jobs.IndexOf(j) + 1;
                return jobs.Skip(index);
            });
            
            Job scheduled = null;
            JobScheduler.Scheduled += (sender, args) => scheduled = args;
            var slotAvailable = false;
            JobScheduler.SlotAvailable += (sender, args) => { slotAvailable = true; };

            // Act
            JobScheduler.JobsReady(jobs);
            Assert.That(jobs[0], Is.EqualTo(scheduled));
            jobs[0].Classification = JobClassification.Completing;
            JobScheduler.JobUpdated(jobs[0], JobClassification.Completing);
            Assert.That(jobs[1], Is.EqualTo(scheduled));
            // Now we abort second and third job while the scheduler still process events
            jobs[1].Classification = jobs[2].Classification = JobClassification.Completed;
            JobScheduler.JobUpdated(jobs[1], JobClassification.Completing);
            JobScheduler.JobUpdated(jobs[1], JobClassification.Completed);
            JobScheduler.JobUpdated(jobs[2], JobClassification.Completed);
            // Now the slot should fall back to the completing job and we finish it
            JobScheduler.JobUpdated(jobs[0], JobClassification.Completed);

            // Assert: Now the clean-up should have been started and a slot should be available
            Assert.That(jobs[3], Is.EqualTo(scheduled));
            Assert.That(slotAvailable);
            var job = new Job(DummyRecipe.BuildRecipe(43), 1)
            {
                Classification = JobClassification.Idle
            };
            var schedulable = JobScheduler.SchedulableJobs(new[] {job});
            Assert.That(schedulable.Any());
        }
    }
}
