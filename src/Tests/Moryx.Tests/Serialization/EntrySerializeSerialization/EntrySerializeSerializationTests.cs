// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

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
        var sourceType = typeof(NotAttributedClassMixedMembers);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(1));

        var alwaysProperty = sourceType.GetProperty(nameof(NotAttributedClassMixedMembers.AlwaysProperty1));
        Assert.That(filteredProperties[0], Is.EqualTo(alwaysProperty), "Only the always property should be filtered.");
    }

    [Test(Description = "Attribute is not defined on class but mixed (always, never) on methods: Only always methods")]
    public void NotOnClassButMixedOnMethods()
    {
        // Arrange
        var sourceType = typeof(NotAttributedClassMixedMembers);

        // Act
        var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

        // Assert
        Assert.That(filteredMethods.Length, Is.EqualTo(1));

        var alwaysMethod = sourceType.GetMethod(nameof(NotAttributedClassMixedMembers.AlwaysMethod1));
        Assert.That(filteredMethods[0], Is.EqualTo(alwaysMethod), "Only the always method should be filtered.");
    }

    [Test(Description = "Attribute is not defined on class nor on properties: Serialize all")]
    public void NotOnClassAndNotOnProperties()
    {
        // Arrange
        var sourceType = typeof(NotAttributedClassOrMember);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(2), "Some property was not filtered.");
    }

    [Test(Description = "Attribute is not defined on class nor on methods: No methods")]
    public void NotOnClassAndNotOnMethods()
    {
        // Arrange
        var sourceType = typeof(NotAttributedClassOrMember);

        // Act
        var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

        // Assert
        Assert.That(filteredMethods.Length, Is.EqualTo(0), "No method should be filtered.");
    }

    [Test(Description = "Attribute is defined as 'Never' on class but not on any property: Serialize nothing")]
    public void NeverOnClassAndNotOnProperties()
    {
        // Arrange
        var sourceType = typeof(NeverAttributedClassNoMember);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(0), "No property should be selected.");
    }

    [Test(Description = "Attribute is defined as 'Never' on class but not on any property: Serialize nothing")]
    public void NeverOnClassAndNotOnMethods()
    {
        // Arrange
        var sourceType = typeof(NeverAttributedClassNoMember);

        // Act
        var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

        // Assert
        Assert.That(filteredMethods.Length, Is.EqualTo(0), "No method should be filtered.");
    }

    [Test(Description = "Attribute is defined as 'Never' on class but 'Always' property: Only always properties")]
    public void NeverOnClassButAlwaysProperties()
    {
        // Arrange
        var sourceType = typeof(NeverAttributedClassAlwaysAttributedMember);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(1));

        var alwaysProperty = sourceType.GetProperty(nameof(NeverAttributedClassAlwaysAttributedMember.AlwaysProperty1));
        Assert.That(filteredProperties[0], Is.EqualTo(alwaysProperty), "Only the always property should be filtered.");
    }

    [Test(Description = "Attribute is defined as 'Never' on class but 'Always' property: Only always methods")]
    public void NeverOnClassButAlwaysOnMethod()
    {
        // Arrange
        var sourceType = typeof(NeverAttributedClassAlwaysAttributedMember);

        // Act
        var filteredMethods = _serialization.GetMethods(sourceType).ToArray();

        // Assert
        Assert.That(filteredMethods.Length, Is.EqualTo(1));

        var alwaysMethod = sourceType.GetMethod(nameof(NeverAttributedClassAlwaysAttributedMember.AlwaysMethod1));
        Assert.That(filteredMethods[0], Is.EqualTo(alwaysMethod), "Only the always method should be filtered.");
    }

    [Test(Description = "Attribute is defined as 'Never' on base class: No property should be serialized")]
    public void NeverOnBaseClass()
    {
        // Arrange
        var sourceType = typeof(NotAttributedDerivedType);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(0));
    }

    [Test(Description = "Explicit properties are not filtered by default")]
    public void NotFilterExplicitPropertiesByDefault()
    {
        // Arrange
        var sourceType = typeof(ExplicitImplementation);

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
        var sourceType = typeof(ExplicitImplementation);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(1));

        var normalProperty = sourceType.GetProperty(nameof(ExplicitImplementation.NormalProperty));
        Assert.That(filteredProperties[0], Is.EqualTo(normalProperty));
    }

    [Test(Description = "Properties of given base type are filtered by default")]
    public void FilterByBaseType()
    {
        // Arrange
        var baseType = typeof(BaseType);
        var sourceType = typeof(DerivedType);
        _serialization = new EntrySerializeSerialization(baseType);

        // Act
        var filteredProperties = _serialization.GetProperties(sourceType).ToArray();

        // Assert
        Assert.That(filteredProperties.Length, Is.EqualTo(1));

        var baseTypeProperty = sourceType.GetProperty(nameof(BaseType.Property1));
        Assert.That(filteredProperties, Does.Not.Contain(baseTypeProperty));
    }

    [Test(Description = "Derived type does not override the EntrySerialize attribute of its base class")]
    public void ClassWithSerializationAttribute_AndBaseClass()
    {
        // Arrange
        var serialization = new EntrySerializeSerialization { FormatProvider = new CultureInfo("en-US") };
        var dummy = new AlwaysAttributedDerivedType();

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, serialization);

        // Assert
        var alwaysProperties = 1;
        Assert.That(alwaysProperties, Is.EqualTo(encoded.SubEntries.Count));
    }

    [Test(Description = "EntrySerialize attribute on class and members controls which properties are encoded")]
    public void ClassWithSerializationAttribute()
    {
        // Arrange
        var serialization = new EntrySerializeSerialization { FormatProvider = new CultureInfo("en-US") };
        var dummy = new AlwaysAttributedClassAndMember();

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, serialization);

        // Assert
        var alwaysProperties = 3;
        Assert.That(alwaysProperties, Is.EqualTo(encoded.SubEntries.Count));
    }
}
