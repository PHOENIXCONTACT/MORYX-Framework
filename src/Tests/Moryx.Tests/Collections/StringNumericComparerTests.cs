// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Collections;
using NUnit.Framework;

namespace Moryx.Tests.Collections
{
    [TestFixture]
    public class StringNumericComparerTests
    {
        [Test(Description = "Tests if the comparer orders the string values numerically.")]
        public void OrderStringsByIntValue()
        {
            // Arrange
            var stringNumbers = new[] { "105", "101", "102", "103", "90" };

            // Act
            var ordered = stringNumbers.OrderBy(s => s, new StringNumericComparer()).ToArray();

            // Assert
            Assert.That(ordered[0], Is.EqualTo("90"));
            Assert.That(ordered[1], Is.EqualTo("101"));
            Assert.That(ordered[2], Is.EqualTo("102"));
            Assert.That(ordered[3], Is.EqualTo("103"));
            Assert.That(ordered[4], Is.EqualTo("105"));
        }

        [Test(Description = "Tests if the comparer orders the string values first numerically and then by string")]
        public void OrderStringsByIntAndThenByString()
        {
            // Arrange
            var stringNumbers = new[] { "105", "101", "Hundi", "Wau", "Wau", "90" };

            // Act
            var ordered = stringNumbers.OrderBy(s => s, new StringNumericComparer()).ToArray();

            // Assert
            Assert.That(ordered[0], Is.EqualTo("90"));
            Assert.That(ordered[1], Is.EqualTo("101"));
            Assert.That(ordered[2], Is.EqualTo("105"));
            Assert.That(ordered[3], Is.EqualTo("Hundi"));
            Assert.That(ordered[4], Is.EqualTo("Wau"));
            Assert.That(ordered[5], Is.EqualTo("Wau"));
        }

        [Test]
        public void CorrectlyOrderTrailingNumbers()
        {
            // Arrange
            var stringNumbers = new[] { "", "Dummy 2", null, "Dummy 10", "Dummy03", "5", "Bummy 2" };

            // Act
            var ordered = stringNumbers.OrderBy(s => s, new StringNumericComparer()).ToArray();

            // Assert
            Assert.That(ordered[0], Is.EqualTo(""));
            Assert.That(ordered[1], Is.EqualTo(null));
            Assert.That(ordered[2], Is.EqualTo("5"));
            Assert.That(ordered[3], Is.EqualTo("Bummy 2"));
            Assert.That(ordered[4], Is.EqualTo("Dummy 2"));
            Assert.That(ordered[5], Is.EqualTo("Dummy03"));
            Assert.That(ordered[6], Is.EqualTo("Dummy 10"));
        }
    }
}
