// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moryx.Model.InMemory;
using Moryx.TestTools.Test.Model;
using NUnit.Framework;

namespace Moryx.Model.Tests
{
    [TestFixture]
    public class InMemoryTests
    {

        [Test]
        public async Task InMemoryContextShouldWork()
        {
            // Arrange
            const string carName = "BMW 320d F31 - Mineral Gray";
            var inMemoryFactory = new InMemoryDbContextManager(Guid.NewGuid().ToString());

            // Act
            var context = inMemoryFactory.Create<TestModelContext>();

            // Assert
            var carsSet = context.Cars;
            var someCar = new CarEntity {Name = carName};
            await carsSet.AddAsync(someCar);

            await context.SaveChangesAsync();

            context = inMemoryFactory.Create<TestModelContext>();
            var reloadedCar = await context.Cars.FirstOrDefaultAsync();

            Assert.IsNotNull(reloadedCar);
            Assert.AreEqual(carName, reloadedCar.Name);
        }
    }
}
