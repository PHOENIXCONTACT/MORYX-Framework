// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Numerics;
using System.Reflection;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the behaviour of the <see cref="NumericStructEntryEncoder{TStruct}"/>s
/// within the <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class NumericEntryEncoderTests
{
    [TestCase(nameof(NumericsDummy.Position2D), 2)]
    [TestCase(nameof(NumericsDummy.Position3D), 3)]
    [TestCase(nameof(NumericsDummy.Position4D), 4)]
    [TestCase(nameof(NumericsDummy.Rotation), 4)]
    [TestCase(nameof(NumericsDummy.Surface), 4)]
    public void CreatesDefaultSubEntriesOnEncodeClass(string propertyName, int expectedCount)
    {
        // Act
        var entry = EntryConvert.EncodeClass(typeof(NumericsDummy));
        var structEntry = entry.SubEntry(propertyName);

        // Assert
        Assert.That(structEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(structEntry.SubEntries, Has.Count.EqualTo(expectedCount));
    }

    [TestCase(nameof(NumericsDummy.Position2D), 2, new[] { "X", "Y" })]
    [TestCase(nameof(NumericsDummy.Position3D), 3, new[] { "X", "Y", "Z" })]
    [TestCase(nameof(NumericsDummy.Position4D), 4, new[] { "X", "Y", "Z", "W" })]
    [TestCase(nameof(NumericsDummy.Rotation), 4, new[] { "X", "Y", "Z", "W" })]
    [TestCase(nameof(NumericsDummy.Surface), 4, new[] { "X", "Y", "Z", "D" })]
    public void EncodesStructWithSubEntries(string propertyName, int expectedCount, string[] expectedIdentifiers)
    {
        // Arrange
        var obj = new NumericsDummy
        {
            Position2D = new Vector2(10f, 20f),
            Position3D = new Vector3(1.5f, 2.5f, 3.5f),
            Position4D = new Vector4(1f, 2f, 3f, 4f),
            Rotation = new Quaternion(1f, 2f, 3f, 4f),
            Surface = new Plane(new Vector3(0f, 1f, 0f), 5f)
        };

        // Act
        var entry = EntryConvert.EncodeObject(obj);

        // Assert
        var structEntry = entry.SubEntry(propertyName);
        Assert.That(structEntry.Value.Type, Is.EqualTo(EntryValueType.Struct));
        Assert.That(structEntry.SubEntries, Has.Count.EqualTo(expectedCount));
        Assert.That(structEntry.SubEntries.Select(e => e.Identifier), Is.EquivalentTo(expectedIdentifiers));
        Assert.That(structEntry.SubEntries.All(e => e.Value.Type == EntryValueType.Single), Is.True);
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
        var dummy = new NumericsDummy { Positions = [new Vector3(1f, 2f, 3f), new Vector3(4f, 5f, 6f)] };

        // Act
        var entry = EntryConvert.EncodeObject(dummy);

        // Assert
        var elements = entry.SubEntry(nameof(NumericsDummy.Positions)).SubEntries;
        Assert.Multiple(() =>
        {
            Assert.That(elements, Has.Count.EqualTo(2));
            Assert.That(elements.All(e => e.Value.Type == EntryValueType.Struct), Is.True);
            Assert.That(elements.All(e => e.SubEntries.Count == 3), Is.True);
        });
    }

    [Test]
    public void EncodeObject_WithEncodersDisabled_DoesNotThrow()
    {
        // Arrange
        var obj = new NumericsDummy { Position3D = new Vector3(1f, 2f, 3f) };

        // Act & Assert
        Assert.DoesNotThrow(() => EntryConvert.EncodeObject(obj, new NoEncoderSerializationDummy()));
    }

    [Test]
    public void EncodeMethod_WithStructParameter_EncodesSubEntries()
    {
        // Act
        var encoded = EntryConvert.EncodeMethod(MoveMethod);

        // Assert
        var parameter = encoded.Parameters.SubEntries.Single();
        Assert.Multiple(() =>
        {
            Assert.That(parameter.Identifier, Is.EqualTo("position"));
            Assert.That(parameter.Value.Type, Is.EqualTo(EntryValueType.Struct));
            Assert.That(parameter.SubEntries.Select(e => e.Identifier), Is.EqualTo(["X", "Y", "Z"]));
            Assert.That(parameter.Validation.IsRequired, Is.True);
        });
    }

    [Test]
    public void InvokeMethod_WithStructParameter_PassesEncodedValues()
    {
        // Arrange
        var dummy = new NumericsDummy();
        var encoded = EntryConvert.EncodeMethod(MoveMethod);
        var parameter = encoded.Parameters.SubEntries.Single();
        SetComponent(parameter, "X", "1");
        SetComponent(parameter, "Y", "2");
        SetComponent(parameter, "Z", "3");

        // Act
        EntryConvert.InvokeMethod(dummy, encoded);

        // Assert
        Assert.That(dummy.Position, Is.EqualTo(new Vector3(1f, 2f, 3f)));
    }

    [Test]
    public void InvokeMethod_WithStructCollectionParameter_PassesAddedElements()
    {
        // Arrange
        var dummy = new NumericsDummy();
        var encoded = EntryConvert.EncodeMethod(typeof(NumericsDummy).GetMethod(nameof(NumericsDummy.MoveAlong))!);
        var parameter = encoded.Parameters.SubEntries.Single();
        var added = parameter.Prototypes.Single().Instantiate();
        SetComponent(added, "X", "4");
        parameter.SubEntries.Add(added);

        // Act
        EntryConvert.InvokeMethod(dummy, encoded);

        // Assert
        Assert.That(dummy.Path, Is.EqualTo([new Vector3(4f, 0f, 0f)]));
    }

    private static IEnumerable<TestCaseData> StructRoundTripCases()
    {
        yield return new TestCaseData(new NumericsDummy { Position2D = new Vector2(10f, 20f) }, nameof(NumericsDummy.Position2D))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Vector2)");
        yield return new TestCaseData(new NumericsDummy { Position3D = new Vector3(1.5f, 2.5f, 3.5f) }, nameof(NumericsDummy.Position3D))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Vector3)");
        yield return new TestCaseData(new NumericsDummy { Position4D = new Vector4(1f, 2f, 3f, 4f) }, nameof(NumericsDummy.Position4D))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Vector4)");
        yield return new TestCaseData(new NumericsDummy { Rotation = new Quaternion(1f, 2f, 3f, 4f) }, nameof(NumericsDummy.Rotation))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Quaternion)");
        yield return new TestCaseData(new NumericsDummy { Surface = new Plane(new Vector3(0f, 1f, 0f), 5f) }, nameof(NumericsDummy.Surface))
            .SetName(nameof(StructValuesOnRoundTripPreserved) + "(Plane)");
    }

    [TestCaseSource(nameof(StructRoundTripCases))]
    public void StructValuesOnRoundTripPreserved(NumericsDummy original, string propertyName)
    {
        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new NumericsDummy();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        var property = typeof(NumericsDummy).GetProperty(propertyName)!;
        var originalValue = property.GetValue(original);
        var restoredValue = property.GetValue(restored);

        Assert.That(restoredValue, Is.EqualTo(originalValue));
    }

    [Test]
    public void UpdateInstance_WithStructCollection_DecodesUpdatedAndAddedElements()
    {
        // Arrange
        var dummy = new NumericsDummy { Positions = [new Vector3(1f, 2f, 3f)] };
        var encoded = EntryConvert.EncodeObject(dummy);
        var entry = encoded.SubEntry(nameof(NumericsDummy.Positions));
        SetComponent(entry.SubEntries.Single(), "X", "7");
        var added = entry.Prototypes.Single().Instantiate();
        SetComponent(added, "Z", "9");
        entry.SubEntries.Add(added);

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Positions, Is.EqualTo([new Vector3(7f, 2f, 3f), new Vector3(0f, 0f, 9f)]));
    }

    [Test]
    public void UpdateInstance_WithStructDictionary_DecodesValues()
    {
        // Arrange
        var dummy = new NumericsDummy { Named = new Dictionary<string, Vector3> { ["origin"] = Vector3.Zero } };
        var encoded = EntryConvert.EncodeObject(dummy);
        var value = encoded.SubEntry(nameof(NumericsDummy.Named)).SubEntries.Single();
        SetComponent(value, "Y", "5");

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Named["origin"], Is.EqualTo(new Vector3(0f, 5f, 0f)));
    }

    private static void SetComponent(Entry structEntry, string component, string value) =>
        structEntry.SubEntry(component).Value.Current = value;

    private static MethodInfo MoveMethod => typeof(NumericsDummy).GetMethod(nameof(NumericsDummy.Move))!;
}
