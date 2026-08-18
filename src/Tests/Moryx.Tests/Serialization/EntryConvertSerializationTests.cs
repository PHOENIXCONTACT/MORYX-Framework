// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class EntryConvertSerializationTests
{
    [Test]
    public void ParameterWithNoDefaultValueShouldHaveValidation_IsRequired()
    {
        // Arrange
        var myClass = typeof(EntrySerialize_Methods);
        var myMethodWithParameters = myClass.GetMethods().FirstOrDefault(x => x.GetParameters().Length > 0);
        const string parameter1Name = "intValue";
        const string parameter2Name = "stringValue1";
        const string parameter3Name = "stringValue2";

        // Act
        var entry = EntryConvert.EncodeMethod(myMethodWithParameters);
        var parameter1Validation = entry.Parameters.SubEntries.FirstOrDefault(x => x.DisplayName == parameter1Name).Validation;
        var parameter2Validation = entry.Parameters.SubEntries.FirstOrDefault(x => x.DisplayName == parameter2Name).Validation;
        var parameter3Validation = entry.Parameters.SubEntries.FirstOrDefault(x => x.DisplayName == parameter3Name).Validation;

        // Assert
        Assert.That(parameter1Validation.IsRequired, Is.True);
        Assert.That(parameter2Validation.IsRequired, Is.True);
        Assert.That(parameter3Validation.IsRequired, Is.False); // parameter with default value is not required
    }

    [Test]
    public void ShouldEncodeClass_Properties()
    {
        // Act
        var entry = EntryConvert.EncodeClass(typeof(DummyClass));

        // Assert
        Assert.That(entry.SubEntries.FirstOrDefault(x => x.DisplayName == nameof(DummyClass.Number)) != null, Is.True);
    }

    [Test]
    public void ShouldEncodeObject_PropertyWithValue()
    {
        // Arrange
        var myObject = new DummyClass();
        myObject.Number = 10;

        // Act
        var entry = EntryConvert.EncodeObject(myObject);

        // Assert
        Assert.That(entry.SubEntries.FirstOrDefault(x => x.DisplayName == nameof(DummyClass.Number)).Value.Current,
            Is.EqualTo(myObject.Number.ToString(Thread.CurrentThread.CurrentCulture)));
    }

    [Test]
    public void ToObject_UsesProvidedFormatProvider_ForFloatDoubleDecimal()
    {
        // Arrange
        var value = "1234,56"; var provider = new CultureInfo("de-DE"); // uses comma as decimal separator

        // Act
        var floatResult = (float)EntryConvert.ToObject(typeof(float), value, provider);
        var doubleResult = (double)EntryConvert.ToObject(typeof(double), value, provider);
        var decimalResult = (decimal)EntryConvert.ToObject(typeof(decimal), value, provider);

        // Assert
        Assert.That(floatResult, Is.EqualTo(1234.56f).Within(1e-5));
        Assert.That(doubleResult, Is.EqualTo(1234.56d).Within(1e-10));
        Assert.That(decimalResult, Is.EqualTo(1234.56m));
    }

    [Test]
    public void ToObject_FallsBackToInvariantCulture_WhenProviderFails_ForFloatDoubleDecimal()
    {
        // Arrange
        var value = "1234.56";
        var provider = new CultureInfo("de-DE"); // dot fails in de-DE, should fallback to invariant

        // Act
        var floatResult = (float)EntryConvert.ToObject(typeof(float), value, provider);
        var doubleResult = (double)EntryConvert.ToObject(typeof(double), value, provider);
        var decimalResult = (decimal)EntryConvert.ToObject(typeof(decimal), value, provider);

        // Assert
        Assert.That(floatResult, Is.EqualTo(1234.56f).Within(1e-5));
        Assert.That(doubleResult, Is.EqualTo(1234.56d).Within(1e-10));
        Assert.That(decimalResult, Is.EqualTo(1234.56m));
    }

    [Test]
    public void ToObject_ThrowsFormatException_WhenParsingFails()
    {
        // Arrange
        var value = "not-a-number";

        // Act + Assert
        Assert.Throws<FormatException>(() =>
            EntryConvert.ToObject(typeof(float), value, CultureInfo.InvariantCulture));

        Assert.Throws<FormatException>(() =>
            EntryConvert.ToObject(typeof(decimal), value, CultureInfo.InvariantCulture));

        Assert.Throws<FormatException>(() =>
            EntryConvert.ToObject(typeof(double), value, CultureInfo.InvariantCulture));
    }

    [Test]
    public void ToObject_ByEntryValueType_AlsoUsesParseWithFallback()
    {
        // Arrange
        var provider = new CultureInfo("de-DE");
        var valueComma = "3,14"; // should parse with provider
        var valueDot = "3.14";   // should parse via fallback to invariant

        // Act
        var singleWithProvider = (float)EntryConvert.ToObject(EntryValueType.Single, valueComma, provider);
        var doubleWithFallback = (double)EntryConvert.ToObject(EntryValueType.Double, valueDot, provider);

        // Assert
        Assert.That(singleWithProvider, Is.EqualTo(3.14f).Within(1e-5));
        Assert.That(doubleWithFallback, Is.EqualTo(3.14d).Within(1e-10));
    }

    [Test]
    public void EncodesVector3AsStructWithSubEntries()
    {
        // Arrange
        var obj = new StructPropertiesClass { Position3D = new Vector3(1.5f, 2.5f, 3.5f) };

        // Act
        var entry = EntryConvert.EncodeObject(obj);
        var vectorEntry = entry.SubEntries.First(e => e.Identifier == nameof(StructPropertiesClass.Position3D));

        // Assert
        Assert.That(vectorEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(vectorEntry.SubEntries, Has.Count.EqualTo(3));
        Assert.That(vectorEntry.SubEntries.Select(e => e.Identifier), Is.EquivalentTo(new[] { "X", "Y", "Z" }));
        Assert.That(vectorEntry.SubEntries.All(e => e.Value.Type == EntryValueType.Single), Is.True);
    }

    [Test]
    public void EncodesVector2AsStructWithSubEntries()
    {
        // Arrange
        var obj = new StructPropertiesClass { Position2D = new Vector2(10f, 20f) };

        // Act
        var entry = EntryConvert.EncodeObject(obj);
        var vectorEntry = entry.SubEntries.First(e => e.Identifier == nameof(StructPropertiesClass.Position2D));

        // Assert
        Assert.That(vectorEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(vectorEntry.SubEntries, Has.Count.EqualTo(2));
        Assert.That(vectorEntry.SubEntries.Select(e => e.Identifier), Is.EquivalentTo(new[] { "X", "Y" }));
    }

    [Test]
    public void EncodesQuaternionAsStructWithSubEntries()
    {
        // Arrange
        var obj = new StructPropertiesClass { Rotation = new Quaternion(1f, 2f, 3f, 4f) };

        // Act
        var entry = EntryConvert.EncodeObject(obj);
        var quatEntry = entry.SubEntries.First(e => e.Identifier == nameof(StructPropertiesClass.Rotation));

        // Assert
        Assert.That(quatEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(quatEntry.SubEntries, Has.Count.EqualTo(4));
        Assert.That(quatEntry.SubEntries.Select(e => e.Identifier), Is.EquivalentTo(new[] { "X", "Y", "Z", "W" }));
    }

    [Test]
    public void Vector3ValuesOnRoundTripPreserved()
    {
        // Arrange
        var original = new StructPropertiesClass
        {
            Position3D = new Vector3(1.5f, 2.5f, 3.5f)
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new StructPropertiesClass();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        Assert.That(restored.Position3D, Is.EqualTo(original.Position3D));
    }

    [Test]
    public void Vector2ValuesOnRoundTripPreserved()
    {
        // Arrange
        var original = new StructPropertiesClass
        {
            Position2D = new Vector2(10f, 20f)
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new StructPropertiesClass();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        Assert.That(restored.Position2D, Is.EqualTo(original.Position2D));
    }

    [Test]
    public void QuaternionValuesOnRoundTripPreserved()
    {
        // Arrange
        var original = new StructPropertiesClass
        {
            Rotation = new Quaternion(1f, 2f, 3f, 4f)
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new StructPropertiesClass();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        Assert.That(restored.Rotation, Is.EqualTo(original.Rotation));
    }

    [Test]
    public void CreatesDefaultSubEntriesForVector3OnEncodeClass()
    {
        // Act
        var entry = EntryConvert.EncodeClass(typeof(StructPropertiesClass));
        var vectorEntry = entry.SubEntries.First(e => e.Identifier == nameof(StructPropertiesClass.Position3D));

        // Assert
        Assert.That(vectorEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(vectorEntry.SubEntries, Has.Count.EqualTo(3));
    }

    [Test]
    public void ParametersShouldRespectRequiredAttribute()
    {
        // Arrange
        var myClass = typeof(EntrySerialize_Methods);
        var method = myClass.GetMethod(nameof(EntrySerialize_Methods.MethodWithRequiredAndOptionalParameters));

        // Act
        var entry = EntryConvert.EncodeMethod(method);

        var plainParameterValidation = entry.Parameters.SubEntries.First(x => x.DisplayName == "plainParameter").Validation;
        var requiredParameterValidation = entry.Parameters.SubEntries.First(x => x.DisplayName == "requiredParameter").Validation;
        var nullableValidation = entry.Parameters.SubEntries.First(x => x.DisplayName == "nullableString").Validation;
        var defaultValueValidation = entry.Parameters.SubEntries.First(x => x.DisplayName == "defaultValueString").Validation;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(plainParameterValidation.IsRequired, Is.True ,"Plain parameter should be reuired");
            Assert.That(requiredParameterValidation.IsRequired, Is.True , "Required parameter should be required");
            Assert.That(nullableValidation.IsRequired, Is.False, "Nullable parameter should not be required");
            Assert.That(defaultValueValidation.IsRequired, Is.False, "Default value shuold not be required");
        });
    }

}
