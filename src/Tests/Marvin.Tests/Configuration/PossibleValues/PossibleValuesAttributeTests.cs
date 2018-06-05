using System.IO;
using System.Linq;
using Marvin.Configuration;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Tests.Configuration
{
    [TestFixture]
    public class PossibleValuesAttributeTests
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ReflectionTool.TestMode = true;
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Test(Description = "Uses the " + nameof(PossibleTypesAttribute) + " to read possible type names from given types or baseType.")]
        public void PossibleTypesAttribute()
        {
            // Arrange
            var attrWithBase = new PossibleTypesAttribute(typeof(SomeBase));
            var attrWithArray = new PossibleTypesAttribute(new[] {typeof(SomeImpl) });

            // Act
            var valuesFromBase = attrWithBase.ResolvePossibleValues(null).ToArray();
            var valuesFromArray = attrWithArray.ResolvePossibleValues(null).ToArray();
            var instance = attrWithBase.ConvertToConfigValue(null, nameof(SomeBase));

            // Assert
            // Check values from base type
            Assert.AreEqual(2, valuesFromBase.Length);
            Assert.AreEqual(nameof(SomeBase), valuesFromBase[0]);
            Assert.AreEqual(nameof(SomeImpl), valuesFromBase[1]);

            // Check values from array types
            Assert.AreEqual(1, valuesFromArray.Length);
            Assert.AreEqual(nameof(SomeImpl), valuesFromArray[0]);

            // Check instance
            Assert.NotNull(instance);
            Assert.AreEqual(instance.GetType(), typeof(SomeBase));
        }

        [Test(Description = "Uses the " + nameof(StateMachineKeysAttribute) + " to read possible state keys from the given state machine type.")]
        public void StateMachineKeysAttribute()
        {
            // Arrange
            var attr = new StateMachineKeysAttribute(typeof(MyStateBase));

            // Act
            var possibleValues = attr.ResolvePossibleValues(null).ToArray();

            // Assert
            Assert.AreEqual(3, possibleValues.Length);
            Assert.AreEqual(nameof(MyStateBase.StateA), possibleValues[0]);
            Assert.AreEqual(nameof(MyStateBase.StateB), possibleValues[1]);
            Assert.AreEqual(nameof(MyStateBase.StateC), possibleValues[2]);
        }


        public class SomeBase
        {

        }

        public class SomeImpl : SomeBase
        {

        }
    }
}