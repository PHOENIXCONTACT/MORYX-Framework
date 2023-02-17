using System;
using Moryx.Products.Management;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests
{
    [TestFixture]
    public class StrategyConfigurationAttributeTests
    {
        private TextStrategyConfigurationAttribute _textStrategyAttribute;

        [SetUp]
        public void SetUp()
        {
            _textStrategyAttribute = new TextStrategyConfigurationAttribute();
        }

        [TestCase(typeof(string), 0, Description = "Checks the TextStrategy type compliance for Strings")]
        [TestCase(typeof(Guid), 0, Description = "Checks the TextStrategy type compliance for GUID")]
        [TestCase(typeof(object), 0, Description = "Checks the TextStrategy type compliance for Object")]
        [TestCase(typeof(SomeType), 1, Description = "Checks the TextStrategy type compliance for simple not inherited classes")]
        [TestCase(typeof(SomeInheritedType), 2, Description = "Checks the TextStrategy type compliance for a inherited classes")]
        [TestCase(typeof(ICloneable), int.MaxValue-1, Description = "Checks the TextStrategy type compliance for interfaces")]
        public void TextStrategyComplianceString(Type type, int expCompliance)
        {
            // Act
            var compliance = _textStrategyAttribute.TypeCompliance(type);

            // Assert
            Assert.AreEqual(expCompliance, compliance);
        }

        private class SomeType
        {
        }

        private class SomeInheritedType : SomeType
        {
        }
    }
}