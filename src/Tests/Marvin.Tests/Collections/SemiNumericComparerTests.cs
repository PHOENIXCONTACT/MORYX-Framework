using System.Linq;
using Marvin.Collections;
using NUnit.Framework;

namespace Marvin.Tests.Collections
{
    [TestFixture]
    public class SemiNumericComparerTests
    {
        [Test(Description = "Tests if the comparer orders the string values numerically.")]
        public void OrderStringsByIntValue()
        {
            // Arrange
            var stringNumbers = new[] { "105", "101", "102", "103", "90" };

            // Act
            var ordered = stringNumbers.OrderBy(s => s, new SemiNumericComparer()).ToArray();

            // Assert
            Assert.AreEqual("90", ordered[0]);
            Assert.AreEqual("101", ordered[1]);
            Assert.AreEqual("102", ordered[2]);
            Assert.AreEqual("103", ordered[3]);
            Assert.AreEqual("105", ordered[4]);
        }

        [Test(Description = "Tests if the comparer orders the string values first numerically and then by string")]
        public void OrderStringsByIntAndThenByString()
        {
            // Arrange
            var stringNumbers = new[] { "105", "101", "Hundi", "Wau", "Wau", "90" };

            // Act
            var ordered = stringNumbers.OrderBy(s => s, new SemiNumericComparer()).ToArray();

            // Assert
            Assert.AreEqual("90", ordered[0]);
            Assert.AreEqual("101", ordered[1]);
            Assert.AreEqual("105", ordered[2]);
            Assert.AreEqual("Hundi", ordered[3]);
            Assert.AreEqual("Wau", ordered[4]);
            Assert.AreEqual("Wau", ordered[5]);
        }
    }
}
