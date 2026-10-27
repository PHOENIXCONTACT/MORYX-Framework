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

    private static Entry SubEntry(Entry entry, string identifier) =>
        entry.SubEntries.Single(e => e.Identifier == identifier);

    [EntryConverter(typeof(TypeTagConverter))]
    public class Tagged
    {
        public string Tag { get; set; }
    }

    public class ConverterDummy
    {
        public Tagged ByType { get; set; }

        [EntryConverter(typeof(PropertyTagConverter))]
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
