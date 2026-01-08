// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization
{
    [TestFixture]
    public class EntrySerializeSerializationTests
    {
        private EntrySerializeSerialization _serialization;

        [SetUp]
        public void SetUp()
        {
            _serialization = new EntrySerializeSerialization();
        }

        [Test(Description = "Attribute is not defined on class but mixed (always, never) on properties: Only always properties")]
        public void NotOnClassButMixedOnProperties()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NotClassMixed);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(1));

            var alwaysProperty = sourceType.GetProperty(nameof(EntrySerialize_NotClassMixed.AlwaysProperty1));
            Assert.That(filteredProperties[0], Is.EqualTo(alwaysProperty), "Only the always property should be filtered.");
        }

        [Test(Description = "Attribute is not defined on class but mixed (always, never) on methods: Only always methods")]
        public void NotOnClassButMixedOnMethods()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NotClassMixed);

            // Act
            var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

            // Assert
            Assert.That(filteredMethods.Length, Is.EqualTo(1));

            var alwaysMethod = sourceType.GetMethod(nameof(EntrySerialize_NotClassMixed.AlwaysMethod1));
            Assert.That(filteredMethods[0], Is.EqualTo(alwaysMethod), "Only the always method should be filtered.");
        }

        [Test(Description = "Attribute is not defined on class nor on properties: Serialize all")]
        public void NotOnClassAndNotOnProperties()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NoClassNoMember);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(2), "Some property was not filtered.");
        }

        [Test(Description = "Attribute is not defined on class nor on methods: No methods")]
        public void NotOnClassAndNotOnMethods()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NoClassNoMember);

            // Act
            var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

            // Assert
            Assert.That(filteredMethods.Length, Is.EqualTo(0), "No method should be filtered.");
        }

        [Test(Description = "Attribute is defined as 'Never' on class but not on any property: Serialize nothing")]
        public void NeverOnClassAndNotOnProperties()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NeverClassNoMember);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(0), "No property should be selected.");
        }

        [Test(Description = "Attribute is defined as 'Never' on class but not on any property: Serialize nothing")]
        public void NeverOnClassAndNotOnMethods()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NeverClassNoMember);

            // Act
            var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

            // Assert
            Assert.That(filteredMethods.Length, Is.EqualTo(0), "No method should be filtered.");
        }

        [Test(Description = "Attribute is defined as 'Never' on class but 'Always' property: Only always properties")]
        public void NeverOnClassButAlwaysProperties()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NeverClassAlwaysMember);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(1));

            var alwaysProperty = sourceType.GetProperty(nameof(EntrySerialize_NeverClassAlwaysMember.AlwaysProperty1));
            Assert.That(filteredProperties[0], Is.EqualTo(alwaysProperty), "Only the always property should be filtered.");
        }

        [Test(Description = "Attribute is defined as 'Never' on class but 'Always' property: Only always methods")]
        public void NeverOnClassButAlwaysOnMethod()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_NeverClassAlwaysMember);

            // Act
            var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

            // Assert
            Assert.That(filteredMethods.Length, Is.EqualTo(1));

            var alwaysMethod = sourceType.GetMethod(nameof(EntrySerialize_NeverClassAlwaysMember.AlwaysMethod1));
            Assert.That(filteredMethods[0], Is.EqualTo(alwaysMethod), "Only the always method should be filtered.");
        }

        [Test(Description = "Attribute is defined as 'Never' on base class: No property should be serialized")]
        public void NeverOnBaseClass()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_Inherited);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(0));
        }

        [Test(Description = "Explicit properties are not filtered by default")]
        public void NotFilterExplicitPropertiesByDefault()
        {
            // Arrange
            var sourceType = typeof(EntrySerialize_Explicit);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(2));
        }

        [Test(Description = "Explicit properties are filtered if enabled")]
        public void FilterExplicitPropertiesIfEnabled()
        {
            // Arrange
            _serialization.FilterExplicitProperties = true;
            var sourceType = typeof(EntrySerialize_Explicit);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(1));

            var normalProperty = sourceType.GetProperty(nameof(EntrySerialize_Explicit.NormalProperty));
            Assert.That(filteredProperties[0], Is.EqualTo(normalProperty));
        }

        [Test(Description = "Properties of given base type are filtered by default")]
        public void FilterByBaseType()
        {
            // Arrange
            var baseType = typeof(EntrySerialize_BaseType);
            var sourceType = typeof(EntrySerialize_DerivedType);
            _serialization = new EntrySerializeSerialization(baseType);

            // Act
            var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

            // Assert
            Assert.That(filteredProperties.Length, Is.EqualTo(1));

            var baseTypeProperty = sourceType.GetProperty(nameof(EntrySerialize_BaseType.Property1));
            Assert.That(filteredProperties, Does.Not.Contain(baseTypeProperty));
        }
    }
}