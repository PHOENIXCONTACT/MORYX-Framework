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
            Assert.Greater(pMax.CompareTo(pMin), 0);
            Assert.AreEqual(pMax.CompareTo(p0), 0);
            Assert.Less(pMin.CompareTo(p3), 0);
            Assert.LessOrEqual(pCustom.CompareTo(p3), pCustom.CompareTo(p3));
            Assert.IsTrue(p0 > p3);
            Assert.IsTrue(p0 >= p3);
            Assert.IsTrue(pMax == p0);
            Assert.IsTrue(pMax <= p0);
            Assert.IsTrue(pMax >= p0);
            Assert.IsTrue(pCustom < p3);
            Assert.IsTrue(pCustom <= p3);
        }
    }
}
