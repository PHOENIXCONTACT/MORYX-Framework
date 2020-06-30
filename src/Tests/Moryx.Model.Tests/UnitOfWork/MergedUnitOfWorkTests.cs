// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.Container;
using Marvin.Modules;
using Marvin.TestTools.Test.Model;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Model.Tests
{
    public class MergedUnitOfWorkTests
    {
        private AnotherInMemoryUnitOfWorkFactory _unitOfWorkFactory;

        [SetUp]
        public void TestSetUp()
        {
            ReflectionTool.TestMode = true;
            var container = new CastleContainer();
            container.LoadComponents<IUnitOfWorkFactory>();
            container.ResolveAll<IUnitOfWorkFactory>().ForEach(f => ((IInitializable)f).Initialize());

            ReflectionTool.TestMode = false;

            _unitOfWorkFactory = new AnotherInMemoryUnitOfWorkFactory(Guid.NewGuid().ToString())
            {
                Container = container
            };
            _unitOfWorkFactory.Initialize();
        }

        [Test]
        public void RepositoryOfParentByTypeGetsReturned()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();

                // Assert
                Assert.NotNull(carRepo);
            }
        }

        [Test]
        public void RepositoryOfChildByTypeGetsReturned()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<IAnotherEntityRepository>();

                // Assert
                Assert.NotNull(carRepo);
            }
        }

        [Test]
        public void AddingEntityToBothContextsWithSavingSavesEntitiesToDb()
        {
            using (var uow = _unitOfWorkFactory.Create())
            {
                // Arrange
                var carRepo = uow.GetRepository<ICarEntityRepository>();
                var anotherRepo = uow.GetRepository<IAnotherEntityRepository>();

                // Act
                carRepo.Create();
                anotherRepo.Create();

                uow.Save();
            }

            using (var uow = _unitOfWorkFactory.Create())
            {
                var carRepo = uow.GetRepository<ICarEntityRepository>();
                var anotherRepo = uow.GetRepository<IAnotherEntityRepository>();

                var cars = carRepo.GetAll();
                var anothers = anotherRepo.GetAll();

                // Assert
                Assert.AreEqual(1, cars.Count);
                Assert.AreEqual(1, anothers.Count);
            }
        }
    }
}
