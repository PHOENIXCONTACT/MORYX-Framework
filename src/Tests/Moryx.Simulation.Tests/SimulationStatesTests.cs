// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Simulation;
using Moryx.Resources.Benchmarking.Messages;
using NUnit.Framework;

namespace Moryx.Simulation.Tests;

internal class SimulationStatesTests : BaseTest
{
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _processSimulator.Start();
    }

    [Test]
    public async Task SimulationStates_Should_Simulate_IdleState()
    {
        //Arrange
        var message = new ReleaseWorkpieceMessage();
        _processControlMock.Setup(pc => pc.GetRunningProcesses()).Returns([new Process { }]);

        //Act
        await _assemblyCell.Driver.SendAsync(message);

        //Assert
        Assert.That(_assemblyCellDriver.SimulatedState, Is.EqualTo(SimulationState.Idle));
    }

    [Test]
    public async Task SimulationStates_Should_Simulate_ExecutingState()
    {
        //Arrange
        var activity = new AssemblyActivity();
        Arrange(7, _assemblyCell, activity);
        _processControlMock.Raise(p => p.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, ActivityProgress.Ready));

        //Act
        await _assemblyCell.Driver.SendAsync(new AssembleProductMessage());

        //Assert
        Assert.That(_assemblyCellDriver.SimulatedState, Is.EqualTo(SimulationState.Executing));
    }

}