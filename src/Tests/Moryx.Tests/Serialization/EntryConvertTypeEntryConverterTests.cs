// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moq;
using Moryx.Configuration;
using Moryx.Container;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class EntryConvertTypeEntryConverterTests
{
    [Test]
    public void EncodeObject_ForTypeWithoutPossibleTypes_IsReadOnlyFullName()
    {
        // Arrange
        var dummy = new TypeDummy { PlainType = typeof(int) };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var subEntry = SubEntry(encoded, nameof(TypeDummy.PlainType));
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
        var dummy = new TypeDummy { PlainType = typeof(int) };
        var encoded = EntryConvert.EncodeObject(dummy);
        SubEntry(encoded, nameof(TypeDummy.PlainType)).Value.Current = typeof(string).FullName;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.PlainType, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void UpdateInstance_OnTypeWithoutPossibleTypesAndEmptyKey_KeepsCurrentValue()
    {
        // Arrange
        var dummy = new TypeDummy { PlainType = typeof(int) };
        var encoded = EntryConvert.EncodeObject(dummy);
        SubEntry(encoded, nameof(TypeDummy.PlainType)).Value.Current = string.Empty;

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
        var dummy = new TypeNameDummy { TypeName = typeof(string).FullName };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        SubEntry(encoded, nameof(TypeNameDummy.TypeName)).Value.Current = typeof(int).FullName;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.TypeName, Is.EqualTo(typeof(int).FullName));
    }

    [Test]
    public void EncodeObject_OnTypeWithPossibleTypes_ProvidesPossibleTypeNames()
    {
        // Arrange
        var dummy = new TypeDummy { SelectableType = typeof(string) };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, CreatePossibleValuesSerialization());

        // Assert
        var subEntry = SubEntry(encoded, nameof(TypeDummy.SelectableType));
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
        var dummy = new TypeDummy { SelectableType = typeof(string) };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        SubEntry(encoded, nameof(TypeDummy.SelectableType)).Value.Current = key;

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
        var dummy = new TypeDummy { SelectableType = typeof(string) };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        SubEntry(encoded, nameof(TypeDummy.SelectableType)).Value.Current = typeof(double).FullName;

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
        var dummy = new TypeMethodDummy();
        var encoded = EntryConvert.EncodeMethod(SelectMethod, serialization);
        encoded.Parameters.SubEntries.Single().Value.Current = typeof(int).FullName;

        // Act
        EntryConvert.InvokeMethod(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.Received, Is.EqualTo(typeof(int)));
    }

    [Test]
    public void InvokeMethod_WithTypeParameterMissingPossibleTypes_PassesParameterDefault()
    {
        // Arrange
        var dummy = new TypeMethodDummy();
        var encoded = EntryConvert.EncodeMethod(typeof(TypeMethodDummy).GetMethod(nameof(TypeMethodDummy.SelectAny))!);
        var parameter = encoded.Parameters.SubEntries.Single();
        parameter.Value.Current = typeof(int).FullName;

        // Act
        EntryConvert.InvokeMethod(dummy, encoded);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parameter.Value.IsReadOnly, Is.True);
            Assert.That(dummy.Received, Is.Null);
        });
    }

    [Test]
    public void EncodeObject_OnTypeListWithPossibleTypes_ProvidesPossibleTypeNamesPerItem()
    {
        // Arrange
        var dummy = new TypeListDummy { Types = [typeof(string)] };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, CreatePossibleValuesSerialization());

        // Assert
        var item = SubEntry(encoded, nameof(TypeListDummy.Types)).SubEntries.Single();
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
        var dummy = new TypeListDummy { Types = [typeof(string)] };
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        var entry = SubEntry(encoded, nameof(TypeListDummy.Types));
        entry.SubEntries.Single().Value.Current = typeof(int).FullName;
        var added = entry.Prototypes.Single().Instantiate();
        added.Value.Current = typeof(string).FullName;
        entry.SubEntries.Add(added);

        // Act
        EntryConvert.UpdateInstance(dummy, encoded, serialization);

        // Assert
        Assert.That(dummy.Types, Is.EqualTo(new[] { typeof(int), typeof(string) }));
    }

    public class TypeListDummy
    {
        [PossibleTypes([typeof(string), typeof(int)], UseFullname = true)]
        public List<Type> Types { get; set; }
    }

    private static MethodInfo SelectMethod => typeof(TypeMethodDummy).GetMethod(nameof(TypeMethodDummy.Select))!;

    public class TypeMethodDummy
    {
        public Type Received { get; private set; } = typeof(object);

        public void Select([PossibleTypes([typeof(string), typeof(int)], UseFullname = true)] Type type) => Received = type;

        public void SelectAny(Type type) => Received = type;
    }

    public class TypeDummy
    {
        public Type PlainType { get; set; }

        [PossibleTypes([typeof(string), typeof(int)], UseFullname = true)]
        public Type SelectableType { get; set; }
    }

    public class TypeNameDummy
    {
        [PossibleTypes([typeof(string), typeof(int)], UseFullname = true)]
        public string TypeName { get; set; }
    }

    private static Entry SubEntry(Entry entry, string identifier) =>
        entry.SubEntries.Single(e => e.Identifier == identifier);

    private static PossibleValuesSerialization CreatePossibleValuesSerialization() =>
        new(new Mock<IContainer>().Object, new Mock<IServiceProvider>().Object, new ValueProviderExecutor(new ValueProviderExecutorSettings()));
}
