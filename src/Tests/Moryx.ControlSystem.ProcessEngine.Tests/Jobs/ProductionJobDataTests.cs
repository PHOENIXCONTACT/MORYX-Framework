// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Processes;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class ProductionJobDataTests : JobDataTestBase
    {
        [Test(Description = "A not started job will be completed without producing parts.")]
        public void CompleteInitialJob()
        {
            // Arrange
            var jobData = GetProductionJob(1);

            // Act
            jobData.Complete();

            // Assert
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [Test(Description = "When a waiting job switches to dispatched and back, it should not create a new process but instead resume the old one.")]
        public void WaitingToDispatchedAndBack()
        {
            // Arrange
            var jobData = GetProductionJob(1);
            jobData.Ready();

            // Act
            jobData.Start();
            jobData.Stop();

            // On the second call the dispatcher resumes the previous process
            jobData.Start();

            // Assert
            DispatcherMock.Verify(d => d.StartProcess(jobData), Times.Exactly(2));
            DispatcherMock.Verify(d => d.Complete(jobData), Times.Exactly(1));
        }

        [TestCase(false, Description = "A started job will be completed without producing parts.")]
        [TestCase(true, Description = "A started job will be aborted without producing parts.")]
        public void CompleteWaitingJob(bool useAbort)
        {
            // Arrange
            var jobData = GetProductionJob(1);
            jobData.Ready();

            // Act
            if (useAbort)
                jobData.Abort();
            else
                jobData.Complete();

            // Assert
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [TestCase(ProcessState.Success, 1, 0, Description = "Produce only one part. The job will not be running but instantly completing.")]
        [TestCase(ProcessState.Failure, 0, 1, Description = "Produce only one part. The job will not be running but instantly completing.")]
        public void CompleteJobWithOnePart(ProcessState result, int expectedSuccess, int expectedFailed)
        {
            var jobData = GetProductionJob(1);

            // Will start first process on job
            var processData = RunFirstProcess(jobData);

            // Because of one part, the job is completing and waiting for result
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completing));
            Assert.That(jobData.Job.Running.Count, Is.EqualTo(1));

            // Process will be finished with success or failure
            SetStateOnProcess(jobData, processData, result);

            // Assert
            AssertCompletedJob(jobData, expectedSuccess, expectedFailed, 0);
        }

        [Test(Description = "Will start a job and produces multiple parts. The job should be completed.")]
        public void CompleteJobWithManyParts()
        {
            var jobData = GetProductionJob(2);

            // Run first process
            var firstProcess = RunFirstProcess(jobData);

            // Because of multiple parts, the job is now running
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Running));

            // The next process should be dispatched
            DispatcherMock.Verify(d => d.StartProcess(jobData), Times.Exactly(2));

            // Add next process
            var secondProcess = AddProcessOnJob(jobData);

            // Now the first process finishes
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            // No new process should be dispatched because of last parts
            DispatcherMock.Verify(d => d.StartProcess(jobData), Times.Exactly(2));

            // Second process will run now
            SetStateOnProcess(jobData, secondProcess, ProcessState.Running);

            // Job should be completing now
            Assert.That(JobClassification.Completing, Is.EqualTo(jobData.Classification));
            Assert.That(jobData.Job.Running.Count, Is.EqualTo(1));

            // Second process success
            SetStateOnProcess(jobData, secondProcess, ProcessState.Success);

            // Assert
            AssertCompletedJob(jobData, 2, 0, 0);
        }

        [Test(Description = "Creates and runs a job which is endless. Only a complete call will complete the job.")]
        public void CompleteEndlessJob()
        {
            // Arrange
            var jobData = GetProductionJob(0);

            // Run and finish first process
            var firstProcess = RunFirstProcess(jobData);
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            // Lets produce some parts
            for (int i = 0; i < 8; i++)
            {
                var processData = AddProcessOnJob(jobData);
                SetStateOnProcess(jobData, processData, ProcessState.Running);
                SetStateOnProcess(jobData, processData, ProcessState.Success);
            }

            // Add last process
            var lastProcessData = AddProcessOnJob(jobData);
            SetStateOnProcess(jobData, lastProcessData, ProcessState.Running);

            // Act & Assert
            // Complete the job
            jobData.Complete();

            // Job is waiting for the last part to complete
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completing));

            // Complete last process
            SetStateOnProcess(jobData, lastProcessData, ProcessState.Success);

            // Assert
            AssertCompletedJob(jobData, 10, 0, 0);
        }

        [Test(Description = "A running job will be stopped so it should be go back to waiting. " +
                            "An example of a stop is a new prioritization of the production order.")]
        public void StopRunningJob()
        {
            // Arrange
            var jobData = GetProductionJob(10);

            // Run and finish first process
            var firstProcess = RunFirstProcess(jobData);
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            var secondProcess = AddProcessOnJob(jobData);
            SetStateOnProcess(jobData, secondProcess, ProcessState.Running);

            // Act
            // Stop the job. All running parts should be produced
            jobData.Stop();

            // Finish last part
            SetStateOnProcess(jobData, secondProcess, ProcessState.Success);

            // Amount not reached, no running process - back to waiting job
            jobData.Interrupt();

            // Assert
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Idle));
        }

        [Test(Description = "A running job will be interrupted.")]
        public void InterruptRunningJob()
        {
            // Arrange
            var jobData = GetProductionJob(10);

            // Run and finish first process
            var firstProcess = RunFirstProcess(jobData);
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            var secondProcess = AddProcessOnJob(jobData);

            // Act
            jobData.Interrupt();

            // Proces controller will interrupt the process as well
            SetStateOnProcess(jobData, secondProcess, ProcessState.Interrupted);

            // Assert
            // No running processes anymore. So the job should be startable again
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Idle));
        }

        [Test(Description = "A completing job will be interrupted")]
        public void InterruptCompletingJob()
        {
            // Arrange
            var jobData = GetProductionJob(3);

            // Run and finish first process
            var firstProcess = RunFirstProcess(jobData);
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            var secondProcess = AddProcessOnJob(jobData);
            SetStateOnProcess(jobData, secondProcess, ProcessState.Running);
            var thirdProcess = AddProcessOnJob(jobData);
            SetStateOnProcess(jobData, thirdProcess, ProcessState.Running);

            // Act
            jobData.Interrupt();
            thirdProcess.State = ProcessState.Interrupted; // Already set interrupted on both jobs
            SetStateOnProcess(jobData, secondProcess, ProcessState.Interrupted);
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completing)); // Make sure job awaits all events
            SetStateOnProcess(jobData, thirdProcess, ProcessState.Interrupted);

            // Assert
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Idle));
            Assert.DoesNotThrow(() => jobData.Interrupt(), "Interrupt an interrupted job should be okay");
        }

        [Test(Description = "A completing job will be interrupted")]
        public void CompletingInterruptingCompletes()
        {
            // Arrange
            var jobData = GetProductionJob(2);

            // Run and finish first process
            var firstProcess = RunFirstProcess(jobData);
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);
            var secondProcess = AddProcessOnJob(jobData);
            SetStateOnProcess(jobData, secondProcess, ProcessState.Running);

            // Act
            jobData.Interrupt();
            SetStateOnProcess(jobData, secondProcess, ProcessState.Success);

            // Assert
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [Test(Description = "A running job will be aborted.")]
        public void AbortRunningJob()
        {
            // Arrange
            var jobData = GetProductionJob(10);

            // Run and finish first process
            var firstProcess = RunFirstProcess(jobData);
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            var secondProcess = AddProcessOnJob(jobData);

            // Act
            jobData.Abort();

            // ProcessController will discard the process as well
            SetStateOnProcess(jobData, secondProcess, ProcessState.Discarded);

            // Assert
            // No running processes anymore. So the job should be startable again
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        [Test(Description = "Cleanup a running job after ControlSystem start sets the job state to completed")]
        public void CleanupRunningJobSetsJobStateToComplete()
        {
            // Arrange
            var jobData = GetProductionJob(5);

            // Create two running processes
            var firstProcess = RunFirstProcess(jobData);
            var secondProcess = AddProcessOnJob(jobData);

            // Finish the first running process correctly
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            // Assume the ControlSystem was restarted.
            // Everything that is still running now should be cleaned up.
            jobData.Load();
            jobData.Ready();
            jobData.Start();

            // Act
            // The Dispatcher will set the process state to cleaning up,
            // and the ActivityProvider will then transit it to broken.
            // At the end it will have reached the state failure
            SetStateOnProcess(jobData, secondProcess, ProcessState.Failure);

            // Assert
            // Check that all failed processes have been removed, and success and failure have been correctly counted.
            AssertCompletedJob(jobData, 1, 1, 0);
        }

        [Test(Description = "Aborting a job that is already in CleaningUp should set the state of the job to completed")]
        public void AbortJobInCleaningUpStateSetsJobStateToComplete()
        {
            // Arrange
            var jobData = GetProductionJob(5);

            // Create two running processes
            var firstProcess = RunFirstProcess(jobData);
            AddProcessOnJob(jobData);

            // Finish the first running process correctly
            SetStateOnProcess(jobData, firstProcess, ProcessState.Success);

            // Assume the ControlSystem was restarted.
            jobData.Load();
            jobData.Ready();
            jobData.Start();

            // Act
            // The user grows impatient and triggers an abort.
            // The job will be set to discarding and will try to clean-up as good as possible
            jobData.Abort();
            // On the next boot it vanishes
            jobData.Load();

            // Assert
            // Only the successful process will be counted. The still running process will be left to rott in that state until the database is scrapped.
            AssertCompletedJob(jobData, 1, 0, 1);
        }

        [Test(Description = "A success process could be a reworked process so the ReworkedCount must also be increased")]
        public void CountASuccessReworkedProcessOnItsCompletion()
        {
            // Arrange
            var jobData = GetProductionJob(100);

            var successProcess = RunFirstProcess(jobData);
            successProcess.Rework = true;

            // Act
            SetStateOnProcess(jobData, successProcess, ProcessState.Success);

            // Assert
            Assert.That(jobData.SuccessCount, Is.EqualTo(1), "There must be one success process");
            Assert.That(jobData.ReworkedCount, Is.EqualTo(1), "There must be one reworked process because the success process was reworked");
            Assert.That(jobData.FailureCount, Is.EqualTo(0), "There must be no failed processes");
        }

        [Test(Description = "A failure process could be a reworked process so the ReworkedCount must be increased")]
        public void CountAFailedReworkedProcessOnItsCompletion()
        {
            // Arrange
            var jobData = GetProductionJob(100);

            var failedProcess = RunFirstProcess(jobData);
            failedProcess.Rework = true;

            // Act
            SetStateOnProcess(jobData, failedProcess, ProcessState.Failure);

            // Assert
            Assert.That(jobData.SuccessCount, Is.EqualTo(0), "There must be no success processes");
            Assert.That(jobData.ReworkedCount, Is.EqualTo(1), "There must be one reworked process because the failure process was reworked");
            Assert.That(jobData.FailureCount, Is.EqualTo(1), "There must be one failed process");
        }

        [TestCase(ProcessState.CleaningUp)]
        [TestCase(ProcessState.Initial)]
        [TestCase(ProcessState.Interrupted)]
        [TestCase(ProcessState.Ready)]
        [TestCase(ProcessState.RemoveBroken)]
        [TestCase(ProcessState.Aborting)]
        [TestCase(ProcessState.Restored)]
        [TestCase(ProcessState.RestoredReady)]
        [TestCase(ProcessState.Running)]
        [TestCase(ProcessState.Stopping)]
        public void DoNotCountReworkedProcessInAnyOtherState(ProcessState state)
        {
            // Arrange
            var jobData = GetProductionJob(100);

            var failedProcess = RunFirstProcess(jobData);
            failedProcess.Rework = true;

            // Act
            SetStateOnProcess(jobData, failedProcess, state);

            // Assert
            Assert.That(jobData.ReworkedCount, Is.EqualTo(0), "There must be no reworked processes count in other states.");
        }

        /// <summary>
        /// Asserts a completed job.
        /// </summary>
        private static void AssertCompletedJob(IProductionJobData jobData, int expectedSuccess, int expectedFailed, int expectedRunning)
        {
            Assert.That(jobData.RunningProcesses.Count, Is.EqualTo(expectedRunning));
            Assert.That(jobData.SuccessCount, Is.EqualTo(expectedSuccess));
            Assert.That(jobData.FailureCount, Is.EqualTo(expectedFailed));
            Assert.That(jobData.Classification, Is.EqualTo(JobClassification.Completed));
        }

        /// <summary>
        /// Begins and starts the job. A new process will be added and running
        /// </summary>
        private ProcessData RunFirstProcess(IJobData jobData)
        {
            jobData.Ready();
            jobData.Start();

            // Check if process was dispatched to the Dispatcher
            DispatcherMock.Verify(d => d.StartProcess(jobData), Times.Once);

            // Add process to job
            var processData = AddProcessOnJob(jobData);

            // Process will be running now
            SetStateOnProcess(jobData, processData, ProcessState.Running);

            return processData;
        }

        /// <summary>
        /// Adds a new process to the job
        /// </summary>
        private static ProcessData AddProcessOnJob(IJobData jobData)
        {
            var processData = new ProcessData(new Process());
            jobData.AddProcess(processData);
            return processData;
        }

        /// <summary>
        /// Sets the state of a process and updates it on the job
        /// </summary>
        private static void SetStateOnProcess(IJobData jobData, ProcessData processData, ProcessState state)
        {
            processData.State = state;
            jobData.ProcessChanged(processData, state);
        }
    }
}

