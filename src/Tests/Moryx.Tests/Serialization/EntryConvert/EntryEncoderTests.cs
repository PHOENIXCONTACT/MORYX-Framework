// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the handling of <see cref="EntryEncoderBase"/>s within the
/// <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class EntryEncoderTests
{
    [Test]
    public void EncodeClass_WithAnyEncoder_EncodesWithoutValue()
    {
        // Act
        var encoded = EntryConvert.EncodeClass(typeof(EncoderMemberTargetsDummy));

        // Assert
        Assert.That(encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByType)).Value.Current, Is.EqualTo("type:"));
    }

    [Test]
    public void UpdateInstance_WithAnyEncoder_AssignsDecodedValue()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy
        {
            ByType = new EncoderTypeTargetDummy { Tag = "a" },
            ByProperty = new EncoderTypeTargetDummy { Tag = "b" }
        };
        var encoded = EntryConvert.EncodeObject(dummy);
        encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByType)).Value.Current = "type:c";
        encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByProperty)).Value.Current = "property:d";

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
    public void UpdateInstance_WithEncoderOnCollection_DecodesWholeCollection()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy { ByMemberEncoder = [new EncoderTypeTargetDummy { Tag = "a" }] };
        var encoded = EntryConvert.EncodeObject(dummy);
        encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByMemberEncoder)).Value.Current = "c,d";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.ByMemberEncoder.Select(t => t.Tag), Is.EqualTo(["c", "d"]));
    }

    [Test]
    public void UpdateInstance_WithItemEncoder_DecodesUpdatedAndAddedItems()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy { ByItemEncoder = [new EncoderTypeTargetDummy { Tag = "a" }] };
        var encoded = EntryConvert.EncodeObject(dummy);
        var entry = encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByItemEncoder));
        entry.SubEntries.Single().Value.Current = "property:b";
        var added = entry.Prototypes.Single().Instantiate();
        added.Value.Current = "property:c";
        entry.SubEntries.Add(added);

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.ByItemEncoder.Select(t => t.Tag), Is.EqualTo(["b", "c"]));
    }

    [Test]
    public void InvokeMethod_WithEncoderOnParameter_TakesPrecedenceOverEncoderOnType()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy();
        var encoded = EntryConvert.EncodeMethod(typeof(EncoderMemberTargetsDummy).GetMethod(nameof(EncoderMemberTargetsDummy.ByParameter))!);
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
    public void EncodeObject_WithEncoderOnProperty_TakesPrecedenceOverEncoderOnType()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy
        {
            ByType = new EncoderTypeTargetDummy { Tag = "a" },
            ByProperty = new EncoderTypeTargetDummy { Tag = "b" }
        };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByType)).Value.Current, Is.EqualTo("type:a"));
            Assert.That(encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByProperty)).Value.Current, Is.EqualTo("property:b"));
        });
    }

    [Test]
    public void EncodeObject_WithAnyEncoder_MemberMetadataIsTakenFromProperty()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy { ByType = new EncoderTypeTargetDummy { Tag = "a" } };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var subEntry = encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByType));
        Assert.Multiple(() =>
        {
            Assert.That(subEntry.Identifier, Is.EqualTo(nameof(EncoderMemberTargetsDummy.ByType)));
            Assert.That(subEntry.DisplayName, Is.EqualTo(nameof(EncoderMemberTargetsDummy.ByType)));
            Assert.That(subEntry.Value.Type, Is.EqualTo(EntryValueType.String));
            Assert.That(subEntry.SubEntries, Is.Empty);
        });
    }

    [Test]
    public void EncodeObject_WithSerializationIgnoringEncoders_UsesDefaultConversion()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy { ByType = new EncoderTypeTargetDummy { Tag = "a" } };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, new NoEncoderSerializationDummy());

        // Assert
        Assert.That(encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByType)).Value.Type, Is.EqualTo(EntryValueType.Class));
    }

    [Test]
    public void EncodeObject_WithEncoderOnCollection_ConvertsWholeCollection()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy { ByMemberEncoder = [new EncoderTypeTargetDummy { Tag = "a" }, new EncoderTypeTargetDummy { Tag = "b" }] };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var entry = encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByMemberEncoder));
        Assert.Multiple(() =>
        {
            Assert.That(entry.Value.Current, Is.EqualTo("a,b"));
            Assert.That(entry.SubEntries, Is.Empty);
        });
    }

    [Test]
    public void EncodeObject_WithItemEncoder_ConvertsEachItem()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy
        {
            ByItemType = [new EncoderTypeTargetDummy { Tag = "a" }],
            ByItemEncoder = [new EncoderTypeTargetDummy { Tag = "b" }]
        };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByItemType)).SubEntries.Single().Value.Current, Is.EqualTo("type:a"));
            Assert.That(encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByItemEncoder)).SubEntries.Single().Value.Current, Is.EqualTo("property:b"));
        });
    }

    [Test]
    public void EncodeObject_WithItemEncoderWithoutPrototypes_UsesEmptyItemAsPrototype()
    {
        // Arrange
        var dummy = new EncoderMemberTargetsDummy { ByItemEncoder = [] };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var entry = encoded.SubEntry(nameof(EncoderMemberTargetsDummy.ByItemEncoder));
        var prototype = entry.Prototypes.Single();
        Assert.Multiple(() =>
        {
            Assert.That(prototype.Identifier, Is.EqualTo(nameof(EncoderTypeTargetDummy)));
            Assert.That(prototype.Value.Current, Is.EqualTo("property:"));
            Assert.That(entry.Value.Possible.Select(p => p.Key), Is.EqualTo([nameof(EncoderTypeTargetDummy)]));
        });
    }

    [Test]
    public void EncodeObject_WithItemsTargetButNoEncoderType_Throws()
    {
        // Arrange
        var dummy = new MissingItemsEncoderDummy { Items = [] };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => EntryConvert.EncodeObject(dummy));
    }

    private sealed class MissingItemsEncoderDummy
    {
        [EntrySerialize(EncoderTarget = EntryEncoderTarget.Items)]
        public List<EncoderTypeTargetDummy> Items { get; set; }
    }
}
