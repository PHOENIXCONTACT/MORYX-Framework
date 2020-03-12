// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.TestTools.Test.Model;
using NUnit.Framework;

namespace Marvin.Model.Tests
{
    [TestFixture]
    public class ReferenceTests
    {
        private InMemoryUnitOfWorkFactory _unitOfWorkFactory;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            _unitOfWorkFactory = new InMemoryUnitOfWorkFactory(Guid.NewGuid().ToString());
            _unitOfWorkFactory.Initialize();
        }

        [Test]
        public void CollectionsNotNull()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                // Act
                var car = carRepo.Create();
                var car2 = new CarEntity();

                // Assert
                Assert.NotNull(car.Wheels);
                Assert.Null(car2.Wheels);
            }
        }

        [Test]
        public void CollectionUsed()
        {
            long id;
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();
                var wheelRepo = uow.GetRepository<IWheelEntityRepository>();

                // Act
                var a = carRepo.Create();
                a.Name = "Test Car";

                var wheel = wheelRepo.Create();
                wheel.WheelType = WheelType.FrontLeft;
                a.Wheels.Add(wheel);

                wheel = wheelRepo.Create();
                wheel.WheelType = WheelType.RearLeft;
                a.Wheels.Add(wheel);

                uow.Save();

                id = a.Id;
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var a = uow.GetRepository<ICarEntityRepository>().GetByKey(id);

                Assert.AreEqual(2, a.Wheels.Count);
            }
        }
    }
}
