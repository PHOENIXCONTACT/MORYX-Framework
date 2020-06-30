// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;
using Marvin.Serialization;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Tests
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
            var valuesFromBase = attrWithBase.GetValues(null).ToArray();
            var valuesFromArray = attrWithArray.GetValues(null).ToArray();

            // Assert
            // Check values from base type
            Assert.AreEqual(2, valuesFromBase.Length);
            Assert.AreEqual(nameof(SomeBase), valuesFromBase[0]);
            Assert.AreEqual(nameof(SomeImpl), valuesFromBase[1]);

            // Check values from array types
            Assert.AreEqual(1, valuesFromArray.Length);
            Assert.AreEqual(nameof(SomeImpl), valuesFromArray[0]);
        }

        [Test(Description = "Uses the " + nameof(StateMachineKeysAttribute) + " to read possible state keys from the given state machine type.")]
        public void StateMachineKeysAttribute()
        {
            // Arrange
            var attr = new StateMachineKeysAttribute(typeof(MyStateBase));

            // Act
            var possibleValues = attr.GetValues(null).ToArray();

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
