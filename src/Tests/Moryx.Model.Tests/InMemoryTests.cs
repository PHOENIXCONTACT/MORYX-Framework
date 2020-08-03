using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Model.InMemory;
using Moryx.TestTools.Test.Model;
using NUnit.Framework;

namespace Moryx.Model.Tests
{
    [TestFixture]
    public class InMemoryTests
    {

        [Test]
        public void InMemoryContextShouldWork()
        {
            // Arrange
            const string carName = "BMW 320d F31 - Mineral Gray";
            var inMemoryFactory = new InMemoryDbContextManager(Guid.NewGuid().ToString());

            // Act
            var context = inMemoryFactory.Create<TestModelContext>();

            // Assert
            var carsSet = context.Cars;
            var someCar = carsSet.Create();
            someCar.Name = carName;

            carsSet.Add(someCar);
            context.SaveChanges();

            context = inMemoryFactory.Create<TestModelContext>();
            var reloadedCar = context.Cars.First();

            Assert.IsNotNull(reloadedCar);
            Assert.AreEqual(carName, reloadedCar.Name);
        }
    }
}
