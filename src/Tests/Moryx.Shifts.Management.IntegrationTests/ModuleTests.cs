// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Runtime.Modules;
using NUnit.Framework;

namespace Moryx.Shifts.Management.IntegrationTests
{
    [TestFixture]
    internal class ModuleTests : TestBase
    {
        [SetUp]
        public override Task SetUp()
        {
            return base.SetUp();
        }

        [TearDown]
        public override Task TearDown()
        {
            return base.TearDown();
        }

        [Test]
        public async Task Start_WhenModuleIsStopped_StartsModule()
        {
            // Arrange
            // Act
            var module = await _env.StartTestModule();

            // Assert
            Assert.That(module.State, Is.EqualTo(ServerModuleState.Running));
        }

        [Test]
        public async Task Stop_WhenModuleIsRunning_StopsModule()
        {
            // Arrange
            await _env.StartTestModule();

            // Act
            var module = await _env.StopTestModule();

            // Assert
            Assert.That(module.State, Is.EqualTo(ServerModuleState.Stopped));
        }

        [Test]
        public async Task Start_WithDatabaseIsFilled_StartsModule()
        {
            // Arrange
            _resourceManagementMock.Setup(r => r.GetResource<IResource>(It.IsAny<long>())).Returns(ResourceMock.Object);
            _operatorManagementMock.SetupGet(o => o.Operators).Returns([Operator]);

            await _env.StartTestModule();
            var shiftType = _facade.CreateShiftType(TypeContext);
            var shift = _facade.CreateShift(GetShiftContext(shiftType));
            var context = GetAssignementContext(shift, ResourceMock.Object, Operator);
            var assignement = _facade.CreateShiftAssignement(context);

            // Act
            await _env.StopTestModule();
            var module = await _env.StartTestModule();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(module.State, Is.EqualTo(ServerModuleState.Running), "Module is not in state running");
                Assert.That(ObjectsAreEqual(_facade.Shifts.Single(), shift), "Shifts do not match after restart");
                Assert.That(ObjectsAreEqual(_facade.ShiftTypes.Single(), shiftType), "ShiftTypes do not match after restart");
                Assert.That(ObjectsAreEqual(_facade.ShiftAssignements.Single(), assignement), "ShiftAssignements do not match after restart");
            });
        }

        [Test]
        public async Task AnyMethod_WhenFacadeNotActivated_ThrowsHealthStateException()
        {
            // Arrange
            await _env.StartTestModule();
            await _env.StopTestModule();

            // Act
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(() => _facade.CreateShift(null), Throws.TypeOf<HealthStateException>());
                Assert.That(() => _facade.UpdateShift(null), Throws.TypeOf<HealthStateException>());
                Assert.That(() => _facade.DeleteShift(0), Throws.TypeOf<HealthStateException>());

                Assert.That(() => _facade.CreateShiftType(null), Throws.TypeOf<HealthStateException>());
                Assert.That(() => _facade.UpdateShiftType(null), Throws.TypeOf<HealthStateException>());
                Assert.That(() => _facade.DeleteShiftType(0), Throws.TypeOf<HealthStateException>());

                Assert.That(() => _facade.CreateShiftAssignement(null), Throws.TypeOf<HealthStateException>());
                Assert.That(() => _facade.DeleteShiftAssignement(0), Throws.TypeOf<HealthStateException>());
            });
        }
    }
}

