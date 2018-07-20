using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marvin.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
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

            DummyAssert(encoded, encodedSub);
        }

        [Test]
        public void CreateInstanceWithArray()
        {
            //Arrange
            var type = typeof(ArrayDummy);
            var encoded = EntryConvert.EncodeClass(type);

            var entry1 = encoded.SubEntries[0];
            var entry2 = encoded.SubEntries[1];

            for (int i = 1; i <= 5; i++)
            {
                var newInstance = entry1.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                entry1.SubEntries.Add(newInstance);
            }

            for (int i = 1; i <= 5; i++)
            {
                var newInstance = entry2.Prototypes[0].Instantiate();
                newInstance.Value.Current = "Number: " + i;
                entry2.SubEntries.Add(newInstance);
            }

            //Act
            var dummy = EntryConvert.CreateInstance<ArrayDummy>(encoded);

            //Assert
            for (int i = 1; i <= 5; i++)
            {
                Assert.AreEqual(i + 1, dummy.Array[i - 1]);
                Assert.AreEqual("Number: " + i, dummy.Keys[i - 1]);
            }
        }

        [Test]
        public void CreateInstanceWithDictionary()
        {
            //Arrange
            var type = typeof(DictionaryClass);
            var encoded = EntryConvert.EncodeClass(type);

            var entry1 = encoded.SubEntries[1];
            var entry2 = encoded.SubEntries[2];

            for (int i = 1; i <= 5; i++)
            {
                var newInstance = entry1.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                newInstance.Key.Name = "Key" + i;
                entry1.SubEntries.Add(newInstance);
            }

            for (int i = 1; i <= 3; i++)
            {
                var newInstance = entry2.Prototypes[0].Instantiate();
                newInstance.Value.Current = ((DummyEnum)(i % 3)).ToString();
                newInstance.Key.Name = "Key_0121" + i;
                entry2.SubEntries.Add(newInstance);
            }


            //Act
            var dummy = EntryConvert.CreateInstance<DictionaryClass>(encoded);

            //Assert
            for (int i = 1; i <= 5; i++)
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
            var dummy = new DictionaryClass()
            {
                SubDictionary = new Dictionary<string, int>()
                {
                    {"077", 6 },
                    {"088", 9 }
                },
                EnumDictionary = new Dictionary<string, DummyEnum>()
                {
                    {"011", DummyEnum.Unset },
                    {"022", DummyEnum.ValueB }
                } 
            };

            //Arrange
            var encoded = EntryConvert.EncodeObject(dummy);

            var entry1 = encoded.SubEntries[1];
            var entry2 = encoded.SubEntries[2];


            for (int i = 1; i <= 2; i++)
            {
                var newInstance = entry1.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                newInstance.Key.Name = "Key" + i;
                entry1.SubEntries.Add(newInstance);
            }

            entry1.SubEntries[0].Value.Current = "123";
            entry1.SubEntries[0].Key.Name = "022";

            for (int i = 1; i <= 3; i++)
            {
                var newInstance = entry2.Prototypes[0].Instantiate();
                newInstance.Value.Current = ((DummyEnum)(i % 3)).ToString();
                newInstance.Key.Name = "Key_0121" + i;
                entry2.SubEntries.Add(newInstance);
            }

            entry2.SubEntries[0].Value.Current = DummyEnum.ValueA.ToString();
            entry2.SubEntries[0].Key.Name = "555";


            //Act
            EntryConvert.UpdateInstance(dummy, encoded);

            //Assert
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
                Keys = new string[] { "test1_2", "test_02", "1245" },
                Enums = new DummyEnum[] { DummyEnum.Unset, DummyEnum.ValueB, DummyEnum.ValueA }
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
            //Arrange
            var type = typeof(ListDummy);
            var encoded = EntryConvert.EncodeClass(type);
            var ent = encoded.SubEntries[1];

            for (int i = 1; i <= 5; i++)
            {
                var newInstance = ent.Prototypes[0].Instantiate();
                newInstance.Value.Current = (1 + i).ToString();
                ent.SubEntries.Add(newInstance);
            }

            //Act
            var listDummy = EntryConvert.CreateInstance<ListDummy>(encoded);

            //Assert
            for (int i = 1; i <= 5; i++)
            {
                Assert.AreEqual(i + 1, listDummy.DoubleList[i - 1]);
            }

        }

        [Test]
        public void UpdatePrimitiveList()
        {
            //Arrange
            var dummy = new ListDummy()
            {
                Number = 0,
                DoubleList = new List<double>() {1.7, 2.5, 3},
                EnumList = new List<DummyEnum>() { DummyEnum.ValueA, DummyEnum.Unset, DummyEnum.ValueB}
            };

            var encoded = EntryConvert.EncodeObject(dummy);
            var ent = encoded.SubEntries[1];
            var ent2 = encoded.SubEntries[2];

            //Act
            encoded.SubEntries[0].Value.Current = "5";
            ent.SubEntries[1].Value.Current = 12.34d.ToString();
            var newInstance = ent.Prototypes[0].Instantiate();
            newInstance.Value.Current = 133.7d.ToString();
            ent.SubEntries.Add(newInstance);

            ent2.SubEntries[1].Value.Current = "ValueB";
            newInstance = ent2.Prototypes[0].Instantiate();
            newInstance.Value.Current = "ValueA";
            ent2.SubEntries.Add(newInstance);

            EntryConvert.UpdateInstance(dummy, encoded);

            //Assert
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
                SingleClass = null,//new SubClass { Foo = (float)1.2, Enum = DummyEnum.ValueB },
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
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(expected[i], encoded[i].Value.Current, "Property value missmatch");
            }
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
            var obj = Prebuild(type, prefill);
            var encoded = EntryConvert.EncodeObject(obj);

            // Act
            var colEntry = CollectionEntry(encoded.SubEntries, type);

            if (type == CollectionType.Dictionary)
            {
                for (int i = 1; i <= newValues; i++)
                {
                    var newInstance = colEntry.Prototypes[0].Instantiate();
                    //change "Key" + 10
                    newInstance.Key.Name = (prefill + i).ToString();
                    // change "Value" 
                    newInstance.SubEntries[0].Value.Current = (prefill + i).ToString("F2");
                    newInstance.SubEntries[1].Value.Current = newInstance.SubEntries[1].Value.Possible[2];
                    newInstance.Key.Identifier = EntryKey.CreatedIdentifier;

                    colEntry.SubEntries.Add(newInstance);
                }
            }
            else
            {
                for (int i = 1; i <= newValues; i++)
                {
                    var newInstance = colEntry.Prototypes[0].Instantiate();
                    newInstance.SubEntries[0].Value.Current = (prefill + i).ToString();
                    newInstance.SubEntries[1].Value.Current = newInstance.SubEntries[1].Value.Possible[2];
                    newInstance.Key.Identifier = EntryKey.CreatedIdentifier;
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
                for (int i = 0; i < totalSize; i++)
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
                for (int i = 0; i < totalSize; i++)
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
                    entry.Key.Name = "1" + entry.SubEntries[0].Value.Current;
                    //entry.Key.Identifier = EntryKey.CreatedIdentifier;
                    // change "Value" 
                    entry.SubEntries[0].Value.Current = "1" + entry.SubEntries[0].Value.Current;
                    entry.SubEntries[1].Value.Current = entry.SubEntries[1].Value.Possible[2];
                }

                EntryConvert.UpdateInstance(obj, encoded);

                // Assert
                var collection = ExtractCollection(type, obj);

                for (int i = 0; i < collection.Count; i++)
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

                for (int i = 0; i < collection.Count; i++)
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
                colEntry.SubEntries.RemoveAll(e => removedIndexes.Contains(int.Parse(e.Key.Identifier) - 1));
            else
                colEntry.SubEntries.RemoveAll(e => removedIndexes.Contains(int.Parse(e.Key.Identifier)));
            EntryConvert.UpdateInstance(obj, encoded);

            // Assert
            var collection = ExtractCollection(type, obj);
            var totalSize = prefill - removedIndexes.Length;
            Assert.AreEqual(totalSize, collection.Count, "New size invalid");


            if (type == CollectionType.Dictionary)
            {
                var array = (collection as IEnumerable<KeyValuePair<int, SubClass>>).ToArray();
                for (int i = 0; i < prefill; i++)
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
                for (int i = 0; i < prefill; i++)
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
            var encoded = EntryConvert.EncodeClass(typeof(DummyClass));

            // Act
            encoded.SubEntries[0].Value.Current = "10";
            encoded.SubEntries[1].Value.Current = "Thomas";
            encoded.SubEntries[3].SubEntries[1].Value.Current = encoded.SubEntries[3].SubEntries[1].Value.Possible[2];
            for (int i = 4; i < 7; i++)
            {
                var colEntry = encoded.SubEntries[i];
                for (int j = 0; j < i; j++)
                {
                    var newInstance = colEntry.Prototypes[0].Instantiate();
                    newInstance.SubEntries[0].Value.Current = j.ToString("F2");
                    newInstance.SubEntries[1].Value.Current = newInstance.SubEntries[1].Value.Possible[1];
                    newInstance.Key.Identifier = EntryKey.CreatedIdentifier;
                    colEntry.SubEntries.Add(newInstance);
                }

            }
            var obj = EntryConvert.CreateInstance<DummyClass>(encoded);

            // Assert
            Assert.AreEqual(10, obj.Number);
            Assert.AreEqual("Thomas", obj.Name);
            Assert.AreEqual(DummyEnum.ValueB, obj.SingleClass.Enum);
            var colAssert = new[] { CollectionType.Array, CollectionType.List, CollectionType.Enumerable, };
            for (int i = 0; i < colAssert.Length; i++)
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
                    for (int j = 0; j < length; j++)
                    {
                        Assert.AreEqual((float)j, array[j].Foo);
                        Assert.AreEqual(DummyEnum.ValueA, array[j].Enum);
                    }
                }
            }
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
            for (int i = 1; i <= values; i++)
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
                    throw new ArgumentOutOfRangeException("type");
            }
            return obj;
        }

        private static Entry CollectionEntry(IEnumerable<Entry> allEntries, CollectionType type)
        {
            return allEntries.First(e => e.Key.Identifier == string.Format("Sub{0}", type.ToString("G")));
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
                    throw new ArgumentOutOfRangeException("type");
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
            };
            Assert.AreEqual(expected.Length, encoded.Count, "Number of entries does not match");
            for (int i = 0; i < encoded.Count; i++)
            {
                Assert.AreEqual(expected[i].Name, encoded[i].Key.Identifier, "Property name missmatch");
                Assert.AreEqual(expected[i].Type, encoded[i].Value.Type, "Type missmatch");
                Assert.AreEqual(expected[i].ReadOnly, encoded[i].Value.IsReadOnly, "ReadOnly missmatch");
            }
            Assert.AreEqual("Foo", encodedSub[0].Key.Identifier, "Name missmatch");
            Assert.AreEqual(EntryValueType.Single, encodedSub[0].Value.Type, "Float not detected");
            Assert.AreEqual("Enum", encodedSub[1].Key.Identifier);
            Assert.AreEqual(EntryValueType.Enum, encodedSub[1].Value.Type, "Enum not detected");
            Assert.AreEqual("Unset", encodedSub[1].Value.Default);
            Assert.AreEqual(3, encodedSub[1].Value.Possible.Length, "Possible values not set");
        }
    }
}