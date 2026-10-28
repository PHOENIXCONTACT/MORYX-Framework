// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Numerics;
using System.Reflection;
using Moq;
using Moryx.Configuration;
using Moryx.Container;
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

    [TestCase(nameof(StructPropertiesClass.Position2D), 2, new[] { "X", "Y" })]
    [TestCase(nameof(StructPropertiesClass.Position3D), 3, new[] { "X", "Y", "Z" })]
    [TestCase(nameof(StructPropertiesClass.Position4D), 4, new[] { "X", "Y", "Z", "W" })]
    [TestCase(nameof(StructPropertiesClass.Rotation), 4, new[] { "X", "Y", "Z", "W" })]
    [TestCase(nameof(StructPropertiesClass.Surface), 4, new[] { "X", "Y", "Z", "D" })]
    public void EncodesStructWithSubEntries(string propertyName, int expectedCount, string[] expectedIdentifiers)
    {
        // Arrange
        var obj = new StructPropertiesClass
        {
            Position2D = new Vector2(10f, 20f),
            Position3D = new Vector3(1.5f, 2.5f, 3.5f),
            Position4D = new Vector4(1f, 2f, 3f, 4f),
            Rotation = new Quaternion(1f, 2f, 3f, 4f),
            Surface = new Plane(new Vector3(0f, 1f, 0f), 5f)
        };

        // Act
        var entry = EntryConvert.EncodeObject(obj);
        var structEntry = entry.SubEntries.First(e => e.Identifier == propertyName);

        // Assert
        Assert.That(structEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(structEntry.SubEntries, Has.Count.EqualTo(expectedCount));
        Assert.That(structEntry.SubEntries.Select(e => e.Identifier), Is.EquivalentTo(expectedIdentifiers));
        Assert.That(structEntry.SubEntries.All(e => e.Value.Type == EntryValueType.Single), Is.True);
    }

    private static IEnumerable<TestCaseData> StructRoundTripCases()
    {
        yield return new TestCaseData(new StructPropertiesClass { Position2D = new Vector2(10f, 20f) }, nameof(StructPropertiesClass.Position2D))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Vector2)");
        yield return new TestCaseData(new StructPropertiesClass { Position3D = new Vector3(1.5f, 2.5f, 3.5f) }, nameof(StructPropertiesClass.Position3D))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Vector3)");
        yield return new TestCaseData(new StructPropertiesClass { Position4D = new Vector4(1f, 2f, 3f, 4f) }, nameof(StructPropertiesClass.Position4D))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Vector4)");
        yield return new TestCaseData(new StructPropertiesClass { Rotation = new Quaternion(1f, 2f, 3f, 4f) }, nameof(StructPropertiesClass.Rotation))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Quaternion)");
        yield return new TestCaseData(new StructPropertiesClass { Surface = new Plane(new Vector3(0f, 1f, 0f), 5f) }, nameof(StructPropertiesClass.Surface))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Plane)");
    }

    [TestCaseSource(nameof(StructRoundTripCases))]
    public void StructValuesOnRoundTripPreserved(StructPropertiesClass original, string propertyName)
    {
        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new StructPropertiesClass();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        var property = typeof(StructPropertiesClass).GetProperty(propertyName)!;
        var originalValue = property.GetValue(original);
        var restoredValue = property.GetValue(restored);

        Assert.That(restoredValue, Is.EqualTo(originalValue));
    }

    [TestCase(nameof(StructPropertiesClass.Position2D), 2)]
    [TestCase(nameof(StructPropertiesClass.Position3D), 3)]
    [TestCase(nameof(StructPropertiesClass.Position4D), 4)]
    [TestCase(nameof(StructPropertiesClass.Rotation), 4)]
    [TestCase(nameof(StructPropertiesClass.Surface), 4)]
    public void CreatesDefaultSubEntriesOnEncodeClass(string propertyName, int expectedCount)
    {
        // Act
        var entry = EntryConvert.EncodeClass(typeof(StructPropertiesClass));
        var structEntry = entry.SubEntries.First(e => e.Identifier == propertyName);

        // Assert
        Assert.That(structEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(structEntry.SubEntries, Has.Count.EqualTo(expectedCount));
    }

    [Test]
    public void EncodeObject_WithStructAsRoot_EncodesSubEntries()
    {
        // Act
        var entry = EntryConvert.EncodeObject(new Vector3(1f, 2f, 3f));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entry.Identifier, Is.EqualTo(nameof(Vector3)));
            Assert.That(entry.Value.Type, Is.EqualTo(EntryValueType.Struct));
            Assert.That(entry.SubEntries.Select(e => e.Identifier), Is.EqualTo(["X", "Y", "Z"]));
        });
    }

    [Test]
    public void EncodeObject_WithStructCollection_EncodesSubEntriesPerElement()
    {
        // Arrange
        var dummy = new StructCollectionDummy { Positions = [new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f)] };

        // Act
        var entry = EntryConvert.EncodeObject(dummy);

        // Assert
        var elements = entry.SubEntries.Single(e => e.Identifier == nameof(StructCollectionDummy.Positions)).SubEntries;
        Assert.Multiple(() =>
        {
            Assert.That(elements, Has.Count.EqualTo(2));
            Assert.That(elements.All(e => e.Value.Type == EntryValueType.Struct), Is.True);
            Assert.That(elements.All(e => e.SubEntries.Count == 3), Is.True);
        });
    }

    [Test]
    public void EncodeObject_ForStructWithConvertersDisabled_DoesNotThrow()
    {
        // Arrange
        var obj = new StructPropertiesClass { Position3D = new Vector3(1f, 2f, 3f) };

        // Act & Assert
        Assert.DoesNotThrow(() => EntryConvert.EncodeObject(obj, new NoConverterSerialization()));
    }

    public class StructCollectionDummy
    {
        public List<Vector3> Positions { get; set; }
    }

    private sealed class NoConverterSerialization : DefaultSerialization
    {
        public override EntryConverter GetConverter(Type memberType, ICustomAttributeProvider attributeProvider) => null;
    }

    [Test(Description = "Decimal maps to EntryValueType.Double.")]
    public void DecimalMapsToDoubleValueType()
    {
        // Act
        var valueType = EntryConvert.TransformType(typeof(decimal));

        // Assert
        Assert.That(valueType, Is.EqualTo(EntryValueType.Double));
    }

    [Test(Description = "DateTime maps to EntryValueType.DateTime.")]
    public void DateTimeMapsToDateTimeValueType()
    {
        // Act
        var valueType = EntryConvert.TransformType(typeof(DateTime));

        // Assert
        Assert.That(valueType, Is.EqualTo(EntryValueType.DateTime));
    }

    [Test(Description = "DateTime values are converted to UTC and preserved on round-trip.")]
    public void DateTimeValuesOnRoundTripPreservedAsUtc()
    {
        // Arrange
        var original = new DummyClass
        {
            ModifiedAt = new DateTime(2026, 8, 18, 14, 30, 0, DateTimeKind.Utc)
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new DummyClass();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        Assert.That(restored.ModifiedAt, Is.EqualTo(original.ModifiedAt));
        Assert.That(restored.ModifiedAt.Kind, Is.EqualTo(DateTimeKind.Utc));
    }

    [Test(Description = "DateTimeOffset maps to EntryValueType.DateTime, same as DateTime.")]
    public void DateTimeOffsetMapsToDateTimeValueType()
    {
        // Act
        var valueType = EntryConvert.TransformType(typeof(DateTimeOffset));

        // Assert
        Assert.That(valueType, Is.EqualTo(EntryValueType.DateTime));
    }

    [Test(Description = "DateTimeOffset UTC instant is preserved on round-trip, but the original offset is lost.")]
    public void DateTimeOffsetValuesOnRoundTripPreserved()
    {
        // Arrange
        var original = new DummyClass
        {
            CreatedAt = new DateTimeOffset(2026, 8, 18, 14, 30, 0, TimeSpan.FromHours(2))
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new DummyClass();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        // Offset is lost, but the UTC instant is preserved
        Assert.That(restored.CreatedAt.UtcDateTime, Is.EqualTo(original.CreatedAt.UtcDateTime));
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

    [Test]
    public void ShouldProvidePossibleValue_UsingContext()
    {
        //Arrange
        var providerMock = new Mock<IServiceProvider>();
        var container = new Mock<IContainer>();
        var myObject = new InstanceAwareValuesClass
        {
            AllowedValues = ["a", "b", "c"]
        };
        var serialization = new PossibleValuesSerialization(container.Object, providerMock.Object, new ValueProviderExecutor(new ValueProviderExecutorSettings()));

        // Act
        var converted = EntryConvert.EncodeObject(myObject, serialization);

        //Assert
        Assert.That(converted.SubEntries.First(x => x.Identifier == nameof(InstanceAwareValuesClass.SelectedValue)).Value.Possible.Length, Is.EqualTo(3));
    }

    [Test(Description = "EncodeMethod passes instance to PossibleValues context for method parameters.")]
    public void PossibleValuesForMethodParametersUsingContext()
    {
        // Arrange
        var providerMock = new Mock<IServiceProvider>();
        var container = new Mock<IContainer>();
        var myObject = new InstanceAwareValuesClass
        {
            AllowedValues = ["x", "y"]
        };
        var serialization = new PossibleValuesSerialization(container.Object, providerMock.Object, new ValueProviderExecutor(new ValueProviderExecutorSettings()));

        // Act
        var methods = EntryConvert.EncodeMethods(myObject, serialization).ToArray();
        var doSomething = methods.First(m => m.Name == nameof(InstanceAwareValuesClass.DoSomething));

        // Assert
        var parameterPossible = doSomething.Parameters.SubEntries.First(x => x.Identifier == "value").Value.Possible;
        Assert.That(parameterPossible.Length, Is.EqualTo(2));
        Assert.That(parameterPossible.Select(p => p.Key), Is.EquivalentTo(new[] { "x", "y" }));
    }

    [Test]
    public void UpdateInstance_WithGetOnlyProperty_UpdatesInPlace()
    {
        // Arrange
        var dummy = new GetOnlyDummy();
        var encoded = EntryConvert.EncodeObject(dummy);
        encoded.SubEntries.Single(e => e.Identifier == nameof(GetOnlyDummy.Sub))
            .SubEntries.Single(e => e.Identifier == nameof(SubClass.Foo)).Value.Current = "42";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Sub.Foo, Is.EqualTo(42f));
    }

    public class GetOnlyDummy
    {
        public SubClass Sub { get; } = new();
    }
}
