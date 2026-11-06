// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class EntryTests
{
    [Test(Description = "Make sure generated clone method is complete")]
    public void GeneratedClone()
    {
        // Arrange
        var dummyClass = ClassDummy.Populate();
        var root = EntryConvert.EncodeObject(dummyClass);

        // Act
        var clone = root.Clone(true);

        // Assert
        AssertValueEquality(root, clone);
    }

    [Test]
    public void EntryEqualsAfterCloning()
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var clone = entry.Clone(true);

        // Act
        var equals = Entry.ValuesEqual(entry, clone);

        // Assert
        Assert.That(equals);
    }

    [Test]
    public void EntryEqualsWhenCheckingSameInstance()
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var entry2 = EntryConvert.EncodeObject(ClassDummy.Populate());

        // Act
        var equals = Entry.ValuesEqual(entry, entry2);

        // Assert
        Assert.That(equals);
    }

    [Test]
    public void EntryDoesNotEqualWhenValueOnRootIsDifferent()
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var entry2 = EntryConvert.EncodeObject(ClassDummy.Populate());

        entry.Value.Current = "OtherValue";

        // Act
        var equals = Entry.ValuesEqual(entry, entry2);

        // Assert
        Assert.That(equals, Is.False);
    }

    [Test]
    public void EntryDoesNotEqualWhenNoSubEntriesArePresent()
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var entry2 = EntryConvert.EncodeObject(ClassDummy.Populate());

        entry2.SubEntries = [];

        // Act
        var equals = entry.Equals(entry2);

        // Assert
        Assert.That(equals, Is.False);
    }

    [TestCase(false, Description = "ValuesEqual returns false when sub-entries have different order")]
    [TestCase(true, Description = "ValuesEqual returns true when sub-entries have same order")]
    public void ValuesEqualRespectsSubEntryOrder(bool sameOrder)
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var entry2 = EntryConvert.EncodeObject(ClassDummy.Populate());

        if (!sameOrder)
        {
            (entry2.SubEntries[0], entry2.SubEntries[1]) = (entry2.SubEntries[1], entry2.SubEntries[0]);
        }

        // Act
        var equals = Entry.ValuesEqual(entry, entry2, orderSensitive: true);

        // Assert
        Assert.That(equals, Is.EqualTo(sameOrder));
    }

    [Test]
    public void EntryDoesNotEqualWhenWhenASubEntryIsDifferent()
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var entry2 = EntryConvert.EncodeObject(ClassDummy.Populate());

        entry2.SubEntries[1].Value.Current = "Value";

        // Act
        var equals = entry.Equals(entry2);

        // Assert
        Assert.That(equals, Is.False);
    }

    [Test]
    public void EntryDoesNotEqualWhenWhenADeepSubEntryIsDifferent()
    {
        // Arrange
        var entry = EntryConvert.EncodeObject(ClassDummy.Populate());
        var entry2 = EntryConvert.EncodeObject(ClassDummy.Populate());

        entry2.SubEntry(nameof(ClassDummy.SubArray)).SubEntries[0].Value.Current = "4.2";

        // Act
        var equals = entry.Equals(entry2);

        // Assert
        Assert.That(equals, Is.False);
    }

    private static void AssertValueEquality(Entry expected, Entry value)
    {
        Assert.That(value.DisplayName, Is.EqualTo(expected.DisplayName));
        Assert.That(value.Identifier, Is.EqualTo(expected.Identifier));
        Assert.That(value.Description, Is.EqualTo(expected.Description));

        Assert.That(value.Value.Type, Is.EqualTo(expected.Value.Type));
        Assert.That(value.Value.UnitType, Is.EqualTo(expected.Value.UnitType));
        Assert.That(value.Value.Current, Is.EqualTo(expected.Value.Current));
        Assert.That(value.Value.Default, Is.EqualTo(expected.Value.Default));
        Assert.That(value.Value.IsReadOnly, Is.EqualTo(expected.Value.IsReadOnly));
        Assert.That(value.Value.Possible?.Select(p => (p.Key, p.DisplayName, p.Description)),
            Is.EqualTo(expected.Value.Possible?.Select(p => (p.Key, p.DisplayName, p.Description))));

        if (expected.Validation is null)
        {
            Assert.That(value.Validation, Is.Null);
        }
        else
        {
            Assert.That(value.Validation, Is.Not.Null);
            Assert.That(value.Validation.DataType, Is.EqualTo(expected.Validation.DataType));
            Assert.That(value.Validation.Minimum, Is.EqualTo(expected.Validation.Minimum));
            Assert.That(value.Validation.Maximum, Is.EqualTo(expected.Validation.Maximum));
            Assert.That(value.Validation.Regex, Is.EqualTo(expected.Validation.Regex));
            Assert.That(value.Validation.IsRequired, Is.EqualTo(expected.Validation.IsRequired));
        }

        Assert.That(value.SubEntries.Count, Is.EqualTo(expected.SubEntries.Count));
        for (var i = 0; i < expected.SubEntries.Count; i++)
        {
            AssertValueEquality(expected.SubEntries[i], value.SubEntries[i]);
        }

        Assert.That(value.Prototypes.Count, Is.EqualTo(expected.Prototypes.Count));
        for (var i = 0; i < expected.Prototypes.Count; i++)
        {
            AssertValueEquality(expected.Prototypes[i], value.Prototypes[i]);
        }
    }
}
