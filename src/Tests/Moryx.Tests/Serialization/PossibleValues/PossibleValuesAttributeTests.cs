// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;
using Moryx.Serialization;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Tests.Serialization
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
            var attrWithArray = new PossibleTypesAttribute([typeof(SomeImpl)]);
            var attrWithFull = new PossibleTypesAttribute(typeof(SomeBase)) { UseFullname = true };

            // Act
            var valuesFromBase = attrWithBase.GetValues(null, null).ToArray();
            var valuesFromArray = attrWithArray.GetValues(null, null).ToArray();
            var valuesWithFull = attrWithFull.GetValues(null, null).ToArray();

            // Assert
            // Check values from base type
            Assert.That(valuesFromBase.Length, Is.EqualTo(2));
            Assert.That(valuesFromBase[0], Is.EqualTo(nameof(SomeBase)));
            Assert.That(valuesFromBase[1], Is.EqualTo(nameof(SomeImpl)));

            // Check values from array types
            Assert.That(valuesFromArray.Length, Is.EqualTo(1));
            Assert.That(valuesFromArray[0], Is.EqualTo(nameof(SomeImpl)));

            // Check values from base type with full name
            Assert.That(valuesWithFull.Length, Is.EqualTo(2));
            Assert.That(valuesWithFull[0], Is.EqualTo(typeof(SomeBase).FullName));
            Assert.That(valuesWithFull[1], Is.EqualTo(typeof(SomeImpl).FullName));
        }

        [Test(Description = "Uses the " + nameof(StateMachineKeysAttribute) + " to read possible state keys from the given state machine type.")]
        public void StateMachineKeysAttribute()
        {
            // Arrange
            var attr = new StateMachineKeysAttribute(typeof(MyStateBase));

            // Act
            var possibleValues = attr.GetValues(null, null).ToArray();

            // Assert
            Assert.That(possibleValues.Length, Is.EqualTo(3));
            Assert.That(possibleValues[0], Is.EqualTo(nameof(MyStateBase.StateA)));
            Assert.That(possibleValues[1], Is.EqualTo(nameof(MyStateBase.StateB)));
            Assert.That(possibleValues[2], Is.EqualTo(nameof(MyStateBase.StateC)));
        }

        public class SomeBase
        {

        }

        public class SomeImpl : SomeBase
        {

        }
    }
}
