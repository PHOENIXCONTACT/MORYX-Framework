// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Setup;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.Recipes;
using Moryx.Notifications;
using Moryx.Workplans;
using NUnit.Framework;
using System.Linq;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class SetupJobDataTests : JobDataTestBase
    {
        private SetupJobData _setupJobData;

        public override void Setup()
        {
            base.Setup();

            _setupJobData = GetSetupJob();
            _setupJobData.Ready();
        }

        [TestCase(Transition.Load, Description = "Should be direclty completed because there is nothing to load in initial")]
        [TestCase(Transition.Interrupt, Description = "Should be directly completed because there is nothing to interrupt in initial")]
        public void CompleteInitialSetupJob(Transition calledTransition)
        {
            // Arrange
            // Nothing to arrange

            // Act
            ExecuteTransition(calledTransition, _setupJobData);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [TestCase(Transition.Load, Description = "Should be direclty completed because there is nothing to load in waiting")]
        [TestCase(Transition.Interrupt, Description = "Should be directly completed because there is nothing to interrupt in waiting")]
        public void CompleteWaitingSetupJob(Transition calledTransition)
        {
            //Arrange

            // Act
            ExecuteTransition(calledTransition, _setupJobData);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [Test(Description = "A waiting setup job should start a new process at the dispatcher if start was called")]
        public void ShouldStartANewProcess()
        {
            // Arrange

            // Act
            _setupJobData.Start();

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Running), "The setup job should running after the start");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once, "There should be one call to start a new process");
            });
        }

        [Test(Description = "A running setup job should be completed if the process was successfull finished")]
        public void ShouldCompleteAfterProcessSucceeded()
        {
            // Arrange
            _setupJobData.Start();

            // Act
            _setupJobData.ProcessChanged(new ProcessData(new Process()), ProcessState.Success);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed), "The setup job should be completed after a successfull process");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one call to start a new process which should be finished");
            });

        }

        [Test(Description = "The setup job should be marked as failed to know when to retry the setup")]
        public void ShouldSignalThatTheSetupAhouldBeRetriedAfterProcessFailed()
        {
            // Arrange
            _setupJobData.Start();

            // Act
            _setupJobData.ProcessChanged(new ProcessData(new Process()), ProcessState.Failure);

            // Assert
            Assert.That(_setupJobData.RecipeRequired, "Setup job should be marked as failed");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one call to start a new process which will be failed");
            });
        }

        [Test(Description = "The failed setup should be retried with a given recipe")]
        public void ShouldRetrySetupWithAGivenRecipe()
        {
            // Arrange
            _setupJobData.Start();
            _setupJobData.ProcessChanged(new ProcessData(new Process()), ProcessState.Failure);
            var oldDisabled = _setupJobData.Recipe.DisabledSteps.First();

            var recipe = new SetupRecipe
            {
                Workplan = _setupJobData.Recipe.Workplan
            };

            // Act
            _setupJobData.UpdateSetup(recipe);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Running), "Setup job should be running again to retry the setup with the given recipe");
            Assert.That(recipe, Is.SameAs(_setupJobData.Recipe), "The setup job should have the given recipe as the current recipe");
            Assert.That(_setupJobData.Recipe.DisabledSteps.Count, Is.EqualTo(1), "Disabled steps were not transferred to new setup");
            Assert.That(_setupJobData.Recipe.DisabledSteps.First(), Is.EqualTo(oldDisabled), "Disabled step does not match");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Exactly(2),
                    "There should be two start process calls. One for the first try and one for the second try");
            });
        }

        [Test(Description = "The failed setup should be retried only a number of times, and then the job should go into RetrySetupBlocked state")]
        public void ShouldRetrySetupOnlyLimitedTimesAndGoIntoRetrySetupBlocked()
        {

            // Arrange
            var setupProcess = new ProcessData(new Process());
            _setupJobData.Start();
            _setupJobData.AddProcess(setupProcess);
            _setupJobData.ProcessChanged(setupProcess, ProcessState.Failure);

            var recipe = new SetupRecipe
            {
                Name = "Just a test recipe",
                Workplan = new Workplan()
            };

            // Act
            for (var i = 0; i < ModuleConfig.DefaultSetupJobRetryLimit; i++)
            {
                _setupJobData.UpdateSetup(recipe);
                _setupJobData.ProcessChanged(new ProcessData(new Process()), ProcessState.Failure);
            }

            _setupJobData.UpdateSetup(recipe);

            // Assert
            Assert.That(_setupJobData.IsRetryLimitReached(), "The retry counter was not reached.");
            Assert.That(recipe, Is.SameAs(_setupJobData.Recipe), "The setup job should have the given recipe as the current recipe");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Exactly(4),
                    "There should be four start process calls. One for the first try and three for the retries");
            });
            Assert.DoesNotThrow(delegate
            {
                NotificationAdapterMock.Verify(d => d.Publish(It.IsAny<INotificationSender>(), It.IsAny<Notification>()), Times.Exactly(1),
                    "There should have been the publication of one notification");
            });
        }

        [Test(Description = "The failed setup should not be retried again if there is no recipe for the next try")]
        public void ShouldCompleteRetrySetupIfNoRecipeIsGiven()
        {
            // Arrange
            _setupJobData.Start();
            _setupJobData.ProcessChanged(new ProcessData(new Process()), ProcessState.Failure);

            // Act
            _setupJobData.UpdateSetup(null);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed),
                "There setup job should be completed because there is no recipe given for the second try");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be only one start process call for the first try");
            });
        }

        [Test(Description = "Setup jobs should be completed if a load request occurs.")]
        public void ShouldCompleteSetupsIfLoadOccurs()
        {
            // Arrange
            var initialJob = GetSetupJob();
            var waitingJob = GetWaitingSetupJob();
            var runningJob = GetRunningSetupJob();
            var interruptingJob = GetInterruptingSetupJob();
            var abortingJob = GetAbortingSetupJob();
            var requestRecipeJob = GetRequestRecipeSetupJob();
            var retrySetupBlockedJob = GetRetrySetupBlockedSetupJob();

            // Act
            initialJob.Load();
            waitingJob.Load();
            runningJob.Load();
            interruptingJob.Load();
            abortingJob.Load();
            requestRecipeJob.Load();
            retrySetupBlockedJob.Load();

            // Assert
            Assert.That(initialJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in initial state should be completed");
            Assert.That(waitingJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in waiting state should be completed");
            Assert.That(runningJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in running state should be completed");
            Assert.That(interruptingJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in interrupting state should be completed");
            Assert.That(abortingJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in aborting state should be completed");
            Assert.That(requestRecipeJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in request recipe state should be completed");
            Assert.That(retrySetupBlockedJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in retry setup blocked state should be completed");
        }

        [Test(Description = "Setup jobs that are in Initial, Waiting, RequestRecipe and RetrySetupBlocked state should be completed if an interrupt request occurs.")]
        public void ShouldCompleteSetupsIfInterruptOccurs()
        {
            // Arrange
            var initialJob = GetSetupJob();
            var waitingJob = GetWaitingSetupJob();
            var requestRecipeJob = GetRequestRecipeSetupJob();
            var retrySetupBlockedJob = GetRetrySetupBlockedSetupJob();

            // Act
            initialJob.Interrupt();
            waitingJob.Interrupt();
            requestRecipeJob.Interrupt();
            retrySetupBlockedJob.Interrupt();

            // Assert
            Assert.That(initialJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in initial state should be completed");
            Assert.That(waitingJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in waiting state should be completed");
            Assert.That(requestRecipeJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in request recipe state should be completed");
            Assert.That(retrySetupBlockedJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in retry setup blocked state should be completed");
        }

        [Test(Description = "Setup jobs that are in Initial, Waiting, RequestRecipe and RetrySetupBlocked state should be completed if an interrupt request occurs.")]
        public void ShouldCompleteSetupsIfAbortOccurs()
        {
            // Arrange
            var initialJob = GetSetupJob();
            var waitingJob = GetWaitingSetupJob();
            var requestRecipeJob = GetRequestRecipeSetupJob();
            var retrySetupBlockedJob = GetRetrySetupBlockedSetupJob();

            // Act
            initialJob.Abort();
            waitingJob.Abort();
            requestRecipeJob.Abort();
            retrySetupBlockedJob.Abort();

            // Assert
            Assert.That(initialJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in initial state should be completed");
            Assert.That(waitingJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in waiting state should be completed");
            Assert.That(requestRecipeJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in request recipe state should be completed");
            Assert.That(retrySetupBlockedJob.Classification, Is.EqualTo(JobClassification.Completed), "The job in retry setup blocked state should be completed");
        }

        [TestCase(Transition.Start, Description = "The setup job should be stay in running if a start call occurs")]
        [TestCase(Transition.Stop, Description = "The setup job should be stay in running if a stop call occurs")]
        public void ShouldStayInRunning(Transition calledTransition)
        {
            // Arrange
            _setupJobData.Start();

            // Act
            ExecuteTransition(calledTransition, _setupJobData);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Running), "The setup job should be still running");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once, "There should be only one stat process call");
                DispatcherMock.Verify(d => d.Complete(It.IsAny<IJobData>()), Times.Never, "There should be no complete call during the running");
                DispatcherMock.Verify(d => d.Abort(It.IsAny<IJobData>()), Times.Never, "There should be no abort call during the running");
                DispatcherMock.Verify(d => d.Interrupt(It.IsAny<IJobData>()), Times.Never, "There should be no interrupt call during the running");
            });
        }

        [Test(Description = "A running setup job should be completed if a load occurs because something went wrong during a restart")]
        public void ShouldCompleteRunningJobOnLoad()
        {
            // Arrange
            _setupJobData.Start();

            // Act
            _setupJobData.Load();

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed), "The setup job should be completed on a load");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one start process call to run the setup");
            });
        }

        [Test(Description = "Setup job should interrupt the started process if an interrupt occurs")]
        public void ShouldInterruptProcess()
        {
            // Arrange
            _setupJobData.Start();

            // Act
            _setupJobData.Interrupt();

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completing), "Setup job should be in completing to interrupt the process");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one start process call to start the setup");
                DispatcherMock.Verify(d => d.Interrupt(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one interrupt call to interrupt the started process");
            });
        }

        [Test(Description = "Setup job cannot be aborted with a started process")]
        public void RunningSetupCannotAborted()
        {
            // Arrange

            // Act
            _setupJobData.Start();
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one start process call to start the setup");
            });
            _setupJobData.Abort();

            // Assert
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.Abort(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one start process call to start the setup");
            });
        }

        [TestCase(Transition.Start, Description = "The setup job should stay in completing if a start occurs because it is to late handle something")]
        [TestCase(Transition.Stop, Description = "The setup job should stay in completing if a stop occurs because it is to late handle something")]
        public void ShouldStayInCompleting(Transition calledTransition)
        {
            // Arrange
            _setupJobData.Start();
            _setupJobData.Interrupt();

            // Act
            ExecuteTransition(calledTransition, _setupJobData);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completing), "The setup job should still completing the process");
            Assert.DoesNotThrow(delegate
            {
                DispatcherMock.Verify(d => d.StartProcess(It.Is<IJobData>(j => j == _setupJobData)), Times.Once,
                    "There should be one start process call to start the setup");
                DispatcherMock.Verify(d => d.Complete(It.IsAny<IJobData>()), Times.Never, "There should be no complete call");
                DispatcherMock.Verify(d => d.Abort(It.IsAny<IJobData>()), Times.Never, "There should be no abort call");
                DispatcherMock.Verify(d => d.Interrupt(It.Is<IJobData>(j => j == _setupJobData)), Times.Once, "There should only one interrupt call");
            });
        }

        [Test(Description = "Setup job should be completed if a load occurs during the completing because something went wrong during the restart")]
        public void ShouldCompleteAnInterruptingJobIfLoadOccurs()
        {
            // Arrange
            _setupJobData.Start();
            _setupJobData.Interrupt();

            // Act
            _setupJobData.Load();

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed), "The setup job should be completed");
        }

        [TestCase(ProcessState.Success, Description = "The setup job should be completed if the process was successfully finished during the completing")]
        [TestCase(ProcessState.Failure, Description = "The setup job should be completed if the process was not successfully finished during the completing")]
        public void InterruptingSetupJobShouldCompleteIfTheProcessIsSomehowFinished(ProcessState processState)
        {
            // Arrange
            _setupJobData.Start();
            _setupJobData.Interrupt();

            // Act
            _setupJobData.ProcessChanged(new ProcessData(new Process()), processState);

            // Assert
            Assert.That(_setupJobData.Classification, Is.EqualTo(JobClassification.Completed), "The setup job should be completed");
        }

        [TestCase(Transition.Load, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.Ready, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.Start, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.Stop, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.Complete, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.Abort, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.Interrupt, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.UpdateSetup, Description = "A job in complete state job should log an error on a Load transition.")]
        [TestCase(Transition.UnblockRetry, Description = "A job in complete state job should log an error on a Load transition.")]
        public void ShouldLogError(Transition calledTransition)
        {
            // Arrange
            var completedJob = GetCompletedSetupJob();

            // Act
            ExecuteTransition(calledTransition, completedJob);

            // Assert
            Assert.That(LoggerMock.Invocations.Count, Is.EqualTo(1), message: $"An error should have been logged on the {nameof(calledTransition)}");
        }
    }
}
