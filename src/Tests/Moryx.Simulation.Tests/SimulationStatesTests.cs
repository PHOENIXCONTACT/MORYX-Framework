// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Simulation;
using Moryx.Simulation.Examples.Messages;
using NUnit.Framework;

namespace Moryx.Simulation.Tests
{
    internal class SimulationStatesTests : BaseTest
    {
        [SetUp] 
        public override void Setup()
        {
            base.Setup();
            _processSimulator.Start();
        }

        [Test]
        public void SimulationStates_Should_Simulate_IdleState()
        {
            //Arrange
            var message = new ReleaseWorkpieceMessage();
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { new AbstractionLayer.Process { } });

            //Act
            _assemblyCell.Driver.Send(message);

            //Assert
            Assert.That(_assemblyCellDriver.SimulatedState, Is.EqualTo(SimulationState.Idle));
        }

        [Test]
        public void SimulationStates_Should_Simulate_ExecutingState()
        {
            //Arrange
            var activity = new AssemblyActivity();
            Arrange(7, _assemblyCell, activity);
            _processControlMock.Raise(p => p.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, ActivityProgress.Ready));

            //Act
            _assemblyCell.Driver.Send(new AssembleProductMessage());

            //Assert
            Assert.That(_assemblyCellDriver.SimulatedState, Is.EqualTo(SimulationState.Executing));
        }

    }
}

