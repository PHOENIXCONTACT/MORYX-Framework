// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Serialization;
using NUnit.Framework;
using static Moryx.Tests.Serialization.EntryConvertTestHelper;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the handling and behaviour of the <see cref="TypeEntryEncoder"/>s within the
/// <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class TypeEntryEncoderTests
{
    [Test]
    public void EncodeObject_ForTypeWithoutPossibleTypes_IsReadOnlyFullName()
    {
        // Arrange
        var dummy = new TypesDummy { PlainType = typeof(int) };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var subEntry = encoded.SubEntry(nameof(TypesDummy.PlainType));
        Assert.Multiple(() =>
        {
            Assert.That(subEntry.Value.Type, Is.EqualTo(EntryValueType.String));
            Assert.That(subEntry.Value.Current, Is.EqualTo(typeof(int).FullName));
            Assert.That(subEntry.Value.IsReadOnly, Is.True);
            Assert.That(subEntry.SubEntries, Is.Empty);
        });
    }

    [Test]
    public void UpdateInstance_OnTypeWithoutPossibleTypes_KeepsCurrentValue()
    {
        // Arrange
        var dummy = new TypesDummy { PlainType = typeof(int) };
        var encoded = EntryConvert.EncodeObject(dummy);
        encoded.SubEntry(nameof(TypesDummy.PlainType)).Value.Current = typeof(string).FullName;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.PlainType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void UpdateInstance_OnTypeWithoutPossibleTypesAndEmptyKey_KeepsCurrentValue()
    {
        // Arrange
        var dummy = new TypesDummy { PlainType = typeof(int) };
        var encoded = EntryConvert.EncodeObject(dummy);
        encoded.SubEntry(nameof(TypesDummy.PlainType)).Value.Current = string.Empty;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.PlainType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void UpdateInstance_OnStringWithPossibleTypes_AssignsTypeName()
    {
        // Arrange
        var serialization = CreatePossibleValuesSerialization();
        var dummy = new TypesDummy { TypeName = typeof(string).FullName };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        encoded.SubEntry(nameof(TypesDummy.TypeName)).Value.Current = typeof(int).FullName;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.TypeName, Is.EqualTo(typeof(int).FullName));
    }

    [Test]
    public void EncodeObject_OnTypeWithPossibleTypes_ProvidesPossibleTypeNames()
    {
        // Arrange
        var dummy = new TypesDummy { SelectableType = typeof(string) };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, CreatePossibleValuesSerialization());

        // Assert
        var subEntry = encoded.SubEntry(nameof(TypesDummy.SelectableType));
        Assert.Multiple(() =>
        {
            Assert.That(subEntry.Value.Current, Is.EqualTo(typeof(String).FullName));
            Assert.That(subEntry.Value.Possible.Select(p => p.Key), Is.EquivalentTo([typeof(String).FullName, typeof(Int32).FullName]));
            Assert.That(subEntry.Value.IsReadOnly, Is.False);
        });
    }

    [TestCase("System.Int32", typeof(int))]
    [TestCase("", null)]
    public void UpdateInstance_OnTypeWithPossibleTypes_ResolvesType(string key, Type expected)
    {
        // Arrange
        var serialization = CreatePossibleValuesSerialization();
        var dummy = new TypesDummy { SelectableType = typeof(string) };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        encoded.SubEntry(nameof(TypesDummy.SelectableType)).Value.Current = key;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.SelectableType, Is.EqualTo(expected));
    }

    [Test]
    public void UpdateInstance_OnTypeNotInPossibleTypes_Throws()
    {
        // Arrange
        var serialization = CreatePossibleValuesSerialization();
        var dummy = new TypesDummy { SelectableType = typeof(string) };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        encoded.SubEntry(nameof(TypesDummy.SelectableType)).Value.Current = typeof(double).FullName;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EntryConvert.UpdateInstance(dummy, encoded, serialization));
    }

    [Test]
    public void EncodeMethod_WithTypeParameterAndPossibleTypes_ProvidesPossibleTypeNames()
    {
        // Act
        var encoded = EntryConvert.EncodeMethod(SelectMethod, CreatePossibleValuesSerialization());

        // Assert
        var parameter = encoded.Parameters.SubEntries.Single();
        Assert.Multiple(() =>
        {
            Assert.That(parameter.Value.Type, Is.EqualTo(EntryValueType.String));
            Assert.That(parameter.Value.Possible.Select(p => p.Key), Is.EquivalentTo([typeof(string).FullName, typeof(int).FullName]));
            Assert.That(parameter.Value.IsReadOnly, Is.False);
            Assert.That(parameter.Validation.IsRequired, Is.True);
        });
    }

    [Test]
    public void InvokeMethod_WithTypeParameterAndPossibleTypes_ResolvesType()
    {
        // Arrange
        var serialization = CreatePossibleValuesSerialization();
        var dummy = new TypesDummy();
        var encoded = EntryConvert.EncodeMethod(SelectMethod, serialization);
        encoded.Parameters.SubEntries.Single().Value.Current = typeof(int).FullName;

        // Act
        EntryConvert.InvokeMethod(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.ReceivedType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void InvokeMethod_WithTypeParameterMissingPossibleTypes_PassesParameterDefault()
    {
        // Arrange
        var dummy = new TypesDummy();
        var encoded = EntryConvert.EncodeMethod(typeof(TypesDummy).GetMethod(nameof(TypesDummy.SelectAny))!);
        var parameter = encoded.Parameters.SubEntries.Single();
        parameter.Value.Current = typeof(int).FullName;

        // Act
        EntryConvert.InvokeMethod(dummy, encoded);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.Value.IsReadOnly, Is.True);
            Assert.That(dummy.ReceivedType, Is.Null);
        });
    }

    [Test]
    public void EncodeObject_OnTypeListWithPossibleTypes_ProvidesPossibleTypeNamesPerItem()
    {
        // Arrange
        var dummy = new TypesDummy { TypeList = [typeof(string)] };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, CreatePossibleValuesSerialization());

        // Assert
        var item = encoded.SubEntry(nameof(TypesDummy.TypeList)).SubEntries.Single();
        Assert.Multiple(() =>
        {
            Assert.That(item.Value.Type, Is.EqualTo(EntryValueType.String));
            Assert.That(item.Value.Current, Is.EqualTo(typeof(string).FullName));
            Assert.That(item.Value.Possible.Select(p => p.Key), Is.EquivalentTo([typeof(string).FullName, typeof(int).FullName]));
            Assert.That(item.Value.IsReadOnly, Is.False);
        });
    }

    [Test]
    public void UpdateInstance_OnTypeListWithPossibleTypes_ResolvesUpdatedAndAddedItems()
    {
        // Arrange
        var serialization = CreatePossibleValuesSerialization();
        var dummy = new TypesDummy { TypeList = [typeof(string)] };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        var entry = encoded.SubEntry(nameof(TypesDummy.TypeList));
        entry.SubEntries.Single().Value.Current = typeof(int).FullName;
        var added = entry.Prototypes.Single().Instantiate();
        added.Value.Current = typeof(string).FullName;
        entry.SubEntries.Add(added);

        // Act
        EntryConvert.UpdateInstance(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.TypeList, Is.EqualTo([typeof(int), typeof(string)]));
    }

    private static MethodInfo SelectMethod => typeof(TypesDummy).GetMethod(nameof(TypesDummy.Select))!;
}
