// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moq;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Simulation;
using Moryx.Simulation.Examples.Messages;
using NUnit.Framework;
using System.Diagnostics;
using Activity = Moryx.AbstractionLayer.Activity;

namespace Moryx.Simulation.Tests
{
    internal class SimulationActivitiesTests : BaseTest
    {
        Mock<SimulatedDummyTestDriver> _assemblyDriverMock;
        Mock<SimulatedDummyTestDriver> _anotherAssemblyDriverMock;
        Mock<IProcess> _processMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _processMock = new Mock<IProcess>();

            _assemblyDriverMock = new Mock<SimulatedDummyTestDriver>() { DefaultValue = DefaultValue.Mock };
            _anotherAssemblyDriverMock = new Mock<SimulatedDummyTestDriver>() { DefaultValue = DefaultValue.Mock };
            _assemblyDriverMock.SetupGet(dr => dr.Cell).Returns(_assemblyCell);
            _assemblyDriverMock.SetupGet(dr => dr.Usages).Returns(new[] { _assemblyCell });
            _assemblyDriverMock.SetupGet(dr => dr.SimulatedState).Returns(SimulationState.Idle);
            _anotherAssemblyDriverMock.SetupGet(dr => dr.Cell).Returns(_anotherAssemblyCell);
            _anotherAssemblyDriverMock.SetupGet(dr => dr.Usages).Returns(new[] { _anotherAssemblyCell });
            _anotherAssemblyDriverMock.SetupGet(dr => dr.SimulatedState).Returns(SimulationState.Idle);

            _assemblyDriverMock.Setup(dr => dr.Send(It.IsAny<AssembleProductMessage>()))
                .Callback<object>(param =>
                {
                    var message = param as AssembleProductMessage;
                    _assemblyDriverMock.Setup(dr => dr.SimulatedState)
                    .Returns(SimulationState.Executing);

                    _assemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _assemblyDriverMock.Object, SimulationState.Executing);
                });

            _anotherAssemblyDriverMock.Setup(dr => dr.Send(It.IsAny<AssembleProductMessage>()))
            .Callback<object>(param =>
            {
                var message = param as AssembleProductMessage;
                _anotherAssemblyDriverMock.Setup(dr => dr.SimulatedState)
                .Returns(SimulationState.Executing);

                _anotherAssemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _anotherAssemblyDriverMock.Object, SimulationState.Executing);
            });

            _assemblyDriverMock.Setup(dr => dr.Send(It.IsAny<ReleaseWorkpieceMessage>()))
               .Callback<object>(param =>
               {
                   var message = param as AssembleProductMessage;
                   _assemblyDriverMock.Setup(dr => dr.SimulatedState)
                   .Returns(SimulationState.Idle);

                   _assemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _assemblyDriverMock.Object, SimulationState.Idle);
               });

            _anotherAssemblyDriverMock.Setup(dr => dr.Ready(It.IsAny<IActivity>()))
               .Callback<IActivity>(message =>
               {
                   _anotherAssemblyDriverMock.Setup(dr => dr.SimulatedState)
                   .Returns(SimulationState.Requested);

                   _anotherAssemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _anotherAssemblyDriverMock.Object, SimulationState.Requested);
                   _anotherAssemblyDriverMock.Raise(dr => dr.Received += null, _anotherAssemblyDriverMock.Object, new WorkpieceArrivedMessage { });
               });

            _assemblyDriverMock.Setup(dr => dr.Ready(It.IsAny<IActivity>()))
               .Callback<IActivity>(message =>
               {
                   _assemblyDriverMock.Setup(dr => dr.SimulatedState)
                   .Returns(SimulationState.Requested);

                   _assemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _assemblyDriverMock.Object, SimulationState.Requested);
                   _assemblyDriverMock.Raise(dr => dr.Received += null, _assemblyDriverMock.Object, new WorkpieceArrivedMessage { });
               });

            _assemblyDriverMock.Setup(dr => dr.Result(It.IsAny<SimulationResult>()))
               .Callback<SimulationResult>(message =>
               {
                   _assemblyDriverMock.Raise(dr => dr.Received += null, _assemblyDriverMock.Object, new AssemblyCompletedMessage { });
               });

            _anotherAssemblyDriverMock.Setup(dr => dr.Result(It.IsAny<SimulationResult>()))
               .Callback<SimulationResult>(message =>
               {
                   _anotherAssemblyDriverMock.Raise(dr => dr.Received += null, _anotherAssemblyDriverMock.Object, new AssemblyCompletedMessage { });
               });

            _assemblyCell.Driver = _assemblyDriverMock.Object;
            _anotherAssemblyCell.Driver = _anotherAssemblyDriverMock.Object;

            //resource management
            _resourceManagementMock.Setup(rm => rm.GetAllResources(It.IsAny<Func<ISimulationDriver, bool>>()))
                .Returns(new[] { _assemblyDriverMock.Object, _anotherAssemblyDriverMock.Object });


            _processControlMock.SetupSequence(pc => pc.Targets(It.IsAny<IProcess>()))
                 .Returns(new[] { _assemblyCell, _anotherAssemblyCell })
                 .Returns(new ICell[0])
                 .Returns(new[] { _assemblyCell, _anotherAssemblyCell })
                 .Returns(new[] { _assemblyCell, _anotherAssemblyCell })
                 .Returns(new[] { _assemblyCell, _anotherAssemblyCell });

            _processControlMock.Setup(pc => pc.Targets(It.IsAny<IActivity>()))
                 .Returns<IActivity>(activity => _activityTargets[activity]);
        }

        [Test]
        public void Assembly1_and_AnotherAssembly_should_both_receive_ready_to_work()
        {
            //arange
            var activity1 = new AssemblyActivity();
            var activity2 = new AssemblyActivity();
            Arrange(3, _assemblyCell, _anotherAssemblyCell, activity1, activity2);
            _processSimulator.Start();

            //act
            RaiseActivityUpdated(activity1, ControlSystem.Processes.ActivityProgress.Ready);
            RaiseActivityUpdated(activity2, ControlSystem.Processes.ActivityProgress.Ready);
            //wait
            Thread.Sleep(100);

            //assert
            //check if both cells received a ready to work from the simulator
            _assemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<IActivity>()), Times.Once);
            _anotherAssemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<IActivity>()), Times.Once);
        }

        [Test]
        public void TargetsCellsOrder_ShouldBeIdenticalToDriversOrder()
        {
            //arange
            _processSimulator.Start();
            var activity1 = new AssemblyActivity();
            var activity2 = new AssemblyActivity();
            Arrange(3, _assemblyCell, _anotherAssemblyCell, activity1, activity2);

            //Act
            RaiseActivityUpdated(activity1, ControlSystem.Processes.ActivityProgress.Ready);

            //Assert 
            _anotherAssemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<IActivity>()), Times.Once);
            _assemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<IActivity>()), Times.Never);
        }

        [Test]
        public void ActivityReady_NextTargetIsLastTarget_ShouldSkipMovement()
        {
            // Arrange
            var processId = 42;

            var completedActivity = new AssemblyActivity();
            completedActivity.Tracing.Started = DateTime.Now;
            completedActivity.Result = new ActivityResult();
            completedActivity.Tracing.ResourceId = _assemblyCell.Id;
            CreateActivity(completedActivity, _processMock.Object, new[] { _assemblyCell });

            var readyActivity = new AssemblyActivity();
            readyActivity.Id = 1337;
            CreateActivity(readyActivity, _processMock.Object, new[] { _assemblyCell });

            _processMock.Setup(p => p.GetActivities(It.IsAny<Func<IActivity, bool>>()))
                .Returns(new[] { completedActivity, readyActivity });

            _processMock.SetupGet(x => x.Id).Returns(processId);
            _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()))
                .Returns(completedActivity);
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { _processMock.Object });

            // Act
            _processSimulator.Start();

            // Assert
            _assemblyDriverMock.Verify(d => d.Ready(It.Is<Activity>(a => a.Id == readyActivity.Id)), Times.Once);
        }

        [Test]
        public void CompleteMovement_ActivityAlreadyStarted_ShouldNotCallReady()
        {
            // Arrange
            _moduleConfig.MovingDuration = 1;
            var processId = 42;
            var completedActivity = new AssemblyActivity();
            completedActivity.Result = new ActivityResult();
            var readyActivity = new AssemblyActivity();

            _processMock.Setup(p => p.GetActivities(It.IsAny<Func<IActivity, bool>>()))
                .Returns(new[] { readyActivity });
            _processMock.Setup(p => p.GetActivities())
                .Returns(new[] { completedActivity, readyActivity });
            _processMock.SetupGet(x => x.Id).Returns(processId);
            _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));

            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { _processMock.Object });
            CreateActivity(readyActivity, _processMock.Object, new[] { _anotherAssemblyCell });

            // Act
            _processSimulator.Start();
            readyActivity.Tracing.Started = DateTime.Now;
            Thread.Sleep(10);

            // Assert
            _anotherAssemblyDriverMock.Verify(d => d.Ready(It.IsAny<Activity>()), Times.Never);
        }

        [Test]
        public void CompleteActivity_ActivityAlreadyCompleted_ShouldNotCallResult()
        {
            // Arrange
            _moduleConfig.DefaultExecutionTime = 50;
            var processId = 42;
            var runningActivity = new AssemblyActivity();
            runningActivity.Tracing.Started = DateTime.Now;
            runningActivity.Tracing.ResourceId = _assemblyCell.Id;
            CreateActivity(runningActivity, _processMock.Object, new[] { _assemblyCell });

            _processMock.Setup(p => p.GetActivities(It.IsAny<Func<IActivity, bool>>()))
                .Returns(new[] { runningActivity });

            _processMock.SetupGet(x => x.Id).Returns(processId);
            _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { _processMock.Object });

            // Act
            _processSimulator.Start();
            runningActivity.Tracing.Completed = DateTime.Now;
            Thread.Sleep(100);

            // Assert
            _assemblyDriverMock.Verify(d => d.Result(It.IsAny<SimulationResult>()), Times.Never);
        }

        [Test]
        public void SimulateReady_ThrowsException_ShouldNotDisruptModule()
        {
            // Arrange
            _moduleConfig.DefaultExecutionTime = 1;
            var processId = 42;
            var readyActivity = new AssemblyActivity();
            CreateActivity(readyActivity, _processMock.Object, new[] { _assemblyCell });

            _processMock.Setup(p => p.GetActivities(It.IsAny<Func<IActivity, bool>>()))
                .Returns(new[] { readyActivity });

            _processMock.SetupGet(x => x.Id).Returns(processId);
            _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { _processMock.Object });

            var exception = new Exception("Test");
            _assemblyDriverMock.Setup(d => d.Ready(It.IsAny<Activity>())).Throws(exception);

            // Act
            _processSimulator.Start();
            Thread.Sleep(100);

            // Assert
            Assert.That(_parallelOperations.ScheduledExecutionExceptions().Any(), Is.False);
        }

        [Test]
        public void DriverResult_ThrowsException_ShouldNotDisruptModule()
        {
            // Arrange
            _moduleConfig.DefaultExecutionTime = 1;
            var processId = 42;
            var runningActivity = new AssemblyActivity();
            runningActivity.Tracing.Started = DateTime.Now;
            runningActivity.Tracing.ResourceId = _assemblyCell.Id;
            CreateActivity(runningActivity, _processMock.Object, new[] { _assemblyCell });

            _processMock.Setup(p => p.GetActivities(It.IsAny<Func<IActivity, bool>>()))
                .Returns(new[] { runningActivity });

            _processMock.SetupGet(x => x.Id).Returns(processId);
            _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { _processMock.Object });

            var exception = new Exception("Test");
            _assemblyDriverMock.Setup(d => d.Result(It.IsAny<SimulationResult>())).Throws(exception);

            // Act
            _processSimulator.Start();
            Thread.Sleep(100);

            // Assert
            Assert.That(_parallelOperations.ScheduledExecutionExceptions().Any(), Is.False);
        }

        private void Arrange(long processId, ICell cell1, ICell cell2, Activity activity1, Activity activity2)
        {
            ((AssemblyTestCell)cell1).TestInit();
            ((AssemblyTestCell)cell2).TestInit();
            // process
            _processMock.SetupSequence(p => p.GetActivities())
                .Returns(new[] { activity1 })
                .Returns(new[] { activity1, activity2 })
                .Returns(new[] { activity1, activity2 })
                .Returns(new[] { activity1, activity2 })
                .Returns(new[] { activity1, activity2 });

            _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()))
                .Returns(activity2);

            _processMock.SetupGet(x => x.Id).Returns(processId);

            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { _processMock.Object });
            CreateActivity(activity1, _processMock.Object, new[] { cell2, cell1 });
            CreateActivity(activity2, _processMock.Object, new[] { cell1, cell2 });
        }

        private Activity CreateActivity(Activity activity, IProcess process, ICell[] cells)
        {
            activity.Process = process;
            process.AddActivity(activity);
            //set targets for the activity
            _activityTargets[activity] = cells;

            return activity;
        }

    }
}

