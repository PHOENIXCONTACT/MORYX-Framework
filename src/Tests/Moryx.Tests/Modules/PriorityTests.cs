// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;
using NUnit.Framework;

namespace Moryx.Tests.Modules
{
    [TestFixture]
    public class PriorityTests
    {
        [Test]
        public void PriorityCompare()
        {
            // Arrange
            var pMax = Priority.Max;
            var pMin = Priority.Min;
            var p0 = Priority.P0;
            var p3 = Priority.P3;
            var pCustom = Priority.P(42);

            // Act

            // Assert
            Assert.That(pMax.CompareTo(pMin), Is.GreaterThan(0));
            Assert.That(0, Is.EqualTo(pMax.CompareTo(p0)));
            Assert.That(pMin.CompareTo(p3), Is.LessThan(0));
            Assert.That(pCustom.CompareTo(p3), Is.LessThanOrEqualTo(pCustom.CompareTo(p3)));
            Assert.That(p0 > p3);
            Assert.That(p0 >= p3);
            Assert.That(pMax == p0);
            Assert.That(pMax <= p0);
            Assert.That(pMax >= p0);
            Assert.That(pCustom < p3);
            Assert.That(pCustom <= p3);
        }
    }
}
