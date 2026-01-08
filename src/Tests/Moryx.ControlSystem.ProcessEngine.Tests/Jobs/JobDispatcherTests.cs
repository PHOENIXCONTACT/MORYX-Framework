// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moq;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.ProcessEngine.Jobs.Production;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class JobDispatcherTests
    {
        private Mock<IProcessController> _processControllerMock;
        private Mock<IJobDataList> _jobListMock;
        private JobDispatcher _jobDispatcher;
        private Mock<IJobData> _jobDataMock;
        private ProductionRecipe _recipe;

        [SetUp]
        public void Setup()
        {
            _processControllerMock = new Mock<IProcessController>();
            _jobListMock = new Mock<IJobDataList>();
            _jobDispatcher = new JobDispatcher
            {
                JobList = _jobListMock.Object,
                ProcessController = _processControllerMock.Object
            };
            _jobDataMock = new Mock<IJobData>();
            _jobDataMock.SetupGet(m => m.Recipe).Returns(DummyRecipe.BuildRecipe());
            _jobDataMock.SetupGet(m => m.AllProcesses).Returns(new List<ProcessData>());
            _jobDataMock.Setup(m => m.AddProcesses(It.IsAny<IReadOnlyList<ProcessData>>()))
                .Callback<IReadOnlyList<ProcessData>>(processes => { _jobDataMock.SetupGet(m => m.RunningProcesses).Returns(processes); });

            _recipe = DummyRecipe.BuildRecipe();
        }

        [Test(Description = "Starts a process on the dispatcher. The process controller should dispatch a new process for the given job.")]
        public void StartProcess()
        {
            // Arrange
            _processControllerMock.Setup(pc => pc.Start(It.IsAny<ProcessData>()));

            // Act
            _jobDispatcher.StartProcess(_jobDataMock.Object);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                _processControllerMock.Verify(pc => pc.Start(It.IsAny<ProcessData>()), Times.Once);
                _jobDataMock.Verify(jobData => jobData.AddProcess(It.IsAny<ProcessData>()), Times.Once);
            });
        }

        [Test(Description = "Resumes a job. The dispatcher should add the running processes back to the job.")]
        public void Resume()
        {
            // Arrange
            var runningProcesses = new List<ProcessData> { new(_recipe.CreateProcess()) };
            _processControllerMock.Setup(pc => pc.LoadProcesses(_jobDataMock.Object)).Returns(runningProcesses);

            // Act
            _jobDispatcher.LoadProcesses(_jobDataMock.Object);
            _jobDispatcher.Resume(_jobDataMock.Object);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                _jobDataMock.Verify(jobData => jobData.AddProcesses(runningProcesses), Times.Once);
                _processControllerMock.Verify(pc => pc.Resume(runningProcesses), Times.Once);
            });
        }

        [Test(Description = "Completes a job. The dispatcher should interrupt unstarted processes.")]
        public void Complete()
        {
            // Arrange
            var successProcess = new ProcessData(_recipe.CreateProcess());
            successProcess.AddActivity(new ActivityData(new MountActivity() { Tracing = { Started = DateTime.Now.AddMinutes(-2), Completed = DateTime.Now } }));
            successProcess.State = ProcessState.Success;
            var idleProcess = new ProcessData(_recipe.CreateProcess());
            idleProcess.AddActivity(new ActivityData(new MountActivity() { Tracing = { Started = DateTime.Now.AddMinutes(-2) } }));
            idleProcess.State = ProcessState.EngineStarted;

            var unstartedProcess = new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Initial };
            var jobProcesses = new[] { successProcess, idleProcess, unstartedProcess };
            _jobDataMock.SetupGet(jobData => jobData.RunningProcesses).Returns(() => jobProcesses);

            // Act
            _jobDispatcher.Complete(_jobDataMock.Object);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                // Only the unstarted process should be interrupted here
                _processControllerMock.Verify(pc => pc.Interrupt(new[]
                {
                    unstartedProcess
                }, false), Times.Once);
            });
        }

        [Test(Description = "Aborts a job. The dispatcher should interrupt all processes with an final unmount.")]
        public void Abort()
        {
            // Arrange
            var jobProcesses = new[] { new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Initial } };
            _jobDataMock.SetupGet(jobData => jobData.RunningProcesses).Returns(() => jobProcesses);

            // Act
            _jobDispatcher.Abort(_jobDataMock.Object);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                // The process controller should interrupt but with an final unmount
                _processControllerMock.Verify(pc => pc.Interrupt(jobProcesses, true), Times.Once);
            });
        }

        [Test(Description = "Cleans a job, that was previously left all dressed up and nowhere to go.")]
        public void Cleanup()
        {
            // Arrange
            var recipeMock = new Mock<ProductionRecipe>();
            var jobData = new ProductionJobData(recipeMock.Object, 10);
            jobData.ProgressChanged += (sender, args) => { };

            var jobProcesses = new[] {
                // This is only a randomly chosen subset of processes. Feel free to change the list...
                new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Restored },
                new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Restored },
                new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Restored },
                new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Restored },
                new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Restored },
                new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Restored }};
            _processControllerMock.Setup(pc => pc.LoadProcesses(jobData)).Returns(jobProcesses);

            // Act
            _jobDispatcher.LoadProcesses(jobData);
            _jobDispatcher.Cleanup(jobData);

            // Assert
            Assert.That(jobData.RunningProcesses, Is.EqualTo(jobProcesses));
        }

        [Test(Description = "Interrupts a job. The dispatcher should interrupt all processes without an final unmount.")]
        public void Interrupt()
        {
            // Arrange
            var jobProcesses = new[] { new ProcessData(_recipe.CreateProcess()) { State = ProcessState.Initial } };
            _jobDataMock.SetupGet(jobData => jobData.RunningProcesses).Returns(() => jobProcesses);

            // Act
            _jobDispatcher.Interrupt(_jobDataMock.Object);

            // Assert
            Assert.DoesNotThrow(delegate
            {
                // The process controller should interrupt but without an unmount
                _processControllerMock.Verify(pc => pc.Interrupt(jobProcesses, false), Times.Once);
            });
        }

        [Test(Description = "On stopping of dispatcher, all jobs should be interrupted.")]
        public void Stop()
        {
            // Arrange
            var firstJob = new Mock<IJobData>();
            var jobs = new List<IJobData>([firstJob.Object]);

            _jobListMock.Setup(list => list.GetEnumerator()).Returns(jobs.GetEnumerator());

            // Act
            _jobDispatcher.Stop();

            // Assert
            Assert.DoesNotThrow(delegate
            {
                // current jobs should be loaded from the job list
                _jobListMock.Verify(jobList => jobList.GetEnumerator(), Times.Once);

                // on current jobs the interrupt should be called
                firstJob.Verify(j => j.Interrupt(), Times.Once);
            });
        }
    }
}

