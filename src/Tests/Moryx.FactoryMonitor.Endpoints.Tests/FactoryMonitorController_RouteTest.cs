// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Moq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Factory;
using Moryx.Tools;
using NUnit.Framework;
using System;
using System.Linq;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    [TestFixture]

    public class FactoryMonitorController_RouteTest : BaseTest
    {
        [Test]
        public void TraceRoute_Create()
        {
            // Arange
            long expectedId = 10;
            _resourceManagementMock.Setup(rm => rm.Read(_assemblyCellLocation.Id, It.IsAny<Func<Resource, Resource>>()))
                .Returns(new MachineLocation { Id = _assemblyCellLocation.Id });
            _resourceManagementMock.Setup(rm => rm.Read(_solderingCellLocation.Id, It.IsAny<Func<Resource, Resource>>()))
                .Returns(new MachineLocation { Id = _solderingCellLocation.Id });
            _resourceManagementMock.Setup(rm => rm.Create(typeof(TransportPath), It.IsAny<Action<Resource>>()))
                .Returns(expectedId);

            //Act
            var endPointResult = _factoryMonitor.TraceRoute(_transportPathModel);

            //Assert
            Assert.That(((OkResult)endPointResult).StatusCode, Is.EqualTo(200));
            _resourceManagementMock.Verify(rm => rm.Create(typeof(TransportPath), It.IsAny<Action<Resource>>()), Times.Once, "The resource was not created!");
        }

        [Test]
        public void TraceRoute_Not_Found()
        {
            long expectedId = 10;
            // Arrange
            _solderingCellLocation.Destinations.AddRange([ new TransportPath
                    {
                        Origin = _solderingCellLocation,
                        Destination = _assemblyCellLocation,
                        Id = expectedId
                    } ]);
            //Act
            var endPointResult = _factoryMonitor.TraceRoute(_transportPathModel);

            //Assert
            Assert.That(((OkResult)endPointResult).StatusCode, Is.EqualTo(200));
            _resourceManagementMock.Verify(rm => rm.Modify(expectedId, It.IsAny<Func<Resource, bool>>()), Times.Once, "The resource was not updated!");

            _solderingCellLocation.Destinations.Clear();
        }

        [Test]
        public void GetRoutes()
        {
            // Arange
            _solderingCellLocation.Destinations.AddRange([ new TransportPath
                    {
                        Destination = _solderingCellLocation,
                        Origin = _assemblyCellLocation,
                        Id = 10
                    } ]);

            _assemblyCellLocation.Destinations.AddRange([ new TransportPath
                    {
                        Destination = _assemblyCellLocation,
                        Origin = _solderingCellLocation,
                        Id = 11
                    } ]);

            //Act
            var endPointResult = _factoryMonitor.GetRoutes();

            //Assert
            foreach (var route in endPointResult.Value)
            {
                Assert.That(GetLocations().Any(x => x.Id == route.IdCellOfDestination || x.Id == route.IdCellOfOrigin));
            }

            _solderingCellLocation.Destinations.Clear();
            _assemblyCellLocation.Destinations.Clear();
        }
    }
}

