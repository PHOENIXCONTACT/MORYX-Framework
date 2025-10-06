// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
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
            var someCar = new CarEntity { Name = carName };
            await carsSet.AddAsync(someCar);

            await context.SaveChangesAsync();

            context = inMemoryFactory.Create<TestModelContext>();
            var reloadedCar = await context.Cars.FirstAsync();

            Assert.That(reloadedCar, Is.Not.Null);
            Assert.That(reloadedCar.Name, Is.EqualTo(carName));

        }

        [Test]
        public async Task TimeShouldAlwaysBeSavedAsUtc()
        {
            // Arrange          
            var inMemoryFactory = new InMemoryDbContextManager(Guid.NewGuid().ToString());

            // Act
            var context = inMemoryFactory.Create<TestModelContext>();
            var carsSet = context.Cars;
            var someCar = new CarEntity { ReleaseDateLocal = DateTime.Now, ReleaseDateUtc = DateTime.Now };
            await carsSet.AddAsync(someCar);

            await context.SaveChangesAsync();

            context = inMemoryFactory.Create<TestModelContext>();
            var reloadedCar = await context.Cars.FirstAsync();

            // Assert
            Assert.That(reloadedCar, Is.Not.Null);
            Assert.That(reloadedCar.ReleaseDateUtc.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(reloadedCar.ReleaseDateLocal.Kind, Is.EqualTo(DateTimeKind.Local));
        }
    }
}
