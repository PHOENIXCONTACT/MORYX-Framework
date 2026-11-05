// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class EntryConvertEntryConverterTests
{
    [Test]
    public void EncodeObject_WithConverterOnProperty_TakesPrecedenceOverConverterOnType()
    {
        // Arrange
        var dummy = new ConverterDummy { ByType = new Tagged { Tag = "a" }, ByProperty = new Tagged { Tag = "b" } };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(SubEntry(encoded, nameof(ConverterDummy.ByType)).Value.Current, Is.EqualTo("type:a"));
            Assert.That(SubEntry(encoded, nameof(ConverterDummy.ByProperty)).Value.Current, Is.EqualTo("property:b"));
        });
    }

    [Test]
    public void EncodeObject_WithAnyConverter_MemberMetadataIsTakenFromProperty()
    {
        // Arrange
        var dummy = new ConverterDummy { ByType = new Tagged { Tag = "a" } };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var subEntry = SubEntry(encoded, nameof(ConverterDummy.ByType));
        Assert.Multiple(() =>
        {
            Assert.That(subEntry.Identifier, Is.EqualTo(nameof(ConverterDummy.ByType)));
            Assert.That(subEntry.DisplayName, Is.EqualTo(nameof(ConverterDummy.ByType)));
            Assert.That(subEntry.Value.Type, Is.EqualTo(EntryValueType.String));
            Assert.That(subEntry.SubEntries, Is.Empty);
        });
    }

    [Test]
    public void EncodeClass_WithAnyConverter_EncodesWithoutValue()
    {
        // Act
        var encoded = EntryConvert.EncodeClass(typeof(ConverterDummy));

        // Assert
        Assert.That(SubEntry(encoded, nameof(ConverterDummy.ByType)).Value.Current, Is.EqualTo("type:"));
    }

    [Test]
    public void UpdateInstance_WithAnyConverter_AssignsDecodedValue()
    {
        // Arrange
        var dummy = new ConverterDummy { ByType = new Tagged { Tag = "a" }, ByProperty = new Tagged { Tag = "b" } };
        var encoded = EntryConvert.EncodeObject(dummy);
        SubEntry(encoded, nameof(ConverterDummy.ByType)).Value.Current = "type:c";
        SubEntry(encoded, nameof(ConverterDummy.ByProperty)).Value.Current = "property:d";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dummy.ByType.Tag, Is.EqualTo("c"));
            Assert.That(dummy.ByProperty.Tag, Is.EqualTo("d"));
        });
    }

    [Test]
    public void EncodeObject_WithSerializationIgnoringConverters_UsesDefaultConversion()
    {
        // Arrange
        var dummy = new ConverterDummy { ByType = new Tagged { Tag = "a" } };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, new NoConverterSerialization());

        // Assert
        Assert.That(SubEntry(encoded, nameof(ConverterDummy.ByType)).Value.Type, Is.EqualTo(EntryValueType.Class));
    }

    [Test]
    public void InvokeMethod_WithConverterOnParameter_TakesPrecedenceOverConverterOnType()
    {
        // Arrange
        var dummy = new TaggedMethodDummy();
        var encoded = EntryConvert.EncodeMethod(typeof(TaggedMethodDummy).GetMethod(nameof(TaggedMethodDummy.Apply))!);
        var parameter = encoded.Parameters.SubEntries.Single();
        var encodedValue = parameter.Value.Current;
        parameter.Value.Current = "property:x";

        // Act
        EntryConvert.InvokeMethod(dummy, encoded);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encodedValue, Is.EqualTo("property:"));
            Assert.That(dummy.Received.Tag, Is.EqualTo("x"));
        });
    }

    [Test]
    public void EncodeObject_WithConverterOnCollection_ConvertsWholeCollection()
    {
        // Arrange
        var dummy = new CollectionConverterDummy { Whole = [new Tagged { Tag = "a" }, new Tagged { Tag = "b" }] };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var entry = SubEntry(encoded, nameof(CollectionConverterDummy.Whole));
        Assert.Multiple(() =>
        {
            Assert.That(entry.Value.Current, Is.EqualTo("a,b"));
            Assert.That(entry.SubEntries, Is.Empty);
        });
    }

    [Test]
    public void UpdateInstance_WithConverterOnCollection_DecodesWholeCollection()
    {
        // Arrange
        var dummy = new CollectionConverterDummy { Whole = [new Tagged { Tag = "a" }] };
        var encoded = EntryConvert.EncodeObject(dummy);
        SubEntry(encoded, nameof(CollectionConverterDummy.Whole)).Value.Current = "c,d";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Whole.Select(t => t.Tag), Is.EqualTo(["c", "d"]));
    }

    [Test]
    public void EncodeObject_WithItemConverter_ConvertsEachItem()
    {
        // Arrange
        var dummy = new CollectionConverterDummy
        {
            ByType = [new Tagged { Tag = "a" }],
            ByProperty = [new Tagged { Tag = "b" }]
        };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(SubEntry(encoded, nameof(CollectionConverterDummy.ByType)).SubEntries.Single().Value.Current, Is.EqualTo("type:a"));
            Assert.That(SubEntry(encoded, nameof(CollectionConverterDummy.ByProperty)).SubEntries.Single().Value.Current, Is.EqualTo("property:b"));
        });
    }

    [Test]
    public void EncodeObject_WithItemConverterWithoutPrototypes_UsesEmptyItemAsPrototype()
    {
        // Arrange
        var dummy = new CollectionConverterDummy { ByProperty = [] };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var entry = SubEntry(encoded, nameof(CollectionConverterDummy.ByProperty));
        var prototype = entry.Prototypes.Single();
        Assert.Multiple(() =>
        {
            Assert.That(prototype.Identifier, Is.EqualTo(nameof(Tagged)));
            Assert.That(prototype.Value.Current, Is.EqualTo("property:"));
            Assert.That(entry.Value.Possible.Select(p => p.Key), Is.EqualTo([nameof(Tagged)]));
        });
    }

    [Test]
    public void UpdateInstance_WithItemConverter_DecodesUpdatedAndAddedItems()
    {
        // Arrange
        var dummy = new CollectionConverterDummy { ByProperty = [new Tagged { Tag = "a" }] };
        var encoded = EntryConvert.EncodeObject(dummy);
        var entry = SubEntry(encoded, nameof(CollectionConverterDummy.ByProperty));
        entry.SubEntries.Single().Value.Current = "property:b";
        var added = entry.Prototypes.Single().Instantiate();
        added.Value.Current = "property:c";
        entry.SubEntries.Add(added);

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.ByProperty.Select(t => t.Tag), Is.EqualTo(["b", "c"]));
    }

    [Test]
    public void EncodeObject_WithItemsTargetButNoConverterType_Throws()
    {
        // Arrange
        var dummy = new MissingItemsConverterDummy { Items = [] };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => EntryConvert.EncodeObject(dummy));
    }

    [Test]
    public void EncodeObject_WithItemsTargetOnSingleValue_Throws()
    {
        // Arrange
        var dummy = new ItemsConverterOnSingleValueDummy { Single = new Tagged { Tag = "a" } };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => EntryConvert.EncodeObject(dummy));
    }

    private static Entry SubEntry(Entry entry, string identifier) =>
        entry.SubEntries.Single(e => e.Identifier == identifier);

    public class CollectionConverterDummy
    {
        public List<Tagged> ByType { get; set; }

        [EntrySerialize(ConverterType = typeof(PropertyTagConverter), ConverterTarget = EntryConverterTarget.Items)]
        public List<Tagged> ByProperty { get; set; }

        [EntrySerialize(ConverterType = typeof(TagListConverter))]
        public List<Tagged> Whole { get; set; }
    }

    public class MissingItemsConverterDummy
    {
        [EntrySerialize(ConverterTarget = EntryConverterTarget.Items)]
        public List<Tagged> Items { get; set; }
    }

    public class ItemsConverterOnSingleValueDummy
    {
        [EntrySerialize(ConverterType = typeof(PropertyTagConverter), ConverterTarget = EntryConverterTarget.Items)]
        public Tagged Single { get; set; }
    }

    public class TagListConverter : EntryConverter
    {
        public override bool CanConvert(Type memberType) => memberType == typeof(List<Tagged>);

        public override Entry Encode(object value, EntryConverterContext context) => new()
        {
            Value = new EntryValue
            {
                Type = EntryValueType.String,
                Current = string.Join(",", ((List<Tagged>)value ?? []).Select(t => t.Tag))
            }
        };

        public override object Decode(Entry entry, object currentValue, EntryConverterContext context) =>
            entry.Value.Current.Split(',').Select(tag => new Tagged { Tag = tag }).ToList();
    }

    public class TaggedMethodDummy
    {
        public Tagged Received { get; private set; }

        public void Apply([EntrySerialize(ConverterType = typeof(PropertyTagConverter))] Tagged tagged) => Received = tagged;
    }

    [EntrySerialize(ConverterType = typeof(TypeTagConverter))]
    public class Tagged
    {
        public string Tag { get; set; }
    }

    public class ConverterDummy
    {
        public Tagged ByType { get; set; }

        [EntrySerialize(ConverterType = typeof(PropertyTagConverter))]
        public Tagged ByProperty { get; set; }
    }

    public abstract class TagConverter(string prefix) : EntryConverter
    {
        public override bool CanConvert(Type memberType) => memberType == typeof(Tagged);

        public override Entry Encode(object value, EntryConverterContext context) => new()
        {
            Value = new EntryValue { Type = EntryValueType.String, Current = prefix + (value as Tagged)?.Tag }
        };

        public override object Decode(Entry entry, object currentValue, EntryConverterContext context) =>
            new Tagged { Tag = entry.Value.Current[prefix.Length..] };
    }

    public class TypeTagConverter() : TagConverter("type:");

    public class PropertyTagConverter() : TagConverter("property:");

    private sealed class NoConverterSerialization : DefaultSerialization
    {
        public override EntryConverter GetConverter(Type memberType, ICustomAttributeProvider attributeProvider) => null;
    }
}
