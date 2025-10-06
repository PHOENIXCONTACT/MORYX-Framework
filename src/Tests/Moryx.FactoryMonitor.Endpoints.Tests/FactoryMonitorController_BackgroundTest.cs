// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc;
using Moq;
using Moryx.AbstractionLayer.Resources;
using NUnit.Framework;
using System;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    [TestFixture]
    public class FactoryMonitorController_BackgroundTest : BaseTest
    {

        [Test]
        public void ChangeBackground_ShouldWork()
        {
            // Arange
            var newBackgroundUrl = @"https://media.istockphoto.com/id/849023956/photo/robot-assembly-line-in-car-factory.jpg?s=612x612&w=0&k=20&c=K39t_WpWlKB6I3iJco72kiscOxYdXl1ons-0VSf8yyo=";
            _resourceManagementMock.Setup(rm => rm.Modify(_manufactoringFactoryId, It.IsAny<Func<Resource, bool>>()))
                .Callback(() =>
                {
                    _manufactoringFactory.BackgroundUrl = newBackgroundUrl;
                });

            //Act
            var endPointResult = _factoryMonitor.ChangeBackground(_manufactoringFactoryId, newBackgroundUrl);

            //Assert
            Assert.That(((OkResult)endPointResult).StatusCode, Is.EqualTo(200));
            Assert.That(_manufactoringFactory.BackgroundUrl, Is.EqualTo(newBackgroundUrl));
            _resourceManagementMock.Verify(rm => rm.Modify(_manufactoringFactoryId, It.IsAny<Func<Resource, bool>>()), Times.Once);
        }

    }
}

