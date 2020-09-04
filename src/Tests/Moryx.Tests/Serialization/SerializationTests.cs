// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests
{
    /// <summary>
    /// Unit tests for the static <see cref="EntryConvert"/> class
    /// </summary>
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void EncodeType()
        {
            // Arrange
            var type = typeof(DummyClass);
            var subType = typeof(SubClass);

            // Act
            var encoded = EntryConvert.EncodeClass(type).SubEntries;
            var encodedSub = EntryConvert.EncodeClass(subType).SubEntries;

            // Assert
            DummyAssert(encoded, encodedSub);
        }

        [Test]
        public void CreateInstanceWithArray()
        {
            // Arrange
            var type = typeof(ArrayDummy);
            var encoded = EntryConvert.EncodeClass(type);

            var entry1 = encoded.SubEntries[0];
            var entry2 = encoded.SubEntries[1];

            for (var i = 1; i <= 5; i++)
            {
                var newInstance = entry1.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                entry1.SubEntries.Add(newInstance);
            }

            for (var i = 1; i <= 5; i++)
            {
                var newInstance = entry2.Prototypes[0].Instantiate();
                newInstance.Value.Current = "Number: " + i;
                entry2.SubEntries.Add(newInstance);
            }

            // Act
            var dummy = EntryConvert.CreateInstance<ArrayDummy>(encoded);

            // Assert
            for (var i = 1; i <= 5; i++)
            {
                Assert.AreEqual(i + 1, dummy.Array[i - 1]);
                Assert.AreEqual("Number: " + i, dummy.Keys[i - 1]);
            }
        }

        [Test]
        public void CreateInstanceWithDictionary()
        {
            // Arrange
            var type = typeof(DictionaryClass);
            var encoded = EntryConvert.EncodeClass(type);

            var entry1 = encoded.SubEntries[1];
            var entry2 = encoded.SubEntries[2];

            for (var i = 1; i <= 5; i++)
            {
                var newInstance = entry1.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                newInstance.DisplayName = "Key" + i;
                entry1.SubEntries.Add(newInstance);
            }

            for (var i = 1; i <= 3; i++)
            {
                var newInstance = entry2.Prototypes[0].Instantiate();
                newInstance.Value.Current = ((DummyEnum)(i % 3)).ToString();
                newInstance.DisplayName = "Key_0121" + i;
                entry2.SubEntries.Add(newInstance);
            }

            // Act
            var dummy = EntryConvert.CreateInstance<DictionaryClass>(encoded);

            // Assert
            for (var i = 1; i <= 5; i++)
            {
                Assert.AreEqual(3, dummy.EnumDictionary.Count );
                Assert.AreEqual(5 , dummy.SubDictionary.Count);
                Assert.AreEqual(6, dummy.SubDictionary["Key5"]);
                Assert.AreEqual(DummyEnum.Unset, dummy.EnumDictionary["Key_01213"]);
            }
        }

        [Test]
        public void UpdateDictionary()
        {
            // Arrange
            var dummy = new DictionaryClass
            {
                SubDictionary = new Dictionary<string, int>
                {
                    {"077", 6 },
                    {"088", 9 }
                },
                EnumDictionary = new Dictionary<string, DummyEnum>
                {
                    {"011", DummyEnum.Unset },
                    {"022", DummyEnum.ValueB }
                }
            };

            var encoded = EntryConvert.EncodeObject(dummy);

            var entry1 = encoded.SubEntries[1];
            var entry2 = encoded.SubEntries[2];


            for (var i = 1; i <= 2; i++)
            {
                var newInstance = entry1.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                newInstance.DisplayName = "Key" + i;
                entry1.SubEntries.Add(newInstance);
            }

            entry1.SubEntries[0].Value.Current = "123";
            entry1.SubEntries[0].DisplayName = "022";

            for (var i = 1; i <= 3; i++)
            {
                var newInstance = entry2.Prototypes[0].Instantiate();
                newInstance.Value.Current = ((DummyEnum)(i % 3)).ToString();
                newInstance.DisplayName = "Key_0121" + i;
                entry2.SubEntries.Add(newInstance);
            }

            entry2.SubEntries[0].Value.Current = DummyEnum.ValueA.ToString();
            entry2.SubEntries[0].DisplayName = "555";

            // Act
            EntryConvert.UpdateInstance(dummy, encoded);

            // Assert
            Assert.AreEqual(5, dummy.EnumDictionary.Count);
            Assert.AreEqual(4, dummy.SubDictionary.Count);
            Assert.AreEqual(123, dummy.SubDictionary["022"]);
            Assert.AreEqual(DummyEnum.ValueA, dummy.EnumDictionary["555"]);
        }

        [Test]
        public void UpdateArray()
        {
            // Arrange
            var dummy = new ArrayDummy
            {
                Array = new[] { 2, 5, 7 },
                Keys = new[] { "test1_2", "test_02", "1245" },
                Enums = new[] { DummyEnum.Unset, DummyEnum.ValueB, DummyEnum.ValueA }
            };

            // Act
            var encoded = EntryConvert.EncodeObject(dummy);
            var entry1 = encoded.SubEntries[0];
            var entry2 = encoded.SubEntries[1];
            var entry3 = encoded.SubEntries[2];

            entry1.SubEntries[1].Value.Current = "42";
            var instance1 = encoded.SubEntries[0].Prototypes[0].Instantiate();
            instance1.Value.Current = "1337";
            entry1.SubEntries.Add(instance1);

            entry2.SubEntries[2].Value.Current = "hallo";
            var instance2 = entry2.Prototypes[0].Instantiate();
            instance2.Value.Current = "new_Value";
            entry2.SubEntries.Add(instance2);

            entry3.SubEntries[0].Value.Current = "ValueA";
            var instance3 = entry3.Prototypes[0].Instantiate();
            instance3.Value.Current = "ValueB";
            entry3.SubEntries.Add(instance3);

            EntryConvert.UpdateInstance(dummy, encoded);

            // Assert
            Assert.AreEqual(4, dummy.Array.Length);
            Assert.AreEqual(42, dummy.Array[1]);
            Assert.AreEqual(1337, dummy.Array[3]);
            Assert.AreEqual(4, dummy.Keys.Length);
            Assert.AreEqual("test1_2", dummy.Keys[0]);
            Assert.AreEqual("test_02", dummy.Keys[1]);
            Assert.AreEqual("hallo", dummy.Keys[2]);
            Assert.AreEqual("new_Value", dummy.Keys[3]);
            Assert.AreEqual(DummyEnum.ValueA, dummy.Enums[0]);
            Assert.AreEqual(DummyEnum.ValueB, dummy.Enums[1]);
            Assert.AreEqual(DummyEnum.ValueA, dummy.Enums[2]);
            Assert.AreEqual(DummyEnum.ValueB, dummy.Enums[3]);
        }

        [Test]
        public void CreateInstanceWithPrimitiveList()
        {
            // Arrange
            var type = typeof(ListDummy);
            var encoded = EntryConvert.EncodeClass(type);
            var ent = encoded.SubEntries[1];

            for (var i = 1; i <= 5; i++)
            {
                var newInstance = ent.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                ent.SubEntries.Add(newInstance);
            }

            // Act
            var listDummy = EntryConvert.CreateInstance<ListDummy>(encoded);

            // Assert
            for (var i = 1; i <= 5; i++)
            {
                Assert.AreEqual(i + 1, listDummy.DoubleList[i - 1]);
            }
        }

        [Test]
        public void UpdatePrimitiveList()
        {
            // Arrange
            var defaultSerialization = new DefaultSerialization { FormatProvider = new CultureInfo("en-US")};

            var dummy = new ListDummy
            {
                Number = 0,
                DoubleList = new List<double> {1.7, 2.5, 3},
                EnumList = new List<DummyEnum> { DummyEnum.ValueA, DummyEnum.Unset, DummyEnum.ValueB}
            };

            var encoded = EntryConvert.EncodeObject(dummy, defaultSerialization);
            var ent = encoded.SubEntries[1];
            var ent2 = encoded.SubEntries[2];

            // Act
            encoded.SubEntries[0].Value.Current = "5";
            ent.SubEntries[1].Value.Current = 12.34d.ToString(defaultSerialization.FormatProvider);
            var newInstance = ent.Prototypes[0].Instantiate();
            newInstance.Value.Current = 133.7d.ToString(defaultSerialization.FormatProvider);
            ent.SubEntries.Add(newInstance);

            ent2.SubEntries[1].Value.Current = "ValueB";
            newInstance = ent2.Prototypes[0].Instantiate();
            newInstance.Value.Current = "ValueA";
            ent2.SubEntries.Add(newInstance);

            EntryConvert.UpdateInstance(dummy, encoded, defaultSerialization);

            // Assert
            Assert.AreEqual(5, dummy.Number);
            Assert.AreEqual(4, dummy.DoubleList.Count);
            Assert.AreEqual(1.7, dummy.DoubleList[0]);
            Assert.AreEqual(12.34, dummy.DoubleList[1]);
            Assert.AreEqual(3.0, dummy.DoubleList[2]);
            Assert.AreEqual(133.7, dummy.DoubleList[3]);

            Assert.AreEqual(4, dummy.EnumList.Count);
            Assert.AreEqual(DummyEnum.ValueA, dummy.EnumList[0]);
            Assert.AreEqual(DummyEnum.ValueB, dummy.EnumList[1]);
            Assert.AreEqual(DummyEnum.ValueB, dummy.EnumList[2]);
            Assert.AreEqual(DummyEnum.ValueA, dummy.EnumList[3]);
        }

        [Test]
        public void EncodeObject()
        {
            // Arrange
            var dummy = new DummyClass
            {
                Number = 10,
                Name = "Thomas",
                SingleClass = null,
                SubArray = new[] { new SubClass { Foo = (float)1.2, Enum = DummyEnum.ValueB } },
                SubList = new List<SubClass> { new SubClass { Foo = (float)3.4, Enum = DummyEnum.ValueA } },
                SubEnumerable = new List<SubClass> { new SubClass { Foo = (float)3.4, Enum = DummyEnum.ValueA } },
                SubDictionary = new Dictionary<int, SubClass>()
            };

            dummy.SubDictionary.Add(1, new SubClass() { Enum = DummyEnum.ValueA, Foo = (float)3.4 });
            dummy.SubDictionary.Add(2, new SubClass() { Enum = DummyEnum.ValueB, Foo = (float)3.5 });

            // Act
            var encoded = EntryConvert.EncodeObject(dummy).SubEntries;
            var encodedSub = encoded[4].SubEntries[0].SubEntries;

            // Assert
            DummyAssert(encoded, encodedSub);
            var expected = new[] { "10", "Thomas", "10" };
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(expected[i], encoded[i].Value.Current, "Property value missmatch");
            }
        }

        [Test]
        public void EncodeCollection()
        {
            // Arrange
            var values = new string[]{"Hello", "World"};

            // Act
            var encoded = EntryConvert.EncodeObject(values);

            // Assert
            Assert.NotNull(encoded);
            Assert.AreEqual(EntryValueType.Collection, encoded.Value.Type);
            Assert.AreEqual(2, encoded.SubEntries.Count);
            var se = encoded.SubEntries;
            Assert.AreEqual(values[0], se[0].Value.Current);
            Assert.AreEqual(values[1], se[1].Value.Current);
        }

        [Test]
        public void SurviveGetterException()
        {
            // Arrange
            var dummy = new ExceptionDummy();

            // Act
            var encoded = EntryConvert.EncodeObject(dummy);

            // Assert
            Assert.NotNull(encoded);
            Assert.AreEqual(EntryValueType.Exception, encoded.SubEntries[0].Value.Type);
            Assert.NotNull(encoded.SubEntries[0].Value.Current);
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
            var colEntry = CollectionEntry(encoded.SubEntries, type);

            if (type == CollectionType.Dictionary)
            {
                for (var i = 1; i <= newValues; i++)
                {
                    var newInstance = colEntry.Prototypes[0].Instantiate();
                    // change "Key" + 10
                    newInstance.DisplayName = (prefill + i).ToString();
                    // change "Value"
                    newInstance.SubEntries[0].Value.Current = (prefill + i).ToString("F2", defaultSerialization.FormatProvider);
                    newInstance.SubEntries[1].Value.Current = newInstance.SubEntries[1].Value.Possible[2];

                    colEntry.SubEntries.Add(newInstance);
                }
            }
            else
            {
                for (var i = 1; i <= newValues; i++)
                {
                    var newInstance = colEntry.Prototypes[0].Instantiate();
                    newInstance.SubEntries[0].Value.Current = (prefill + i).ToString();
                    newInstance.SubEntries[1].Value.Current = newInstance.SubEntries[1].Value.Possible[2];
                    colEntry.SubEntries.Add(newInstance);
                }
            }

            EntryConvert.UpdateInstance(obj, encoded);

            // Assert
            var collection = ExtractCollection(type, obj);
            var totalSize = prefill + newValues;
            Assert.AreEqual(totalSize, collection.Count, "New size invalid");

            if (type == CollectionType.Dictionary)
            {
                var array = (collection as IEnumerable<KeyValuePair<int,SubClass>>).ToArray();
                for (var i = 0; i < totalSize; i++)
                {
                    Assert.AreEqual((float)i + 1, array[i].Key, "Key not set!");
                    Assert.AreEqual((float)i + 1, array[i].Value.Foo, "Value not set!");
                    var expectedEnum = i < prefill ? DummyEnum.ValueA : DummyEnum.ValueB;
                    Assert.AreEqual(expectedEnum, array[i].Value.Enum, "Enum not set");
                }
            }
            else
            {
                var array = (collection as IEnumerable<SubClass>).ToArray();
                for (var i = 0; i < totalSize; i++)
                {
                    Assert.AreEqual((float)i + 1, array[i].Foo, "Value not set!");
                    var expectedEnum = i < prefill ? DummyEnum.ValueA : DummyEnum.ValueB;
                    Assert.AreEqual(expectedEnum, array[i].Enum, "Enum not set");
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
            var colEntry = CollectionEntry(encoded.SubEntries, type);

            if (type == CollectionType.Dictionary)
            {
                foreach (var entry in colEntry.SubEntries)
                {
                    //change "Key" + 10
                    entry.DisplayName = "1" + entry.SubEntries[0].Value.Current;
                    // change "Value"
                    entry.SubEntries[0].Value.Current = "1" + entry.SubEntries[0].Value.Current;
                    entry.SubEntries[1].Value.Current = entry.SubEntries[1].Value.Possible[2];
                }

                EntryConvert.UpdateInstance(obj, encoded);

                // Assert
                var collection = ExtractCollection(type, obj);

                for (var i = 0; i < collection.Count; i++)
                {
                    Assert.IsTrue(obj.SubDictionary.ContainsKey(11 + i));
                    Assert.AreEqual((float)11 + i, obj.SubDictionary[11 + i].Foo);
                    Assert.AreEqual(DummyEnum.ValueB, obj.SubDictionary[11 + i].Enum);
                }
            }
            else
            {
                foreach (var entry in colEntry.SubEntries)
                {
                    entry.SubEntries[0].Value.Current = "1" + entry.SubEntries[0].Value.Current;
                    entry.SubEntries[1].Value.Current = entry.SubEntries[1].Value.Possible[2];
                }
                EntryConvert.UpdateInstance(obj, encoded);

                // Assert
                var collection = ExtractCollection(type, obj);

                var array = (collection as IEnumerable<SubClass>).ToArray();

                for (var i = 0; i < collection.Count; i++)
                {
                    Assert.AreEqual((float)11 + i, array[i].Foo);
                    Assert.AreEqual(DummyEnum.ValueB, array[i].Enum);
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
            var colEntry = CollectionEntry(encoded.SubEntries, type);
            if(type == CollectionType.Dictionary)
                colEntry.SubEntries.RemoveAll(e => removedIndexes.Contains(int.Parse(e.Identifier) - 1));
            else
                colEntry.SubEntries.RemoveAll(e => removedIndexes.Contains(int.Parse(e.Identifier)));
            EntryConvert.UpdateInstance(obj, encoded);

            // Assert
            var collection = ExtractCollection(type, obj);
            var totalSize = prefill - removedIndexes.Length;
            Assert.AreEqual(totalSize, collection.Count, "New size invalid");


            if (type == CollectionType.Dictionary)
            {
                var array = (collection as IEnumerable<KeyValuePair<int, SubClass>>).ToArray();
                for (var i = 0; i < prefill; i++)
                {
                    if (removedIndexes.Contains(i))
                        continue;

                    var match = array.FirstOrDefault(e => e.Key == i + 1);
                    Assert.NotNull(match);
                }
            }
            else
            {
                var array = (collection as IEnumerable<SubClass>).ToArray();
                for (var i = 0; i < prefill; i++)
                {
                    if (removedIndexes.Contains(i))
                        continue;

                    var match = array.FirstOrDefault(e => e.Foo == i + 1);
                    Assert.NotNull(match);
                }
            }
        }

        [Test]
        public void CreateInstance()
        {
            // Arrange
            var defaultSerialization = new DefaultSerialization();
            var encoded = EntryConvert.EncodeClass(typeof(DummyClass), defaultSerialization);

            // Act
            encoded.SubEntries[0].Value.Current = "10";
            encoded.SubEntries[1].Value.Current = "Thomas";
            encoded.SubEntries[3].SubEntries[1].Value.Current = encoded.SubEntries[3].SubEntries[1].Value.Possible[2];
            for (var i = 4; i < 7; i++)
            {
                var colEntry = encoded.SubEntries[i];
                for (var j = 0; j < i; j++)
                {
                    var newInstance = colEntry.Prototypes[0].Instantiate();
                    newInstance.SubEntries[0].Value.Current = j.ToString("F2", defaultSerialization.FormatProvider);
                    newInstance.SubEntries[1].Value.Current = newInstance.SubEntries[1].Value.Possible[1];
                    colEntry.SubEntries.Add(newInstance);
                }

            }
            var obj = EntryConvert.CreateInstance<DummyClass>(encoded);

            // Assert
            Assert.AreEqual(10, obj.Number);
            Assert.AreEqual("Thomas", obj.Name);
            Assert.AreEqual(DummyEnum.ValueB, obj.SingleClass.Enum);
            var colAssert = new[] { CollectionType.Array, CollectionType.List, CollectionType.Enumerable };
            for (var i = 0; i < colAssert.Length; i++)
            {
                var length = 4 + i;
                var collection = ExtractCollection(colAssert[i], obj);
                Assert.AreEqual(length, collection.Count);

                if (colAssert[i] == CollectionType.Dictionary)
                {

                }
                else
                {
                    var array = (collection as IEnumerable<SubClass>).ToArray();
                    for (var j = 0; j < length; j++)
                    {
                        Assert.AreEqual((float)j, array[j].Foo);
                        Assert.AreEqual(DummyEnum.ValueA, array[j].Enum);
                    }
                }
            }
        }

        [Test]
        public void CreateInstanceWithConstructor()
        {
            // Arrange
            var dummyType = typeof(ConstructorDummy);

            // Act
            var constructors = EntryConvert.EncodeConstructors(dummyType).ToArray();
            var constructor = constructors.First(c => c.Parameters.SubEntries.Count == 1);
            constructor.Parameters.SubEntries[0].Value.Current = "42";
            var instance = (ConstructorDummy)EntryConvert.CreateInstance(dummyType, constructor);

            // Assert
            Assert.NotNull(instance);
            Assert.AreEqual(42, instance.Foo);
            Assert.AreEqual(string.Empty, instance.Text, "EntryConvert did not pick the correct overload");
        }

        [Test]
        public void EntryEqualsAfterCloning()
        {
            // Arrange
            var entry = CreateTestEntry();
            var clone = entry.Clone(true);

            // Act
            var equals = entry.Equals(clone);

            // Assert
            Assert.IsTrue(equals);
        }

        [Test]
        public void EntryEqualsWhenCheckingSameInstance()
        {
            // Arrange
            var entry = CreateTestEntry();
            var entry2 = CreateTestEntry();

            // Act
            var equals = entry.Equals(entry2);

            // Assert
            Assert.IsTrue(equals);
        }

        [Test]
        public void EntryDoesNotEqualWhenValueOnRootIsDifferent()
        {
            // Arrange
            var entry = CreateTestEntry();
            var entry2 = CreateTestEntry();

            entry.Value.Current = "OtherValue";

            // Act
            var equals = entry.Equals(entry2);

            // Assert
            Assert.IsFalse(equals);
        }

        [Test]
        public void EntryDoesNotEqualWhenNoSubEntriesArePresent()
        {
            // Arrange
            var entry = CreateTestEntry();
            var entry2 = CreateTestEntry();

            entry2.SubEntries = new List<Entry>();

            // Act
            var equals = entry.Equals(entry2);

            // Assert
            Assert.IsFalse(equals);
        }

        [Test]
        public void EntryDoesNotEqualWhenWhenASubEntryIsDifferent()
        {
            // Arrange
            var entry = CreateTestEntry();
            var entry2 = CreateTestEntry();

            entry2.SubEntries[1].Value.Current = "Value";

            // Act
            var equals = entry.Equals(entry2);

            // Assert
            Assert.IsFalse(equals);
        }

        [Test]
        public void EntryDoesNotEqualWhenWhenADeepSubEntryIsDifferent()
        {
            // Arrange
            var entry = CreateTestEntry();
            var entry2 = CreateTestEntry();

            entry2.SubEntries[2].SubEntries[0].Value.Current = "Value";

            // Act
            var equals = entry.Equals(entry2);

            // Assert
            Assert.IsFalse(equals);
        }

        [Test]
        public void EntryEqualsWithComparisonOperator()
        {
            // Arrange
            var entry = CreateTestEntry();
            var entry2 = CreateTestEntry();

            // Act
            var aEqualsB = entry == entry2;
            var aNotEqualsB = entry != entry2;

            // Assert
            Assert.IsTrue(aEqualsB);
            Assert.IsFalse(aNotEqualsB);
        }

        [Test(Description = "Encodes a MemoryStream")]
        public void MemoryStreamEncode()
        {
            // Arrange
            var testString = "This is a test";
            var streamDummy = new MemoryStreamDummy(testString);

            // Act
            var entry = EntryConvert.EncodeObject(streamDummy);

            // Assert
            Assert.AreEqual(1, entry.SubEntries.Count);
            Assert.AreEqual(EntryValueType.Stream, entry.SubEntries[0].Value.Type);
            Assert.AreEqual("VGhpcyBpcyBhIHRlc3Q=", entry.SubEntries[0].Value.Current);
        }

        [Test(Description = "Decodes to a MemoryStream and creates a new stream")]
        public void MemoryStreamDecode()
        {
            // Arrange
            var testString = "This is a test";
            var streamDummy = new MemoryStreamDummy(testString);
            var targetStreamDummy = new MemoryStreamDummy("");
            var streamInstanceToCheck = targetStreamDummy.MemoryStream;
            var entry = EntryConvert.EncodeObject(streamDummy);

            // Act
            EntryConvert.UpdateInstance(targetStreamDummy, entry);

            // Assert
            var stringValue = Encoding.UTF8.GetString(targetStreamDummy.MemoryStream.ToArray());

            Assert.AreEqual(testString, stringValue);
            Assert.AreNotSame(streamInstanceToCheck, targetStreamDummy.MemoryStream);
        }

        [Test(Description = "Decodes to a MemoryStream and reuses the stream")]
        public void MemoryStreamDecodeReuseCurrentStream()
        {
            // Arrange
            var testString = "This is a test";
            var dummyString = "12345678912345";
            var streamDummy = new MemoryStreamDummy(testString);
            var targetStreamDummy = new MemoryStreamDummy(dummyString);
            var streamInstanceToCheck = targetStreamDummy.MemoryStream;
            var entry = EntryConvert.EncodeObject(streamDummy);

            // Act
            EntryConvert.UpdateInstance(targetStreamDummy, entry);

            // Assert
            var stringValue = Encoding.UTF8.GetString(targetStreamDummy.MemoryStream.ToArray());

            Assert.AreEqual(testString, stringValue);
            Assert.AreSame(streamInstanceToCheck, targetStreamDummy.MemoryStream);
        }

        [Test(Description = "Decodes to a MemoryStream and reuses the stream but the initial stream size is greater than the new applied value")]
        public void MemoryStreamDecodeReuseCurrentStreamInitSizeIsGreaterThanNewData()
        {
            // Arrange
            var testString = "A test";
            var dummyString = "12345678912345";
            var streamDummy = new MemoryStreamDummy(testString);
            var targetStreamDummy = new MemoryStreamDummy(dummyString);
            var streamInstanceToCheck = targetStreamDummy.MemoryStream;
            var entry = EntryConvert.EncodeObject(streamDummy);

            // Act
            EntryConvert.UpdateInstance(targetStreamDummy, entry);

            // Assert
            var stringValue = Encoding.UTF8.GetString(targetStreamDummy.MemoryStream.ToArray());

            Assert.AreEqual(testString, stringValue);
            Assert.AreSame(streamInstanceToCheck, targetStreamDummy.MemoryStream);
        }

        [Test(Description = "Encodes a FileStream")]
        public void FileStreamEncode()
        {
            // Arrange
            var testString =  "This is a test";
            var testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "text.txt");
            var streamDummy = new FileStreamDummy(testFilePath, FileMode.Create);
            var testBytes = Encoding.UTF8.GetBytes(testString);
            streamDummy.FileStream.Write(testBytes, 0, testBytes.Length);

            // Act
            var entry = EntryConvert.EncodeObject(streamDummy);

            // Assert
            Assert.AreEqual(1, entry.SubEntries.Count);
            Assert.AreEqual(EntryValueType.Stream, entry.SubEntries[0].Value.Type);
            Assert.AreEqual("VGhpcyBpcyBhIHRlc3Q=", entry.SubEntries[0].Value.Current);

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
            var streamDummy = new FileStreamDummy(testFilePath, FileMode.Create);
            var targetStreamDummy = new FileStreamDummy(testFilePath2, FileMode.Create);
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
            targetStreamDummy.FileStream.Read(buffer, 0, buffer.Length);

            var stringValue = Encoding.UTF8.GetString(buffer);

            Assert.AreEqual(testString, stringValue);
            Assert.AreSame(streamInstanceToCheck, targetStreamDummy.FileStream);

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

            var dummy = new DummyClass
            {
                Number = 1001,
                SingleClass = new SubClass { Foo = 1.1234f }
            };

            // Act
            var encoded = EntryConvert.EncodeObject(dummy, serialization);
            var dummyDecoded = EntryConvert.CreateInstance<DummyClass>(encoded, serialization);

            // Assert
            Assert.AreEqual(1001.ToString(formatProvider), encoded.SubEntries[0].Value.Current);
            Assert.AreEqual(1.1234f.ToString(formatProvider), encoded.SubEntries[3].SubEntries[0].Value.Current);

            Assert.AreEqual(1001, dummyDecoded.Number);
            Assert.AreEqual(1.1234f, dummyDecoded.SingleClass.Foo);
        }

        private static Entry CreateTestEntry()
        {
            return new Entry
            {
                Identifier = "Test",
                DisplayName = "Test",
                Description = "Description",
                SubEntries =
                {
                    new Entry
                    {
                        Identifier = "L1.1",
                        DisplayName = "L1.1",
                        Description = "Description",
                        Value =
                        {
                            Current = "Level1.1",
                            Default = "",
                            Type = EntryValueType.String
                        }
                    },
                    new Entry
                    {
                        Identifier = "L1.2",
                        DisplayName = "L1.2",
                        Description = "Description",
                        Value =
                        {
                            Current = "Level1.2",
                            Default = "",
                            Type = EntryValueType.String
                        }
                    },
                    new Entry
                    {
                        Identifier = "L1.3",
                        DisplayName = "L1.3",
                        Description = "Description",
                        SubEntries =
                        {
                            new Entry
                            {
                                Identifier = "L2.3.1",
                                DisplayName = "L2.3.1",
                                Description = "Description",
                                Value =
                                {
                                    Current = "Level2.3.1",
                                    Default = "",
                                    Type = EntryValueType.String
                                }
                            }
                        },
                        Value =
                        {
                            Current = "",
                            Default = "",
                            Type = EntryValueType.Class
                        }
                    }
                },
                Value =
                {
                    Current = "Root",
                    Default = "",
                    Type = EntryValueType.String
                }
            };
        }

        public enum CollectionType
        {
            List,
            Array,
            Enumerable,
            Dictionary
        }

        private static DummyClass Prebuild(CollectionType type, int values)
        {
            var obj = new DummyClass();
            var elements = new List<SubClass>();
            var dictionary = new Dictionary<int, SubClass>();
            for (var i = 1; i <= values; i++)
            {
                elements.Add(new SubClass { Foo = i, Enum = DummyEnum.ValueA });
                dictionary.Add(i, new SubClass { Foo = i, Enum = DummyEnum.ValueA });
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

        private static Entry CollectionEntry(IEnumerable<Entry> allEntries, CollectionType type)
        {
            return allEntries.First(e => e.Identifier == $"Sub{type:G}");
        }

        private static IList ExtractCollection(CollectionType type, DummyClass obj)
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

        private static void DummyAssert(IList<Entry> encoded, IList<Entry> encodedSub)
        {
            // Assert
            var expected = new[]
            {
                new {Name = "Number", Type = EntryValueType.Int32, ReadOnly = false},
                new {Name = "Name", Type = EntryValueType.String, ReadOnly = false},
                new {Name = "ReadOnly", Type = EntryValueType.Int32, ReadOnly = true},
                new {Name = "SingleClass", Type = EntryValueType.Class, ReadOnly = false},
                new {Name = "SubArray", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "SubList", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "SubEnumerable", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "SubDictionary", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "EnumArray", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "EnumList", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "EnumEnumerable", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "BoolArray", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "BoolList", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "BoolEnumerable", Type = EntryValueType.Collection, ReadOnly = false},
                new {Name = "SingleClassNonLocalized", Type = EntryValueType.Class, ReadOnly = false}
            };
            Assert.AreEqual(expected.Length, encoded.Count, "Number of entries does not match");
            for (var i = 0; i < encoded.Count; i++)
            {
                Assert.AreEqual(expected[i].Name, encoded[i].Identifier, "Property name missmatch");
                Assert.AreEqual(expected[i].Type, encoded[i].Value.Type, "Type missmatch");
                Assert.AreEqual(expected[i].ReadOnly, encoded[i].Value.IsReadOnly, "ReadOnly missmatch");
            }
            Assert.AreEqual("Foo", encodedSub[0].Identifier, "Name missmatch");
            Assert.AreEqual(EntryValueType.Single, encodedSub[0].Value.Type, "Float not detected");
            Assert.AreEqual("Enum", encodedSub[1].Identifier);
            Assert.AreEqual(EntryValueType.Enum, encodedSub[1].Value.Type, "Enum not detected");
            Assert.AreEqual("Unset", encodedSub[1].Value.Default);
            Assert.AreEqual(3, encodedSub[1].Value.Possible.Length, "Possible values not set");
        }
    }
}
