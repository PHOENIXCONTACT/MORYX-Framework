// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Moq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Factory;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Moryx.AspNetCore;
using Moryx.FactoryMonitor.Endpoints.Models;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    [TestFixture]
    public class FactoryMonitorController_CellOperationsTest : BaseTest
    {
        [Test]
        public void GetCellPropertiesSettings_Should_Return_Properties_With_Entry_Attribute()
        {
            // Arange
            string identifier = _assemblyCell.Id.ToString();
            _assemblyCell.Temperature = 157;
            //Act
            var endPointResult = _factoryMonitor.GetCellPropertiesSettings(identifier);

            //Assert
            Assert.That(endPointResult.Value.Keys.Contains(nameof(_assemblyCell.Temperature)));
            var valuePair = endPointResult.Value.FirstOrDefault(pair => pair.Key == nameof(_assemblyCell.Temperature));
            Assert.That(Equals(valuePair.Value.CurrentValue, _assemblyCell.Temperature.ToString()));
        }

        [Test]
        public void GetCellPropertiesSettings_ShouldThrowException_For_Wrong_Id()
        {
            // --------------part 2 - bad cell
            string identifier = "100";

            //act
            var endPointResult = _factoryMonitor.GetCellPropertiesSettings(identifier);

            //assert
            Assert.That(((NotFoundObjectResult)endPointResult.Result).StatusCode, Is.EqualTo(404));
            Assert.That(((NotFoundObjectResult)endPointResult.Result).Value, Is.InstanceOf<MoryxExceptionResponse>(), "The exception type should be a MoryxExceptionResponse");

        }

        [Test]
        public async Task CellSettings_Update_Should_Return_200_OK()
        {
            // Arange
            long id = _assemblyCellId;
            var newCellSettings = new CellSettingsModel { Icon = "science_icon", Image = "" };

            //Act
            var endPointResult = await _factoryMonitor.CellSettings(id, newCellSettings);

            //Assert
            Assert.That(((OkResult)endPointResult).StatusCode, Is.EqualTo(200));
            _resourceManagementMock.Verify(rm => rm.ModifyUnsafeAsync(_assemblyCellLocation.Id, (Func<Resource, Task<bool>>)It.IsAny<Func<Resource, Task<bool>>>()), Times.Once, "The Location resource was not updated!");
        }

        [Test]
        public async Task CellSettings_Shoud_Return_NotFound()
        {
            // assert
            long id = 0;
            var newCellSettings = new CellSettingsModel { Icon = "my_icon", Image = "dffsdfsdf" };

            //Act
            var endPointResult = await _factoryMonitor.CellSettings(id, newCellSettings);

            //Assert
            Assert.That(((NotFoundResult)endPointResult).StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task MoveCell_Shouldwork_For_CorrectId()
        {
            // Arange
            var newLocation = new CellLocationModel { Id = _assemblyCellId, PositionX = 0.5, PositionY = 0.2 };
            _assemblyCellLocation.Position =
                new Position { PositionX = newLocation.PositionX, PositionY = newLocation.PositionY };
            _resourceManagementMock.Setup(rm => rm.GetResource<IMachineLocation>(It.IsAny<Func<IMachineLocation, bool>>()))
                 .Returns(_assemblyCellLocation);

            //Act
            var endPointResult = await _factoryMonitor.MoveCell(newLocation);

            //Assert
            Assert.That(((OkObjectResult)endPointResult.Result).StatusCode, Is.EqualTo(200));
            Assert.That(((OkObjectResult)endPointResult.Result).Value, Is.EqualTo(newLocation));
            _resourceManagementMock.Verify(rm => rm.GetResource<IMachineLocation>(It.IsAny<Func<IMachineLocation, bool>>()), Times.AtLeastOnce);
        }

    }
}

