﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Simulation.Simulator;
using Moq;
using NUnit.Framework;
using Moryx.Logging;
using Moryx.TestTools.UnitTest;

namespace Moryx.Simulation.Tests
{
    internal class BaseTest
    {
        protected ModuleConfig _moduleConfig;
        protected Mock<IModuleLogger> _simulationLoggerMock;
        protected Mock<IProcessControl> _processControlMock;
        protected Mock<IResourceManagement> _resourceManagementMock;
        protected AssemblyTestCell _assemblyCell;
        protected AssemblyTestCell _anotherAssemblyCell;
        protected SimulatedDummyTestDriver _assemblyCellDriver;
        protected SimulatedDummyTestDriver _anotherAssemblyCellDriver;
        protected ProcessSimulator _processSimulator;
        protected NotSoParallelOps _parallelOperations = new();
        protected readonly Dictionary<IActivity, IReadOnlyList<ICell>> _activityTargets = [];

        [SetUp]
        public virtual void Setup()
        {
            //module configuration
            _moduleConfig = new ModuleConfig
            {
                ConfigState = Configuration.ConfigState.Generated,
                DefaultExecutionTime = 500,
                SuccessRate = 90,
                Acceleration = 1
            };

            //driver
            _assemblyCellDriver = new SimulatedDummyTestDriver();
            _anotherAssemblyCellDriver = new SimulatedDummyTestDriver();

            //cell
            _assemblyCell = Builder.CellBuilder(1, _assemblyCellDriver);

            _anotherAssemblyCell = Builder.CellBuilder(2, _anotherAssemblyCellDriver);

            //process control
            _processControlMock = new Mock<IProcessControl>();
            _processControlMock.Setup(pc => pc.Targets(It.IsAny<IActivity>()))
                .Returns<IActivity>(a => _activityTargets.TryGetValue(a, out IReadOnlyList<ICell>? value) ? value : []);

            _processControlMock.Setup(pc => pc.Targets(It.IsAny<IProcess>()))
                .Returns(new[] { _assemblyCell, _anotherAssemblyCell });
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns([]);

            //resource management
            _resourceManagementMock = new Mock<IResourceManagement>();
            _resourceManagementMock.Setup(rm => rm.GetAllResources<ISimulationDriver>(It.IsAny<Func<ISimulationDriver, bool>>()))
                .Returns(new[] { _assemblyCellDriver, _anotherAssemblyCellDriver });

            //logger mock
            _simulationLoggerMock = new();
            //proces simulator
            _processSimulator = new ProcessSimulator();
            _processSimulator.Logger = _simulationLoggerMock.Object;
            _processSimulator.ProcessControl = _processControlMock.Object;
            _processSimulator.ResourceManagement = _resourceManagementMock.Object;
            _processSimulator.ParallelOperations = _parallelOperations;
            _processSimulator.Config = _moduleConfig;
        }




        protected Activity CreateActivity(Activity activity, Process process, ICell cell)
        {
            activity.Process = process;
            process.AddActivity(activity);
            //set targets for the activity
            _activityTargets[activity] = [cell];

            return activity;
        }

        protected void RaiseActivityUpdated(IActivity activity, ActivityProgress progress)
        {
            _processControlMock.Raise(pm => pm.ActivityUpdated += null, new ActivityUpdatedEventArgs(activity, progress));
        }

        [TearDown]
        public void TearDown()
        {
            _processSimulator.Stop();
        }

        protected void Arrange(long processId, ICell cell, Activity activity)
        {
            ((AssemblyTestCell)cell).TestInit();
            var process = new ProductionProcess { Id = processId };
            _processControlMock.Setup(pc => pc.RunningProcesses).Returns(new[] { process });
            CreateActivity(activity, process, cell);

        }

    }
}

