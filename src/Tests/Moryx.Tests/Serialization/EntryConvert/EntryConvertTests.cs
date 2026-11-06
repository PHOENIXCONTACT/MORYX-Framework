// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Globalization;
using System.Text;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests for the <see cref="EntryConvert"/> that do not fall into any of the subcategories covered
/// by the other test fixtures in this folder.
/// </summary>
[TestFixture]
public class EntryConvertTests
{
    [Test]
    public void EncodeType()
    {
        // Arrange
        var type = typeof(ClassDummy);
        var subType = typeof(SubClassDummy);

        // Act
        var encoded = EntryConvert.EncodeClass(type).SubEntries;
        var encodedSub = EntryConvert.EncodeClass(subType).SubEntries;

        // Assert
        DummyAssert(encoded, encodedSub, EntryValueType.Int32);
    }

    [Test]
    public void ShouldCreateInstanceArray_WithIdentifier_ThatStart_With_CREATED()
    {
        // Arrange
        var @object = new ClassDummy { BoolArray = [true, true, true, true, true] };
        var encoded = EntryConvert.EncodeObject(@object);

        var boolEntry = encoded.SubEntry(nameof(ClassDummy.BoolArray));
        for (var i = 0; i < 5; i++)
        {
            boolEntry.SubEntries[i].Identifier = Entry.CreatedIdentifier + i;
        }

        // Act
        var dummy = EntryConvert.CreateInstance<ClassDummy>(encoded);

        // Assert
        for (var i = 0; i < 5; i++)
        {
            Assert.That(dummy.BoolArray[i], Is.True);
        }
    }

    [Test]
    public void ShouldNotCreateInstanceArray_WithIdentifier_ThatEnd_With_CREATED()
    {
        // Arrange
        var @object = new ClassDummy { BoolArray = [true, true, true, true, true] };
        var encoded = EntryConvert.EncodeObject(@object);

        var boolEntry = encoded.SubEntry(nameof(ClassDummy.BoolArray));
        for (var i = 0; i < 5; i++)
        {
            boolEntry.SubEntries[i].Identifier = i + Entry.CreatedIdentifier;
        }

        // Act
        var dummy = EntryConvert.CreateInstance<ClassDummy>(encoded);

        // Assert
        for (var i = 0; i < 5; i++)
        {
            Assert.That(dummy.BoolArray[i], Is.Not.True);
        }
    }

    [Test]
    public void CreateInstanceWithArray()
    {
        // Arrange
        var encoded = EntryConvert.EncodeClass(typeof(ClassDummy));

        var entry1 = encoded.SubEntry(nameof(ClassDummy.BoolArray));
        var entry2 = encoded.SubEntry(nameof(ClassDummy.EnumArray));

        for (var i = 1; i <= 5; i++)
        {
            entry1.AddFromPrototype((i % 2 != 0).ToString());
        }

        for (var i = 1; i <= 5; i++)
        {
            entry2.AddFromPrototype(entry2.Prototypes[0].Value.Possible[i % 3].Key);
        }

        // Act
        var dummy = EntryConvert.CreateInstance<ClassDummy>(encoded);

        // Assert
        for (var i = 1; i <= 5; i++)
        {
            Assert.That(dummy.BoolArray[i - 1], Is.EqualTo(i % 2 != 0));
            Assert.That(dummy.EnumArray[i - 1], Is.EqualTo((EnumDummy)(i % 3)));
        }
    }

    [Test]
    public void CreateInstanceWithDictionary()
    {
        // Arrange
        var type = typeof(DictionaryClassDummy);
        var encoded = EntryConvert.EncodeClass(type);

        var entry1 = encoded.SubEntry(nameof(DictionaryClassDummy.SubDictionary));
        var entry2 = encoded.SubEntry(nameof(DictionaryClassDummy.EnumDictionary));

        for (var i = 1; i <= 5; i++)
        {
            entry1.AddFromPrototype((1 + i).ToString(CultureInfo.InvariantCulture)).DisplayName = "Key" + i;
        }

        for (var i = 1; i <= 3; i++)
        {
            entry2.AddFromPrototype(((EnumDummy)(i % 3)).ToString()).DisplayName = "Key_0121" + i;
        }

        // Act
        var dummy = EntryConvert.CreateInstance<DictionaryClassDummy>(encoded);

        // Assert
        for (var i = 1; i <= 5; i++)
        {
            Assert.That(dummy.EnumDictionary.Count, Is.EqualTo(3));
            Assert.That(dummy.SubDictionary.Count, Is.EqualTo(5));
            Assert.That(dummy.SubDictionary["Key5"], Is.EqualTo(6));
            Assert.That(dummy.EnumDictionary["Key_01213"], Is.EqualTo(EnumDummy.Unset));
        }
    }

    [Test]
    public void UpdateDictionary()
    {
        // Arrange
        var dummy = new DictionaryClassDummy
        {
            SubDictionary = new Dictionary<string, int>
            {
                {"077", 6 },
                {"088", 9 }
            },
            EnumDictionary = new Dictionary<string, EnumDummy>
            {
                {"011", EnumDummy.Unset },
                {"022", EnumDummy.ValueB }
            }
        };

        var encoded = EntryConvert.EncodeObject(dummy);

        var entry1 = encoded.SubEntry(nameof(DictionaryClassDummy.SubDictionary));
        var entry2 = encoded.SubEntry(nameof(DictionaryClassDummy.EnumDictionary));

        for (var i = 1; i <= 2; i++)
        {
            entry1.AddFromPrototype((1 + i).ToString(CultureInfo.InvariantCulture)).DisplayName = "Key" + i;
        }

        entry1.SubEntries[0].Value.Current = "123";
        entry1.SubEntries[0].DisplayName = "022";

        for (var i = 1; i <= 3; i++)
        {
            entry2.AddFromPrototype(((EnumDummy)(i % 3)).ToString()).DisplayName = "Key_0121" + i;
        }

        entry2.SubEntries[0].Value.Current = nameof(EnumDummy.ValueA);
        entry2.SubEntries[0].DisplayName = "555";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.EnumDictionary.Count, Is.EqualTo(5));
        Assert.That(dummy.SubDictionary.Count, Is.EqualTo(4));
        Assert.That(dummy.SubDictionary["022"], Is.EqualTo(123));
        Assert.That(dummy.EnumDictionary["555"], Is.EqualTo(EnumDummy.ValueA));
    }

    [Test]
    public void UpdateArray()
    {
        // Arrange
        var dummy = new ClassDummy
        {
            BoolArray = [true, false, true],
            EnumArray = [EnumDummy.Unset, EnumDummy.ValueA, EnumDummy.ValueB],
            EnumList = [EnumDummy.Unset, EnumDummy.ValueB, EnumDummy.ValueA]
        };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);
        var entry1 = encoded.SubEntry(nameof(ClassDummy.BoolArray));
        var entry2 = encoded.SubEntry(nameof(ClassDummy.EnumArray));
        var entry3 = encoded.SubEntry(nameof(ClassDummy.EnumList));

        entry1.SubEntries[1].Value.Current = "True";
        entry1.AddFromPrototype("False");

        entry2.SubEntries[2].Value.Current = "Unset";
        entry2.AddFromPrototype("ValueA");

        entry3.SubEntries[0].Value.Current = "ValueA";
        entry3.AddFromPrototype("ValueB");

        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.BoolArray.Length, Is.EqualTo(4));
        Assert.That(dummy.BoolArray[1], Is.True);
        Assert.That(dummy.BoolArray[3], Is.False);
        Assert.That(dummy.EnumArray.Length, Is.EqualTo(4));
        Assert.That(dummy.EnumArray[0], Is.EqualTo(EnumDummy.Unset));
        Assert.That(dummy.EnumArray[1], Is.EqualTo(EnumDummy.ValueA));
        Assert.That(dummy.EnumArray[2], Is.EqualTo(EnumDummy.Unset));
        Assert.That(dummy.EnumArray[3], Is.EqualTo(EnumDummy.ValueA));
        Assert.That(dummy.EnumList.Count, Is.EqualTo(4));
        Assert.That(dummy.EnumList[0], Is.EqualTo(EnumDummy.ValueA));
        Assert.That(dummy.EnumList[1], Is.EqualTo(EnumDummy.ValueB));
        Assert.That(dummy.EnumList[2], Is.EqualTo(EnumDummy.ValueA));
        Assert.That(dummy.EnumList[3], Is.EqualTo(EnumDummy.ValueB));
    }

    [Test]
    public void CreateInstanceWithPrimitiveList()
    {
        // Arrange
        var type = typeof(ListDummy);
        var encoded = EntryConvert.EncodeClass(type);
        var ent = encoded.SubEntry(nameof(ListDummy.DoubleList));

        for (var i = 1; i <= 5; i++)
        {
            ent.AddFromPrototype((1 + i).ToString(CultureInfo.InvariantCulture));
        }

        // Act
        var listDummy = EntryConvert.CreateInstance<ListDummy>(encoded);

        // Assert
        for (var i = 1; i <= 5; i++)
        {
            Assert.That(listDummy.DoubleList[i - 1], Is.EqualTo(i + 1));
        }
    }

    [Test]
    public void UpdatePrimitiveList()
    {
        // Arrange
        var defaultSerialization = new DefaultSerialization { FormatProvider = new CultureInfo("en-US") };

        var dummy = new ListDummy
        {
            Number = 0,
            DoubleList = [1.7, 2.5, 3],
            EnumList = [EnumDummy.ValueA, EnumDummy.Unset, EnumDummy.ValueB]
        };

        var encoded = EntryConvert.EncodeObject(dummy, defaultSerialization);
        var ent = encoded.SubEntry(nameof(ListDummy.DoubleList));
        var ent2 = encoded.SubEntry(nameof(ListDummy.EnumList));

        // Act
        encoded.SubEntry(nameof(ListDummy.Number)).Value.Current = "5";
        ent.SubEntries[1].Value.Current = 12.34d.ToString(defaultSerialization.FormatProvider);
        ent.AddFromPrototype(133.7d.ToString(defaultSerialization.FormatProvider));

        ent2.SubEntries[1].Value.Current = "ValueB";
        ent2.AddFromPrototype("ValueA");

        EntryConvert.UpdateInstance(dummy, encoded, defaultSerialization);

        // Assert
        Assert.That(dummy.Number, Is.EqualTo(5));
        Assert.That(dummy.DoubleList.Count, Is.EqualTo(4));
        Assert.That(dummy.DoubleList[0], Is.EqualTo(1.7));
        Assert.That(dummy.DoubleList[1], Is.EqualTo(12.34));
        Assert.That(dummy.DoubleList[2], Is.EqualTo(3.0));
        Assert.That(dummy.DoubleList[3], Is.EqualTo(133.7));

        Assert.That(dummy.EnumList.Count, Is.EqualTo(4));
        Assert.That(dummy.EnumList[0], Is.EqualTo(EnumDummy.ValueA));
        Assert.That(dummy.EnumList[1], Is.EqualTo(EnumDummy.ValueB));
        Assert.That(dummy.EnumList[2], Is.EqualTo(EnumDummy.ValueB));
        Assert.That(dummy.EnumList[3], Is.EqualTo(EnumDummy.ValueA));
    }

    [Test]
    public void EncodeObject()
    {
        // Arrange
        var dummy = new ClassDummy
        {
            Number = 10,
            Name = "Thomas",
            SingleClass = null,
            SubArray = [new SubClassDummy { Foo = (float)1.2, Enum = EnumDummy.ValueB }],
            SubList = [new SubClassDummy { Foo = (float)3.4, Enum = EnumDummy.ValueA }],
            SubEnumerable = new List<SubClassDummy> { new() { Foo = (float)3.4, Enum = EnumDummy.ValueA } },
            SubDictionary = new Dictionary<int, SubClassDummy>()
        };

        dummy.SubDictionary.Add(1, new SubClassDummy() { Enum = EnumDummy.ValueA, Foo = (float)3.4 });
        dummy.SubDictionary.Add(2, new SubClassDummy() { Enum = EnumDummy.ValueB, Foo = (float)3.5 });

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);
        var encodedSub = encoded.SubEntry(nameof(ClassDummy.SubList)).SubEntries[0].SubEntries;

        // Assert
        DummyAssert(encoded.SubEntries, encodedSub);
        Assert.That(encoded.SubEntry(nameof(ClassDummy.Number)).Value.Current, Is.EqualTo("10"), "Property value missmatch");
        Assert.That(encoded.SubEntry(nameof(ClassDummy.Name)).Value.Current, Is.EqualTo("Thomas"), "Property value missmatch");
    }

    [Test]
    public void EncodeCollection()
    {
        // Arrange
        var values = new[] { "Hello", "World" };

        // Act
        var encoded = EntryConvert.EncodeObject(values);

        // Assert
        Assert.That(encoded, Is.Not.Null);
        Assert.That(encoded.Value.Type, Is.EqualTo(EntryValueType.Collection));
        Assert.That(encoded.SubEntries.Count, Is.EqualTo(2));
        var se = encoded.SubEntries;
        Assert.That(se[0].Value.Current, Is.EqualTo(values[0]));
        Assert.That(se[1].Value.Current, Is.EqualTo(values[1]));
    }

    [Test]
    public void SurviveGetterException()
    {
        // Arrange
        var dummy = new ClassDummy();

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        Assert.That(encoded, Is.Not.Null);
        Assert.That(encoded.SubEntries[17].Value.Type, Is.EqualTo(EntryValueType.Exception));
        Assert.That(encoded.SubEntries[17].Value.Current, Is.Not.Null);
    }

    [TestCase(CollectionType.Array, 3, 2)]
    [TestCase(CollectionType.Array, 0, 4)]
    [TestCase(CollectionType.List, 5, 3)]
    [TestCase(CollectionType.List, 0, 1)]
    [TestCase(CollectionType.Enumerable, 1, 1)]
    [TestCase(CollectionType.Enumerable, 3, 0)]
    [TestCase(CollectionType.Dictionary, 1, 1)]
    [TestCase(CollectionType.Dictionary, 3, 1)]
    [TestCase(CollectionType.Dictionary, 0, 2)]
    public void AddEntry(CollectionType type, int prefill, int newValues)
    {
        // Arrange
        var defaultSerialization = new DefaultSerialization();
        var obj = Prebuild(type, prefill);
        var encoded = EntryConvert.EncodeObject(obj, defaultSerialization);

        // Act
        var colEntry = CollectionEntry(encoded, type);

        if (type == CollectionType.Dictionary)
        {
            for (var i = 1; i <= newValues; i++)
            {
                var newInstance = colEntry.AddFromPrototype();
                // change "Key" + 10
                newInstance.DisplayName = (prefill + i).ToString(CultureInfo.InvariantCulture);
                // change "Value"
                newInstance.SubEntry(nameof(SubClassDummy.Foo)).Value.Current = (prefill + i).ToString("F2", defaultSerialization.FormatProvider);
                SelectPossible(newInstance.SubEntry(nameof(SubClassDummy.Enum)), 2);
            }
        }
        else
        {
            for (var i = 1; i <= newValues; i++)
            {
                var newInstance = colEntry.AddFromPrototype();
                newInstance.SubEntry(nameof(SubClassDummy.Foo)).Value.Current = (prefill + i).ToString(CultureInfo.InvariantCulture);
                SelectPossible(newInstance.SubEntry(nameof(SubClassDummy.Enum)), 2);
            }
        }

        EntryConvert.UpdateInstance(obj, encoded);

        // Assert
        var collection = ExtractCollection(type, obj);
        var totalSize = prefill + newValues;
        Assert.That(collection.Count, Is.EqualTo(totalSize), "New size invalid");

        if (type == CollectionType.Dictionary)
        {
            var array = (collection as IEnumerable<KeyValuePair<int, SubClassDummy>>)!.ToArray();
            for (var i = 0; i < totalSize; i++)
            {
                Assert.That(array[i].Key, Is.EqualTo((float)i + 1), "Key not set!");
                Assert.That(array[i].Value.Foo, Is.EqualTo((float)i + 1), "Value not set!");
                var expectedEnum = i < prefill ? EnumDummy.ValueA : EnumDummy.ValueB;
                Assert.That(array[i].Value.Enum, Is.EqualTo(expectedEnum), "Enum not set");
            }
        }
        else
        {
            var array = (collection as IEnumerable<SubClassDummy>)!.ToArray();
            for (var i = 0; i < totalSize; i++)
            {
                Assert.That(array[i].Foo, Is.EqualTo((float)i + 1), "Value not set!");
                var expectedEnum = i < prefill ? EnumDummy.ValueA : EnumDummy.ValueB;
                Assert.That(array[i].Enum, Is.EqualTo(expectedEnum), "Enum not set");
            }
        }
    }

    [TestCase(CollectionType.Array)]
    [TestCase(CollectionType.List)]
    [TestCase(CollectionType.Enumerable)]
    [TestCase(CollectionType.Dictionary)]
    public void UpdateEntries(CollectionType type)
    {
        // Arrange
        var obj = Prebuild(type, 3);
        var encoded = EntryConvert.EncodeObject(obj);

        // Act
        var colEntry = CollectionEntry(encoded, type);

        if (type == CollectionType.Dictionary)
        {
            foreach (var entry in colEntry.SubEntries)
            {
                var foo = entry.SubEntry(nameof(SubClassDummy.Foo));
                //change "Key" + 10
                entry.DisplayName = "1" + foo.Value.Current;
                // change "Value"
                foo.Value.Current = "1" + foo.Value.Current;
                SelectPossible(entry.SubEntry(nameof(SubClassDummy.Enum)), 2);
            }

            EntryConvert.UpdateInstance(obj, encoded);

            // Assert
            var collection = ExtractCollection(type, obj);

            for (var i = 0; i < collection.Count; i++)
            {
                Assert.That(obj.SubDictionary.ContainsKey(11 + i));
                Assert.That(obj.SubDictionary[11 + i].Foo, Is.EqualTo((float)11 + i));
                Assert.That(obj.SubDictionary[11 + i].Enum, Is.EqualTo(EnumDummy.ValueB));
            }
        }
        else
        {
            foreach (var entry in colEntry.SubEntries)
            {
                var foo = entry.SubEntry(nameof(SubClassDummy.Foo));
                foo.Value.Current = "1" + foo.Value.Current;
                SelectPossible(entry.SubEntry(nameof(SubClassDummy.Enum)), 2);
            }
            EntryConvert.UpdateInstance(obj, encoded);

            // Assert
            var collection = ExtractCollection(type, obj);

            var array = (collection as IEnumerable<SubClassDummy>)!.ToArray();

            for (var i = 0; i < collection.Count; i++)
            {
                Assert.That(array[i].Foo, Is.EqualTo((float)11 + i));
                Assert.That(array[i].Enum, Is.EqualTo(EnumDummy.ValueB));
            }
        }
    }

    [TestCase(CollectionType.Array, 3, new[] { 1 })]
    [TestCase(CollectionType.Array, 4, new[] { 0, 2 })]
    [TestCase(CollectionType.List, 5, new[] { 1, 2 })]
    [TestCase(CollectionType.List, 10, new[] { 1, 2, 5, 6, 7 })]
    [TestCase(CollectionType.Enumerable, 4, new[] { 2 })]
    [TestCase(CollectionType.Enumerable, 1, new[] { 0 })]
    [TestCase(CollectionType.Dictionary, 1, new[] { 0 })]
    [TestCase(CollectionType.Dictionary, 3, new[] { 1 })]
    [TestCase(CollectionType.Dictionary, 10, new[] { 0, 9 })]
    public void RemoveEntry(CollectionType type, int prefill, int[] removedIndexes)
    {
        // Arrange
        var obj = Prebuild(type, prefill);
        var encoded = EntryConvert.EncodeObject(obj);

        // Act
        var colEntry = CollectionEntry(encoded, type);
        if (type == CollectionType.Dictionary)
        {
            colEntry.SubEntries.RemoveAll(e => removedIndexes.Contains(int.Parse(e.Identifier, CultureInfo.InvariantCulture) - 1));
        }
        else
        {
            colEntry.SubEntries.RemoveAll(e => removedIndexes.Contains(int.Parse(e.Identifier, CultureInfo.InvariantCulture)));
        }

        EntryConvert.UpdateInstance(obj, encoded);

        // Assert
        var collection = ExtractCollection(type, obj);
        var totalSize = prefill - removedIndexes.Length;
        Assert.That(collection.Count, Is.EqualTo(totalSize), "New size invalid");

        if (type == CollectionType.Dictionary)
        {
            var array = (collection as IEnumerable<KeyValuePair<int, SubClassDummy>>)!.ToArray();
            for (var i = 0; i < prefill; i++)
            {
                if (removedIndexes.Contains(i))
                {
                    continue;
                }

                var match = array.FirstOrDefault(e => e.Key == i + 1);
                Assert.That(match, Is.Not.Null);
            }
        }
        else
        {
            var array = (collection as IEnumerable<SubClassDummy>)!.ToArray();
            for (var i = 0; i < prefill; i++)
            {
                if (removedIndexes.Contains(i))
                {
                    continue;
                }

                var match = array.FirstOrDefault(e => e.Foo == i + 1);
                Assert.That(match, Is.Not.Null);
            }
        }
    }

    [TestCase(CollectionType.Array, 4)]
    [TestCase(CollectionType.List, 4)]
    [TestCase(CollectionType.Enumerable, 4)]
    public void ReorderEntries(CollectionType type, int prefill)
    {
        // Arrange
        var obj = Prebuild(type, prefill);
        var encoded = EntryConvert.EncodeObject(obj);
        var colEntry = CollectionEntry(encoded, type);

        // Act - reverse the sub-entries order
        colEntry.SubEntries.Reverse();
        EntryConvert.UpdateInstance(obj, encoded);

        // Assert
        var collection = ExtractCollection(type, obj);
        Assert.That(collection.Count, Is.EqualTo(prefill), "Size should remain the same");

        var array = (collection as IEnumerable<SubClassDummy>)!.ToArray();
        for (var i = 0; i < prefill; i++)
        {
            // Original order was Foo = 1,2,3,4 — reversed should be 4,3,2,1
            Assert.That(array[i].Foo, Is.EqualTo(prefill - i), "Item not in expected reordered position");
        }
    }

    [TestCase(CollectionType.Array)]
    [TestCase(CollectionType.List)]
    [TestCase(CollectionType.Enumerable)]
    public void ReorderWithAddAndRemove(CollectionType type)
    {
        // Arrange - create collection with items Foo=1,2,3,4,5
        var obj = Prebuild(type, 5);
        var encoded = EntryConvert.EncodeObject(obj);
        var colEntry = CollectionEntry(encoded, type);

        // Remove index 1 (Foo=2) and index 3 (Foo=4)
        colEntry.SubEntries.RemoveAt(3);
        colEntry.SubEntries.RemoveAt(1);

        // Add a new entry (Foo=99)
        var newInstance = colEntry.Prototypes[0].Instantiate();
        newInstance.SubEntry(nameof(SubClassDummy.Foo)).Value.Current = "99";
        SelectPossible(newInstance.SubEntry(nameof(SubClassDummy.Enum)), 2);

        // Act - insert the new entry at position 1 (between Foo=1 and Foo=3)
        colEntry.SubEntries.Insert(1, newInstance);
        EntryConvert.UpdateInstance(obj, encoded);

        // Assert - expected order: Foo=1, Foo=99, Foo=3, Foo=5
        var collection = ExtractCollection(type, obj);
        Assert.That(collection.Count, Is.EqualTo(4));

        var array = (collection as IEnumerable<SubClassDummy>)!.ToArray();
        Assert.That(array[0].Foo, Is.EqualTo(1));
        Assert.That(array[1].Foo, Is.EqualTo(99));
        Assert.That(array[2].Foo, Is.EqualTo(3));
        Assert.That(array[3].Foo, Is.EqualTo(5));
    }

    [Test]
    public void CreateInstance()
    {
        // Arrange
        var defaultSerialization = new DefaultSerialization();
        var encoded = EntryConvert.EncodeClass(typeof(ClassDummy), defaultSerialization);

        // Act
        var collectionTypes = new[] { CollectionType.Array, CollectionType.List, CollectionType.Enumerable };
        encoded.SubEntry(nameof(ClassDummy.Number)).Value.Current = "10";
        encoded.SubEntry(nameof(ClassDummy.Name)).Value.Current = "Thomas";
        SelectPossible(encoded.SubEntry(nameof(ClassDummy.SingleClass)).SubEntry(nameof(SubClassDummy.Enum)), 2);
        for (var i = 0; i < collectionTypes.Length; i++)
        {
            var colEntry = CollectionEntry(encoded, collectionTypes[i]);
            for (var j = 0; j < 4 + i; j++)
            {
                var newInstance = colEntry.AddFromPrototype();
                newInstance.SubEntry(nameof(SubClassDummy.Foo)).Value.Current = j.ToString("F2", defaultSerialization.FormatProvider);
                SelectPossible(newInstance.SubEntry(nameof(SubClassDummy.Enum)), 1);
            }
        }
        var obj = EntryConvert.CreateInstance<ClassDummy>(encoded);

        // Assert
        Assert.That(obj.Number, Is.EqualTo(10));
        Assert.That(obj.Name, Is.EqualTo("Thomas"));
        Assert.That(obj.SingleClass.Enum, Is.EqualTo(EnumDummy.ValueB));
        for (var i = 0; i < collectionTypes.Length; i++)
        {
            var length = 4 + i;
            var collection = ExtractCollection(collectionTypes[i], obj);
            Assert.That(collection.Count, Is.EqualTo(length));

            var array = (collection as IEnumerable<SubClassDummy>)!.ToArray();
            for (var j = 0; j < length; j++)
            {
                Assert.That(array[j].Foo, Is.EqualTo((float)j));
                Assert.That(array[j].Enum, Is.EqualTo(EnumDummy.ValueA));
            }
        }
    }

    [Test(Description = "Encodes a MemoryStream")]
    public void MemoryStreamEncode()
    {
        // Arrange
        var testString = "This is a test";
        var streamDummy = new MemoryStreamClassDummy(testString);

        // Act
        var entry = EntryConvert.EncodeObject(streamDummy);

        // Assert
        Assert.That(entry.SubEntries.Count, Is.EqualTo(1));
        var streamEntry = entry.SubEntry(nameof(MemoryStreamClassDummy.MemoryStream));
        Assert.That(streamEntry.Value.Type, Is.EqualTo(EntryValueType.Stream));
        Assert.That(streamEntry.Value.Current, Is.EqualTo("VGhpcyBpcyBhIHRlc3Q="));
    }

    [Test(Description = "Decodes to a MemoryStream and creates a new stream")]
    public void MemoryStreamDecode()
    {
        // Arrange
        var testString = "This is a test";
        var streamDummy = new MemoryStreamClassDummy(testString);
        var targetStreamDummy = new MemoryStreamClassDummy("");
        var streamInstanceToCheck = targetStreamDummy.MemoryStream;
        var entry = EntryConvert.EncodeObject(streamDummy);

        // Act
        EntryConvert.UpdateInstance(targetStreamDummy, entry);

        // Assert
        var stringValue = Encoding.UTF8.GetString(targetStreamDummy.MemoryStream.ToArray());

        Assert.That(stringValue, Is.EqualTo(testString));
        Assert.That(streamInstanceToCheck, Is.Not.SameAs(targetStreamDummy.MemoryStream));
    }

    [Test(Description = "Decodes to a MemoryStream and reuses the stream")]
    public void MemoryStreamDecodeReuseCurrentStream()
    {
        // Arrange
        var testString = "This is a test";
        var dummyString = "12345678912345";
        var streamDummy = new MemoryStreamClassDummy(testString);
        var targetStreamDummy = new MemoryStreamClassDummy(dummyString);
        var streamInstanceToCheck = targetStreamDummy.MemoryStream;
        var entry = EntryConvert.EncodeObject(streamDummy);

        // Act
        EntryConvert.UpdateInstance(targetStreamDummy, entry);

        // Assert
        var stringValue = Encoding.UTF8.GetString(targetStreamDummy.MemoryStream.ToArray());

        Assert.That(stringValue, Is.EqualTo(testString));
        Assert.That(streamInstanceToCheck, Is.SameAs(targetStreamDummy.MemoryStream));
    }

    [Test(Description = "Decodes to a MemoryStream and reuses the stream but the initial stream size is greater than the new applied value")]
    public void MemoryStreamDecodeReuseCurrentStreamInitSizeIsGreaterThanNewData()
    {
        // Arrange
        var testString = "A test";
        var dummyString = "12345678912345";
        var streamDummy = new MemoryStreamClassDummy(testString);
        var targetStreamDummy = new MemoryStreamClassDummy(dummyString);
        var streamInstanceToCheck = targetStreamDummy.MemoryStream;
        var entry = EntryConvert.EncodeObject(streamDummy);

        // Act
        EntryConvert.UpdateInstance(targetStreamDummy, entry);

        // Assert
        var stringValue = Encoding.UTF8.GetString(targetStreamDummy.MemoryStream.ToArray());

        Assert.That(stringValue, Is.EqualTo(testString));
        Assert.That(streamInstanceToCheck, Is.SameAs(targetStreamDummy.MemoryStream));
    }

    [Test(Description = "Encodes a FileStream")]
    public void FileStreamEncode()
    {
        // Arrange
        var testString = "This is a test";
        var testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "text.txt");
        var streamDummy = new FileStreamClassDummy(testFilePath, FileMode.Create);
        var testBytes = Encoding.UTF8.GetBytes(testString);
        streamDummy.FileStream.Write(testBytes, 0, testBytes.Length);

        // Act
        var entry = EntryConvert.EncodeObject(streamDummy);

        // Assert
        Assert.That(entry.SubEntries.Count, Is.EqualTo(1));
        var streamEntry = entry.SubEntry(nameof(FileStreamClassDummy.FileStream));
        Assert.That(streamEntry.Value.Type, Is.EqualTo(EntryValueType.Stream));
        Assert.That(streamEntry.Value.Current, Is.EqualTo("VGhpcyBpcyBhIHRlc3Q="));

        streamDummy.FileStream.Close();
        File.Delete(testFilePath);
    }

    [Test(Description = "Decodes to a FileStream and reuses the stream")]
    public void FileStreamDecodeReuseCurrentStream()
    {
        // Arrange
        var testString = "This is a test";
        var testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "text1.txt");
        var testFilePath2 = Path.Combine(TestContext.CurrentContext.TestDirectory, "text2.txt");
        var streamDummy = new FileStreamClassDummy(testFilePath, FileMode.Create);
        var targetStreamDummy = new FileStreamClassDummy(testFilePath2, FileMode.Create);
        var targetStreamDummyInitialData = Encoding.UTF8.GetBytes("12345678901234567890");
        targetStreamDummy.FileStream.Write(targetStreamDummyInitialData, 0, targetStreamDummyInitialData.Length);

        var streamInstanceToCheck = targetStreamDummy.FileStream;
        var testBytes = Encoding.UTF8.GetBytes(testString);
        streamDummy.FileStream.Write(testBytes, 0, testBytes.Length);

        var entry = EntryConvert.EncodeObject(streamDummy);

        // Act
        EntryConvert.UpdateInstance(targetStreamDummy, entry);

        // Assert
        var buffer = new byte[targetStreamDummy.FileStream.Length];
        targetStreamDummy.FileStream.Seek(0, SeekOrigin.Begin);
        targetStreamDummy.FileStream.ReadExactly(buffer, 0, buffer.Length);

        var stringValue = Encoding.UTF8.GetString(buffer);

        Assert.That(stringValue, Is.EqualTo(testString));
        Assert.That(streamInstanceToCheck, Is.SameAs(targetStreamDummy.FileStream));

        streamDummy.FileStream.Close();
        File.Delete(testFilePath);
        targetStreamDummy.FileStream.Close();
        File.Delete(testFilePath2);
    }

    [TestCase("en-US", Description = "Testing format parsing and writing with en-US")]
    [TestCase("de-DE", Description = "Testing format parsing and writing with de-DE")]
    [TestCase("fr-FR", Description = "Testing format parsing and writing with fr-FR")]
    [TestCase("ru-RU", Description = "Testing format parsing and writing with ru-RU")]
    [TestCase("zh-CN", Description = "Testing format parsing and writing with zh-CN")]
    [TestCase("he-IL", Description = "Testing format parsing and writing with he-IL")]
    [TestCase("ar-EG", Description = "Testing format parsing and writing with ar-EG")]
    public void FormatProviderTest(string cultureName)
    {
        // Arrange
        var formatProvider = new CultureInfo(cultureName);
        var serialization = new DefaultSerialization { FormatProvider = formatProvider };

        var dummy = new ClassDummy
        {
            Number = 1001,
            SingleClass = new SubClassDummy { Foo = 1.1234f }
        };

        // Act
        var encoded = EntryConvert.EncodeObject(dummy, serialization);
        var dummyDecoded = EntryConvert.CreateInstance<ClassDummy>(encoded, serialization);

        // Assert
        Assert.That(encoded.SubEntry(nameof(ClassDummy.Number)).Value.Current, Is.EqualTo(1001.ToString(formatProvider)));
        Assert.That(encoded.SubEntry(nameof(ClassDummy.SingleClass)).SubEntry(nameof(SubClassDummy.Foo)).Value.Current,
            Is.EqualTo(1.1234f.ToString(formatProvider)));

        Assert.That(dummyDecoded.Number, Is.EqualTo(1001));
        Assert.That(dummyDecoded.SingleClass.Foo, Is.EqualTo(1.1234f));
    }

    [Test]
    public void ShouldEncodeClass_Properties()
    {
        // Act
        var entry = EntryConvert.EncodeClass(typeof(ClassDummy));

        // Assert
        Assert.That(entry.SubEntries.Select(e => e.Identifier), Does.Contain(nameof(ClassDummy.Number)));
    }

    [Test]
    public void ShouldEncodeObject_PropertyWithValue()
    {
        // Arrange
        var myObject = new ClassDummy();
        myObject.Number = 10;

        // Act
        var entry = EntryConvert.EncodeObject(myObject);

        // Assert
        Assert.That(entry.SubEntry(nameof(ClassDummy.Number)).Value.Current,
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
    public void UpdateInstance_WithGetOnlyProperty_UpdatesInPlace()
    {
        // Arrange
        var dummy = new ClassDummy();
        var encoded = EntryConvert.EncodeObject(dummy);
        encoded.SubEntry(nameof(ClassDummy.GetterOnly)).SubEntry(nameof(SubClassDummy.Foo)).Value.Current = "42";

        // Act
        EntryConvert.UpdateInstance(dummy, encoded);

        // Assert
        Assert.That(dummy.GetterOnly.Foo, Is.EqualTo(42f));
    }

    public enum CollectionType
    {
        List,
        Array,
        Enumerable,
        Dictionary
    }

    private static ClassDummy Prebuild(CollectionType type, int values)
    {
        var obj = new ClassDummy();
        var elements = new List<SubClassDummy>();
        var dictionary = new Dictionary<int, SubClassDummy>();
        for (var i = 1; i <= values; i++)
        {
            elements.Add(new SubClassDummy { Foo = i, Enum = EnumDummy.ValueA });
            dictionary.Add(i, new SubClassDummy { Foo = i, Enum = EnumDummy.ValueA });
        }
        switch (type)
        {
            case CollectionType.List:
                obj.SubList = elements;
                break;
            case CollectionType.Array:
                obj.SubArray = elements.ToArray();
                break;
            case CollectionType.Enumerable:
                obj.SubEnumerable = elements;
                break;
            case CollectionType.Dictionary:
                obj.SubDictionary = dictionary;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
        return obj;
    }

    private static Entry CollectionEntry(Entry encoded, CollectionType type) =>
        encoded.SubEntry($"Sub{type:G}");

    private static IList ExtractCollection(CollectionType type, ClassDummy obj)
    {
        IList collection;
        switch (type)
        {
            case CollectionType.List:
                collection = obj.SubList;
                break;
            case CollectionType.Array:
                collection = obj.SubArray;
                break;
            case CollectionType.Enumerable:
                collection = obj.SubEnumerable.ToList();
                break;
            case CollectionType.Dictionary:
                collection = obj.SubDictionary.ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
        return collection;
    }

    private static void SelectPossible(Entry entry, int index) =>
        entry.Value.Current = entry.Value.Possible[index].Key;

    private static void DummyAssert(IList<Entry> encoded, IList<Entry> encodedSub,
        EntryValueType throwsExceptionType = EntryValueType.Exception)
    {
        // Assert
        var expected = new[]
        {
            new { Name = "Number", Type = EntryValueType.Int32, ReadOnly = false },
            new { Name = "Name", Type = EntryValueType.String, ReadOnly = false },
            new { Name = "ModifiedAt", Type = EntryValueType.DateTime, ReadOnly = false },
            new { Name = "CreatedAt", Type = EntryValueType.DateTime, ReadOnly = false },
            new { Name = "SingleClass", Type = EntryValueType.Class, ReadOnly = false },
            new { Name = "SubArray", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "SubList", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "SubEnumerable", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "SubDictionary", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "EnumArray", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "EnumList", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "EnumEnumerable", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "BoolArray", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "BoolList", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "BoolEnumerable", Type = EntryValueType.Collection, ReadOnly = false },
            new { Name = "GetterOnly", Type = EntryValueType.Class, ReadOnly = true },
            new { Name = "NullablePrimitive", Type = EntryValueType.String, ReadOnly = false },
            // Without an instance the getter is never invoked, so the exception cannot be detected
            new { Name = "ThrowsException", Type = throwsExceptionType, ReadOnly = true }
        };
        Assert.That(encoded.Count, Is.EqualTo(expected.Length), "Number of entries does not match");
        for (var i = 0; i < encoded.Count; i++)
        {
            Assert.That(encoded[i].Identifier, Is.EqualTo(expected[i].Name), "Property name mismatch");
            Assert.That(encoded[i].Value.Type, Is.EqualTo(expected[i].Type), "Type mismatch");
            Assert.That(encoded[i].Value.IsReadOnly, Is.EqualTo(expected[i].ReadOnly), "ReadOnly mismatch");
        }
        Assert.That(encodedSub[0].Identifier, Is.EqualTo("Foo"), "Name mismatch");
        Assert.That(encodedSub[0].Value.Type, Is.EqualTo(EntryValueType.Single), "Float not detected");
        Assert.That(encodedSub[1].Identifier, Is.EqualTo("Enum"));
        Assert.That(encodedSub[1].Value.Type, Is.EqualTo(EntryValueType.Enum), "Enum not detected");
        Assert.That(encodedSub[1].Value.Default, Is.EqualTo("Unset"));
        Assert.That(encodedSub[1].Value.Possible.Length, Is.EqualTo(3), "Possible values not set");
    }
}
