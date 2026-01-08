// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Processes;
using Moryx.ControlSystem.Simulation;
using Moryx.Resources.Benchmarking.Messages;
using NUnit.Framework;

namespace Moryx.Simulation.Tests
{
    internal class SimulationDriverTests : BaseTest
    {
        Mock<SimulatedDummyTestDriver> _simulationDriverTestMock;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            _simulationDriverTestMock = new Mock<SimulatedDummyTestDriver>() { DefaultValue = DefaultValue.Mock };
            _simulationDriverTestMock.SetupGet(dr => dr.Cell).Returns(_assemblyCell);
            _simulationDriverTestMock.SetupGet(dr => dr.Usages).Returns([_assemblyCell]);
            _simulationDriverTestMock.Setup(dr => dr.SendAsync(It.IsAny<AssembleProductMessage>(), It.IsAny<CancellationToken>()))
                .Callback<object, CancellationToken>((param, _) =>
                {
                    var message = param as AssembleProductMessage;
                    _simulationDriverTestMock.Setup(dr => dr.SimulatedState)
                    .Returns(SimulationState.Executing);

                    _simulationDriverTestMock.Raise(dr => dr.SimulatedStateChanged += null, _simulationDriverTestMock.Object, SimulationState.Executing);
                });

            _simulationDriverTestMock.Setup(dr => dr.SendAsync(It.IsAny<ReleaseWorkpieceMessage>(), It.IsAny<CancellationToken>()))
               .Callback<object, CancellationToken>((param, _) =>
               {
                   var message = param as AssembleProductMessage;
                   _simulationDriverTestMock.Setup(dr => dr.SimulatedState)
                   .Returns(SimulationState.Idle);

                   _simulationDriverTestMock.Raise(dr => dr.SimulatedStateChanged += null, _simulationDriverTestMock.Object, SimulationState.Idle);
               });

            _simulationDriverTestMock.Setup(dr => dr.Ready(It.IsAny<Activity>()))
               .Callback<Activity>(message =>
               {
                   _simulationDriverTestMock.Setup(dr => dr.SimulatedState)
                   .Returns(SimulationState.Requested);

                   _simulationDriverTestMock.Raise(dr => dr.SimulatedStateChanged += null, _simulationDriverTestMock.Object, SimulationState.Requested);
                   _simulationDriverTestMock.Raise(dr => dr.Received += null, _simulationDriverTestMock.Object, new WorkpieceArrivedMessage { ProcessId = message.Process.Id });
               });

            _simulationDriverTestMock.Setup(dr => dr.Result(It.IsAny<SimulationResult>()))
               .Callback<SimulationResult>(message =>
               {
                   _simulationDriverTestMock.Raise(dr => dr.Received += null, _simulationDriverTestMock.Object, new AssemblyCompletedMessage { Result = message.Result });
               });

            _assemblyCell.Driver = _simulationDriverTestMock.Object;

            //resource management
            _resourceManagementMock.Setup(rm => rm.GetResourcesUnsafe<ISimulationDriver>(It.IsAny<Func<ISimulationDriver, bool>>()))
                .Returns([_simulationDriverTestMock.Object]);

            //start the simulator
            _processSimulator.Start();
        }

        [Test]
        public async Task Driver_Should_SendMessage()
        {
            //Arrange
            var activity = new AssemblyActivity();
            Arrange(3, _assemblyCell, activity);
            _processControlMock.Raise(p => p.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, ActivityProgress.Ready));

            //TODO: What? SendAsync is executed on the driver, and it is verified that it was executed on the driver? This makes no sense.

            //Act
            await _assemblyCell.Driver.SendAsync(new AssembleProductMessage());

            //Assert
            _simulationDriverTestMock.Verify(dr => dr.SendAsync(It.IsAny<AssembleProductMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Driver_Should_Receive_Ready_message()
        {
            //Arrange
            var activity = new AssemblyActivity();
            Arrange(4, _assemblyCell, activity);

            //Act
            _processControlMock.Raise(p => p.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, ActivityProgress.Ready));

            //Assert
            _simulationDriverTestMock.Verify(dr => dr.Ready(It.IsAny<Activity>()), Times.Once);
        }

        [Test]
        public void Driver_Should_Receive_Result_message()
        {
            //Arrange
            var activity = new AssemblyActivity();
            activity.Tracing.ResourceId = _assemblyCell.Id;
            Arrange(5, _assemblyCell, activity);

            //Act
            //trigger ready
            _simulationDriverTestMock.Raise(x => x.SimulatedStateChanged += null, _simulationDriverTestMock.Object, SimulationState.Idle);

            _processControlMock.Raise(x => x.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, ActivityProgress.Running));
            Thread.Sleep(4000);

            //Assert
            _simulationDriverTestMock.Verify(dr => dr.Result(It.IsAny<SimulationResult>()), Times.Once);
        }

    }
}

