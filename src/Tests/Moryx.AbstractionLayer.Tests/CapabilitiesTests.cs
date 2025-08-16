using Moryx.AbstractionLayer.Resources;
using NUnit.Framework;
using System;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class CapabilitiesTests
    {

        [Test]
        public void AvailableCapabilitiesAttributeCanHandleDuplicateClassnames()
        {
            var sut = new SystemUnderTest();
            var propertyInfo = sut.GetType().GetProperty(nameof(SystemUnderTest.TheType));
            var attribute = propertyInfo.GetCustomAttributes(typeof(AvailableCapabilitiesAttribute), false)[0] as AvailableCapabilitiesAttribute;

            var values = attribute.GetValues(null, null);
            Assert.That(values, Is.Not.Null.And.Count.EqualTo(2));
        }
    }

    public class SystemUnderTest
    {
        [AvailableCapabilities(typeof(Resources.TestData.NamespaceA.SameNameCapabilities), typeof(Resources.TestData.NamespaceB.SameNameCapabilities))]
        public Type TheType { get; set; }
    }
}
