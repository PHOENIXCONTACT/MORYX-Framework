// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Simulation;
using Moryx.Resources.Benchmarking.Messages;
using NUnit.Framework;
using Activity = Moryx.AbstractionLayer.Activities.Activity;

namespace Moryx.Simulation.Tests;

internal class SimulationActivitiesTests : BaseTest
{
    private Mock<SimulatedDummyTestDriver> _assemblyDriverMock;
    private Mock<SimulatedDummyTestDriver> _anotherAssemblyDriverMock;
    private Mock<Process> _processMock;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _processMock = new Mock<Process>();

        _assemblyDriverMock = new Mock<SimulatedDummyTestDriver>() { DefaultValue = DefaultValue.Mock };
        _anotherAssemblyDriverMock = new Mock<SimulatedDummyTestDriver>() { DefaultValue = DefaultValue.Mock };
        _assemblyDriverMock.SetupGet(dr => dr.Cell).Returns(_assemblyCell);
        _assemblyDriverMock.SetupGet(dr => dr.Usages).Returns([_assemblyCell]);
        _assemblyDriverMock.SetupGet(dr => dr.SimulatedState).Returns(SimulationState.Idle);
        _anotherAssemblyDriverMock.SetupGet(dr => dr.Cell).Returns(_anotherAssemblyCell);
        _anotherAssemblyDriverMock.SetupGet(dr => dr.Usages).Returns([_anotherAssemblyCell]);
        _anotherAssemblyDriverMock.SetupGet(dr => dr.SimulatedState).Returns(SimulationState.Idle);

        _assemblyDriverMock.Setup(dr => dr.SendAsync(It.IsAny<AssembleProductMessage>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((param, _) =>
            {
                var message = param as AssembleProductMessage;
                _assemblyDriverMock.Setup(dr => dr.SimulatedState)
                    .Returns(SimulationState.Executing);

                _assemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _assemblyDriverMock.Object, SimulationState.Executing);
            });

        _anotherAssemblyDriverMock.Setup(dr => dr.SendAsync(It.IsAny<AssembleProductMessage>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((param, _) =>
            {
                var message = param as AssembleProductMessage;
                _anotherAssemblyDriverMock.Setup(dr => dr.SimulatedState)
                    .Returns(SimulationState.Executing);

                _anotherAssemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _anotherAssemblyDriverMock.Object, SimulationState.Executing);
            });

        _assemblyDriverMock.Setup(dr => dr.SendAsync(It.IsAny<ReleaseWorkpieceMessage>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((param, _) =>
            {
                var message = param as AssembleProductMessage;
                _assemblyDriverMock.Setup(dr => dr.SimulatedState)
                    .Returns(SimulationState.Idle);

                _assemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _assemblyDriverMock.Object, SimulationState.Idle);
            });

        _anotherAssemblyDriverMock.Setup(dr => dr.Ready(It.IsAny<Activity>()))
            .Callback<Activity>(message =>
            {
                _anotherAssemblyDriverMock.Setup(dr => dr.SimulatedState)
                    .Returns(SimulationState.Requested);

                _anotherAssemblyDriverMock.Raise(dr => dr.SimulatedStateChanged += null, _anotherAssemblyDriverMock.Object, SimulationState.Requested);
                _anotherAssemblyDriverMock.Raise(dr => dr.Received += null, _anotherAssemblyDriverMock.Object, new WorkpieceArrivedMessage { });
            });

        _assemblyDriverMock.Setup(dr => dr.Ready(It.IsAny<Activity>()))
            .Callback<Activity>(message =>
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
        _resourceManagementMock.Setup(rm => rm.GetResourcesUnsafe(It.IsAny<Func<ISimulationDriver, bool>>()))
            .Returns([_assemblyDriverMock.Object, _anotherAssemblyDriverMock.Object]);

        _processControlMock.SetupSequence(pc => pc.Targets(It.IsAny<Process>()))
            .Returns([_assemblyCell, _anotherAssemblyCell])
            .Returns(Array.Empty<ICell>())
            .Returns([_assemblyCell, _anotherAssemblyCell])
            .Returns([_assemblyCell, _anotherAssemblyCell])
            .Returns([_assemblyCell, _anotherAssemblyCell]);

        _processControlMock.Setup(pc => pc.Targets(It.IsAny<Activity>()))
            .Returns<Activity>(activity => _activityTargets[activity]);
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
        _assemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<Activity>()), Times.Once);
        _anotherAssemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<Activity>()), Times.Once);
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
        _anotherAssemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<Activity>()), Times.Once);
        _assemblyDriverMock.Verify(dr => dr.Ready(It.IsAny<Activity>()), Times.Never);
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
        CreateActivity(completedActivity, _processMock.Object, [_assemblyCell]);

        var readyActivity = new AssemblyActivity();
        readyActivity.Id = 1337;
        CreateActivity(readyActivity, _processMock.Object, [_assemblyCell]);

        _processMock.Setup(p => p.GetActivities(It.IsAny<Func<Activity, bool>>()))
            .Returns([completedActivity, readyActivity]);

        _processMock.SetupGet(x => x.Id).Returns(processId);
        _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<Activity, bool>>()))
            .Returns(completedActivity);
        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([_processMock.Object]);

        // Act
        _processSimulator.Start();

        // Assert
        _assemblyDriverMock.Verify(d => d.Ready(It.Is<Activity>(a => a.Id == readyActivity.Id)), Times.Once);
    }

    [Test]
    public void CompleteMovement_ActivityAlreadyStarted_ShouldNotCallReady()
    {
        // Arrange
        _moduleConfig.MovingDuration = 5;
        var processId = 42;
        var completedActivity = new AssemblyActivity();
        completedActivity.Result = new ActivityResult();
        var readyActivity = new AssemblyActivity();

        _processMock.Setup(p => p.GetActivities(It.IsAny<Func<Activity, bool>>()))
            .Returns([readyActivity]);
        _processMock.Setup(p => p.GetActivities())
            .Returns([completedActivity, readyActivity]);
        _processMock.SetupGet(x => x.Id).Returns(processId);
        _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<Activity, bool>>()));
        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([_processMock.Object]);
        CreateActivity(readyActivity, _processMock.Object, [_anotherAssemblyCell]);

        // Act
        _processSimulator.Start(); // Start movement for readyActivity
        readyActivity.Tracing.Started = DateTime.Now; // Simulate that activity has started during movement
        Thread.Sleep(10); // Wait for movement to complete

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
        CreateActivity(runningActivity, _processMock.Object, [_assemblyCell]);

        _processMock.Setup(p => p.GetActivities(It.IsAny<Func<Activity, bool>>()))
            .Returns([runningActivity]);

        _processMock.SetupGet(x => x.Id).Returns(processId);
        _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));
        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([_processMock.Object]);

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
        CreateActivity(readyActivity, _processMock.Object, [_assemblyCell]);

        _processMock.Setup(p => p.GetActivities(It.IsAny<Func<Activity, bool>>()))
            .Returns([readyActivity]);

        _processMock.SetupGet(x => x.Id).Returns(processId);
        _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));
        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([_processMock.Object]);

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
        CreateActivity(runningActivity, _processMock.Object, [_assemblyCell]);

        _processMock.Setup(p => p.GetActivities(It.IsAny<Func<Activity, bool>>()))
            .Returns([runningActivity]);

        _processMock.SetupGet(x => x.Id).Returns(processId);
        _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()));
        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([_processMock.Object]);

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
            .Returns([activity1])
            .Returns([activity1, activity2])
            .Returns([activity1, activity2])
            .Returns([activity1, activity2])
            .Returns([activity1, activity2]);

        _processMock.Setup(x => x.GetActivity(ActivitySelectionType.LastOrDefault, It.IsAny<Func<IActivity, bool>>()))
            .Returns(activity2);

        _processMock.SetupGet(x => x.Id).Returns(processId);

        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([_processMock.Object]);
        CreateActivity(activity1, _processMock.Object, [cell2, cell1]);
        CreateActivity(activity2, _processMock.Object, [cell1, cell2]);
    }

    private Activity CreateActivity(Activity activity, Process process, ICell[] cells)
    {
        activity.Process = process;
        process.AddActivity(activity);
        //set targets for the activity
        _activityTargets[activity] = cells;

        return activity;
    }

}