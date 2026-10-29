// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Products;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class IdentityEntryConverterTests
{
    [Test]
    public void Encode_ProductIdentity_EncodesTypeAndConstructorArguments()
    {
        // Arrange
        var dummy = new IdentityDummy { Identity = new ProductIdentity("4711", 5) };

        // Act
        var entry = IdentityEntry(EntryConvert.EncodeObject(dummy));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entry.Value.Type, Is.EqualTo(EntryValueType.Class));
            Assert.That(entry.Value.Current, Is.EqualTo(typeof(ProductIdentity).FullName));
            Assert.That(entry.Value.IsReadOnly, Is.False);
            Assert.That(entry.SubEntries.Select(e => (e.Identifier, e.Value.Current)),
                Is.EqualTo([("identifier", "4711"), ("revision", "5")]));
        });
    }

    [Test]
    public void Encode_ProvidesPossibleIdentityTypesAsPrototypes()
    {
        // Arrange
        var dummy = new IdentityDummy { Identity = new ProductIdentity("4711", 5) };
        string[] expected = [typeof(ProductIdentity).FullName, typeof(BatchIdentity).FullName, typeof(NullIdentity).FullName];

        // Act
        var entry = IdentityEntry(EntryConvert.EncodeObject(dummy));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entry.Value.Possible.Select(p => p.Key), Is.SupersetOf(expected));
            Assert.That(entry.Prototypes.Select(p => p.Identifier), Is.EquivalentTo(entry.Value.Possible.Select(p => p.Key)));
        });
    }

    [Test]
    public void Encode_IdentityListWithItemConverter_ProvidesCandidatesAsPrototypes()
    {
        // Arrange
        var dummy = new IdentityDummy { Identities = [new ProductIdentity("4711", 5)] };
        string[] expected = [typeof(ProductIdentity).FullName, typeof(BatchIdentity).FullName, typeof(NullIdentity).FullName];

        // Act
        var entry = IdentitiesEntry(EntryConvert.EncodeObject(dummy));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(entry.SubEntries.Single().Value.Current, Is.EqualTo(typeof(ProductIdentity).FullName));
            Assert.That(entry.Prototypes.Select(p => p.Identifier), Is.SupersetOf(expected));
            Assert.That(entry.Value.Possible.Select(p => p.Key), Is.EqualTo(entry.Prototypes.Select(p => p.Identifier)));
        });
    }

    [Test]
    public void Decode_ChangedArguments_CreatesNewIdentity()
    {
        // Arrange
        var dummy = new IdentityDummy { Identity = new ProductIdentity("4711", 5) };
        var encoded = EntryConvert.EncodeObject(dummy);
        IdentityEntry(encoded).SubEntries.Single(e => e.Identifier == "revision").Value.Current = "6";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Identity, Is.EqualTo(new ProductIdentity("4711", 6)));
    }

    [Test]
    public void Decode_PrototypeOfOtherType_ChangesIdentityType()
    {
        // Arrange
        var dummy = new IdentityDummy { Identity = new ProductIdentity("4711", 5) };
        var encoded = EntryConvert.EncodeObject(dummy);
        var entry = IdentityEntry(encoded);

        var replacement = entry.Prototypes.Single(p => p.Identifier == typeof(BatchIdentity).FullName).Clone(true);
        replacement.Identifier = entry.Identifier;
        replacement.SubEntries.Single(e => e.Identifier == "identifier").Value.Current = "B-1";
        encoded.SubEntries[encoded.SubEntries.IndexOf(entry)] = replacement;

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Identity, Is.EqualTo(new BatchIdentity("B-1")));
    }

    [Test]
    public void EncodeAndDecode_NoIdentity_UsesNullIdentityInstance()
    {
        // Arrange
        var dummy = new IdentityDummy();

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(IdentityEntry(encoded).Value.Current, Is.EqualTo(typeof(NullIdentity).FullName));
            Assert.That(dummy.Identity, Is.SameAs(NullIdentity.Instance));
        });
    }

    [Test]
    public void Decode_UnknownIdentityType_Throws()
    {
        // Arrange
        var dummy = new IdentityDummy { Identity = new ProductIdentity("4711", 5) };
        var encoded = EntryConvert.EncodeObject(dummy);
        IdentityEntry(encoded).Value.Current = "UnknownIdentity";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EntryConvert.UpdateInstance(dummy, encoded));
    }

    [Test]
    public void Decode_IdentityListWithItemConverter_SwitchesAndAddsIdentities()
    {
        // Arrange
        var dummy = new IdentityDummy { Identities = [new ProductIdentity("4711", 5)] };
        var encoded = EntryConvert.EncodeObject(dummy);
        var entry = IdentitiesEntry(encoded);
        var batchPrototype = entry.Prototypes.Single(p => p.Identifier == typeof(BatchIdentity).FullName);

        var switched = batchPrototype.Clone(true);
        switched.Identifier = entry.SubEntries.Single().Identifier;
        switched.SubEntries.Single(e => e.Identifier == "identifier").Value.Current = "B-1";
        entry.SubEntries[0] = switched;

        var added = batchPrototype.Instantiate();
        added.SubEntries.Single(e => e.Identifier == "identifier").Value.Current = "B-2";
        entry.SubEntries.Add(added);

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.Identities, Is.EqualTo(new IIdentity[] { new BatchIdentity("B-1"), new BatchIdentity("B-2") }));
    }

    private static Entry IdentityEntry(Entry encoded) =>
        encoded.SubEntries.Single(e => e.Identifier == nameof(IdentityDummy.Identity));

    private static Entry IdentitiesEntry(Entry encoded) =>
        encoded.SubEntries.Single(e => e.Identifier == nameof(IdentityDummy.Identities));

    public class IdentityDummy
    {
        [EntrySerialize(ConverterType = typeof(IdentityEntryConverter))]
        public IIdentity Identity { get; set; }

        [EntrySerialize(ConverterType = typeof(IdentityEntryConverter), ConverterTarget = EntryConverterTarget.Items)]
        public List<IIdentity> Identities { get; set; }
    }
}
