// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.TestTools.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Model;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.FactoryMonitor.Endpoints.Tests.Resources;
using Moryx.Orders;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    public abstract class BaseTest
    {
        protected Mock<IResourceManagement> _resourceManagementMock = new();
        protected Mock<IProcessControl> _processFacadeMock = new();
        protected Mock<IOrderManagement> _orderFacadeMock = new();
        protected FactoryMonitorController _factoryMonitor;
        protected DummyCell _assemblyCell;
        protected DummyCell _solderingCell;
        protected MachineLocation _assemblyCellLocation;
        protected MachineLocation _solderingCellLocation;
        protected ManufacturingFactory _manufactoringFactory;
        protected const long _assemblyCellId = 1;
        protected const long _solderingCellId = 2;
        protected const long _manufactoringFactoryId = 3;
        protected string backgroundUrl = "https://www.kuka.com/-/media/kuka-corporate/images/home/stage/kuka-robot-systems.jpg?rev=65074bf713a74a8ea6b08da65d068946&w=1400&hash=6E6DBB77432C3395E47F2A539F6EEE86";
        protected readonly Dictionary<IActivity, IReadOnlyList<ICell>> _activityTargets = new();
        protected TransportRouteModel _transportPathModel;
        protected Position _assemblyCellposition;
        protected Position _solderingCellPosition;
        protected ResourceGraphMock _graph;

        [SetUp]
        public virtual void Setup()
        {
            _graph = new ResourceGraphMock();
            // positions
            _assemblyCellposition = new Position { PositionX = 0.5, PositionY = 0.7 };
            _solderingCellPosition = new Position { PositionX = 0.3, PositionY = 0.5 };
            var converter = new Converter(new CellSerialization());

            //manufacturing resource
            _manufactoringFactory = _graph.Instantiate<ManufacturingFactory>();
            _manufactoringFactory.Id = _manufactoringFactoryId;
            _manufactoringFactory.BackgroundUrl = backgroundUrl;
            _manufactoringFactory.Children.Add(_assemblyCellLocation);
            _manufactoringFactory.Children.Add(_solderingCellLocation);
            _resourceManagementMock.Setup(rm => rm.GetResources<IManufacturingFactory>())
                        .Returns([_manufactoringFactory]);
            _resourceManagementMock.Setup(rm => rm.GetResource(It.IsAny<Func<IManufacturingFactory, bool>>()))
                        .Returns(_manufactoringFactory);
            //_assemblyCell
            _assemblyCell = _graph.Instantiate<DummyCell>();
            _assemblyCell.Id = _assemblyCellId;
            _resourceManagementMock.Setup(rm => rm.GetResource<ICell>(_assemblyCellId))
                .Returns(_assemblyCell);

            //_solderingCell
            _solderingCell = _graph.Instantiate<DummyCell>();
            _solderingCell.Id = _solderingCellId;
            _resourceManagementMock.Setup(rm => rm.GetResource<ICell>(_solderingCellId))
                .Returns(_solderingCell);

            //_assemblyCell location 
            _assemblyCellLocation = _graph.Instantiate<MachineLocation>();
            _assemblyCellLocation.Children.Add(_assemblyCell);
            _assemblyCellLocation.Id = 4;
            _assemblyCellLocation.Position = _assemblyCellposition;
            _assemblyCellLocation.SpecificIcon = "users";
            _resourceManagementMock.Setup(rm =>
            rm.GetResource<IMachineLocation>(It.Is<Func<IMachineLocation, bool>>(f => f(_assemblyCellLocation))))
                .Returns(_assemblyCellLocation);
            _resourceManagementMock.Setup(rm => rm.Read(_assemblyCellLocation.Id, It.IsAny<Func<Resource, MachineLocation>>()))
                .Returns(_assemblyCellLocation);
            //_solderingCell location 
            _solderingCellLocation = _graph.Instantiate<MachineLocation>();
            _solderingCellLocation.Children.Add(_solderingCell);
            _solderingCellLocation.Id = 5;
            _solderingCellLocation.Position = _solderingCellPosition;
            _solderingCellLocation.SpecificIcon = "group";
            _resourceManagementMock.Setup(rm =>
            rm.GetResource<IMachineLocation>(It.Is<Func<IMachineLocation, bool>>(f => f(_solderingCellLocation))))
                .Returns(_solderingCellLocation);
            _resourceManagementMock.Setup(rm => rm.Read(_solderingCellLocation.Id, It.IsAny<Func<Resource, MachineLocation>>()))
                .Returns(_solderingCellLocation);

            // resource management cells
            _resourceManagementMock.Setup(rm => rm.GetResources<ICell>())
                .Returns([_assemblyCell, _solderingCell]);
            _resourceManagementMock.Setup(rm => rm.GetResources<IMachineLocation>())
                .Returns(GetLocations());
            _resourceManagementMock.Setup(rm => rm.GetResources(It.IsAny<Func<IMachineLocation, bool>>()))
                .Returns(GetLocations());
            _resourceManagementMock.Setup(rm => rm.Read(_assemblyCellId, It.IsAny<Func<Resource, Resource>>()))
                .Returns(_assemblyCell);
            _resourceManagementMock.Setup(rm => rm.Read(_assemblyCellId, It.IsAny<Func<Resource, ResourceChangedModel>>()))
                 .Returns(converter.ToResourceChangedModel(_assemblyCell));
            _resourceManagementMock.Setup(rm => rm.Read(_solderingCellId, It.IsAny<Func<Resource, Resource>>()))
                .Returns(_solderingCell);
            _resourceManagementMock.Setup(rm => rm.Read(_solderingCellId, It.IsAny<Func<Resource, ResourceChangedModel>>()))
                 .Returns(converter.ToResourceChangedModel(_solderingCell));
            _resourceManagementMock.Setup(rm => rm.Read(_manufactoringFactoryId, It.IsAny<Func<Resource, Resource>>()))
                .Returns(_manufactoringFactory);
            _resourceManagementMock.Setup(rm => rm.Read(_manufactoringFactoryId, It.IsAny<Func<Resource, ResourceChangedModel>>()))
                .Returns(converter.ToResourceChangedModel(_manufactoringFactory));
            //process
            _processFacadeMock.SetupGet(pm => pm.RunningProcesses)
                .Returns(Array.Empty<IProcess>());
            _processFacadeMock.Setup(pm => pm.Targets(It.IsAny<IActivity>()))
                .Returns<IActivity>(a => _activityTargets.ContainsKey(a) ? _activityTargets[a] : Array.Empty<ICell>());

            //orders
            _orderFacadeMock.Setup(o => o.GetOperations(It.IsAny<Func<Operation, bool>>()))
                .Returns(Array.Empty<Operation>());

            //factory controller
            _factoryMonitor = new FactoryMonitorController(_resourceManagementMock.Object, _processFacadeMock.Object, _orderFacadeMock.Object);

            //transport path
            _transportPathModel = new TransportRouteModel()
            {
                IdCellOfOrigin = _solderingCellId,
                IdCellOfDestination = _assemblyCellId,
                Paths = new List<Position>
                {
                    _solderingCellLocation.Position,
                    new() { PositionX = 0.1, PositionY = 0.2},
                    _assemblyCellLocation.Position
                }
            };
        }

        protected IMachineLocation[] GetLocations()
        {
            return [_assemblyCellLocation, _solderingCellLocation];
        }

    }
}

