// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests
{
    /// <summary>
    /// Unit tests for the static <see cref="EntryToModelConverter"/> class
    /// </summary>
    [TestFixture]
    public class EntryToModelConverterTests
    {
        [Test]
        public void ConvertFromModelAndBackToModel()
        {
            // Arrange
            var dummyClass = CreateDummyServer();
            var dummyClassClient = new EntryModelDummyClient();

            // Act
            var serverEntry = EntryConvert.EncodeObject(dummyClass, new DefaultSerialization { FormatProvider = new CultureInfo("en-us") });

            var clientConverter = EntryToModelConverter.Create<EntryModelDummyClient>(new CultureInfo("en-us"));
            clientConverter.FromModel(serverEntry, dummyClassClient);

            // Assert
            // Check server to client conversion
            Assert.That(dummyClassClient.Value, Is.EqualTo(73));
            Assert.That(dummyClassClient.HasAnything, Is.EqualTo(true));
            Assert.That(dummyClassClient.Class.Value, Is.EqualTo("0.5"));
            Assert.That(dummyClassClient.Class.Enum, Is.EqualTo("ValueB"));
            Assert.That(dummyClassClient.Collection.Count(), Is.EqualTo(2));
            Assert.That(dummyClassClient.Dictionary.Count(), Is.EqualTo(2));
            Assert.That(dummyClassClient.Dictionary["1"].Value, Is.EqualTo("15.8"));
            Assert.That(dummyClassClient.Dictionary["1"].Enum, Is.EqualTo("Unset"));
            Assert.That(dummyClassClient.Dictionary["2"].Value, Is.EqualTo("435.2"));
            Assert.That(dummyClassClient.Dictionary["2"].Enum, Is.EqualTo("ValueA"));

            // Arrange
            dummyClassClient.Value = 174;
            dummyClassClient.HasAnything = false;
            dummyClassClient.Class.Value = "15.8";
            dummyClassClient.Class.Enum = "ValueA";
            dummyClassClient.Collection.Remove(dummyClassClient.Collection.First());
            dummyClassClient.Collection.First().Value = "90";
            dummyClassClient.Collection.First().Enum = "ValueA";
            dummyClassClient.Dictionary["1"].Value = "10076";
            dummyClassClient.Dictionary["1"].Enum = "ValueB";
            dummyClassClient.Dictionary.Remove("2");

            // Act
            clientConverter.ToModel(dummyClassClient, serverEntry);

            EntryConvert.UpdateInstance(dummyClass, serverEntry, new DefaultSerialization { FormatProvider = new CultureInfo("en-us") });

            // Assert
            // Check client to server conversion
            Assert.That(dummyClass.Value, Is.EqualTo(174));
            Assert.That(dummyClass.HasAnything, Is.EqualTo(false));
            Assert.That(dummyClass.Class.Value, Is.EqualTo(15.8f));
            Assert.That(dummyClass.Class.Enum, Is.EqualTo(DummyEnum.ValueA));
            Assert.That(dummyClass.Collection.Count, Is.EqualTo(1));
            Assert.That(dummyClass.Collection.First().Value, Is.EqualTo(90f));
            Assert.That(dummyClass.Collection.First().Enum, Is.EqualTo(DummyEnum.ValueA));
            Assert.That(dummyClass.Dictionary.Count, Is.EqualTo(1));
            Assert.That(dummyClass.Dictionary[1].Value, Is.EqualTo(10076f));
            Assert.That(dummyClass.Dictionary[1].Enum, Is.EqualTo(DummyEnum.ValueB));
        }

        [Test]
        public void AddItemToEntryCollection()
        {
            // Arrange
            var dummyClass = CreateDummyServer();
            var dummyClient = CreateDummyClient();

            // Act
            var serverEntry = EntryConvert.EncodeObject(dummyClass, new DefaultSerialization { FormatProvider = new CultureInfo("en-us") });

            dummyClient.Collection.Add(typeof(EntryModelSubClassDummyClient).Name);

            var clientConverter = EntryToModelConverter.Create<EntryModelDummyClient>(new CultureInfo("en-us"));
            clientConverter.ToModel(dummyClient, serverEntry);

            // Assert
            Assert.That(dummyClient.Collection.Count(), Is.EqualTo(3));
            Assert.That(dummyClient.Collection.Last().Value, Is.Null);
            Assert.That(dummyClient.Collection.Last().Enum, Is.Null);

            Assert.That(serverEntry.SubEntries[4].SubEntries.Count, Is.EqualTo(3));
            Assert.That(serverEntry.SubEntries[4].SubEntries[2].Value.Current, Is.EqualTo(typeof(EntryModelSubClassDummyClient).Name));
        }

        [Test]
        public void RemoveItemFromCollectionWhichIsArray()
        {
            //Arrange
            var dummy = new DummyClassIList();
            dummy.Number = 10;
            dummy.Name = "Thomas";
            dummy.SingleClass = null;
            dummy.SubArray = [new SubClass { Foo = (float)1.2, Enum = DummyEnum.ValueB }];
            dummy.SubList = [new SubClass { Foo = (float)3.4, Enum = DummyEnum.ValueA }];
            dummy.SubEnumerable = new List<SubClass> { new SubClass { Foo = (float)3.4, Enum = DummyEnum.ValueA } };
            dummy.SubDictionary = new Dictionary<int, SubClass>();
            dummy.SubIList = [1, 2, 3, 7];

            var entry = EntryConvert.EncodeObject(dummy);
            var x = entry.SubEntries.FirstOrDefault(e => e.Identifier == "SubIList");
            x.SubEntries.RemoveAt(0);

            //Act
            EntryConvert.UpdateInstance(dummy, entry);

            //Assert
            Assert.That(3, Is.EqualTo(dummy.SubIList.Count));
        }

        [Test]
        public void AddItemToCollectionWhichIsArray()
        {
            //Arrange
            var dummy = new DummyClassIList();
            dummy.Number = 10;
            dummy.Name = "Thomas";
            dummy.SingleClass = null;
            dummy.SubArray = [new SubClass { Foo = (float)1.2, Enum = DummyEnum.ValueB }];
            dummy.SubList = [new SubClass { Foo = (float)3.4, Enum = DummyEnum.ValueA }];
            dummy.SubEnumerable = new List<SubClass> { new SubClass { Foo = (float)3.4, Enum = DummyEnum.ValueA } };
            dummy.SubDictionary = new Dictionary<int, SubClass>();
            dummy.SubIList = [1, 2, 3, 7];

            var entry = EntryConvert.EncodeObject(dummy);
            var x = entry.SubEntries.FirstOrDefault(e => e.Identifier == "SubIList");
            x.SubEntries.Add(x.Prototypes.First());

            //Act
            EntryConvert.UpdateInstance(dummy, entry);

            //Assert
            Assert.That(5, Is.EqualTo(dummy.SubIList.Count));
        }

        [Test]
        public void RemoveItemFromEntryCollection()
        {
            // Arrange
            var dummyClass = CreateDummyServer();
            var dummyClient = CreateDummyClient();

            // Act
            var serverEntry = EntryConvert.EncodeObject(dummyClass, new DefaultSerialization { FormatProvider = new CultureInfo("en-us") });

            dummyClient.Collection.Remove(dummyClient.Collection.Last());

            var clientConverter = EntryToModelConverter.Create<EntryModelDummyClient>(new CultureInfo("en-us"));
            clientConverter.ToModel(dummyClient, serverEntry);

            // Assert
            Assert.That(dummyClient.Collection.Count(), Is.EqualTo(1));
            Assert.That(dummyClient.Collection.Last().Value, Is.EqualTo("0.8"));
            Assert.That(dummyClient.Collection.Last().Enum, Is.EqualTo("ValueA"));

            Assert.That(serverEntry.SubEntries[4].SubEntries.Count, Is.EqualTo(1));
            Assert.That(serverEntry.SubEntries[4].SubEntries[0].SubEntries[0].Value.Current, Is.EqualTo("0.8"));
            Assert.That(serverEntry.SubEntries[4].SubEntries[0].SubEntries[1].Value.Current, Is.EqualTo("ValueA"));
        }

        [Test]
        public void AddItemToEntryDictionary()
        {
            // Arrange
            var dummyClass = CreateDummyServer();
            var dummyClient = CreateDummyClient();

            // Act
            var serverEntry = EntryConvert.EncodeObject(dummyClass, new DefaultSerialization { FormatProvider = new CultureInfo("en-us") });

            dummyClient.Dictionary.Add("3", typeof(EntryModelSubClassDummyClient).Name);

            var clientConverter = EntryToModelConverter.Create<EntryModelDummyClient>(new CultureInfo("en-us"));
            clientConverter.ToModel(dummyClient, serverEntry);

            // Assert
            Assert.That(dummyClient.Dictionary.Count(), Is.EqualTo(3));
            Assert.That(dummyClient.Dictionary["3"].Value, Is.Null);
            Assert.That(dummyClient.Dictionary["3"].Enum, Is.Null);

            Assert.That(serverEntry.SubEntries[5].SubEntries.Count, Is.EqualTo(3));
            Assert.That(serverEntry.SubEntries[5].SubEntries[2].Value.Current, Is.EqualTo(typeof(EntryModelSubClassDummyClient).Name));
        }

        [Test]
        public void RemoveItemFromEntryDictionary()
        {
            // Arrange
            var dummyClass = CreateDummyServer();
            var dummyClient = CreateDummyClient();

            // Act
            var serverEntry = EntryConvert.EncodeObject(dummyClass, new DefaultSerialization { FormatProvider = new CultureInfo("en-us") });

            dummyClient.Dictionary.Remove("2");

            var clientConverter = EntryToModelConverter.Create<EntryModelDummyClient>(new CultureInfo("en-us"));
            clientConverter.ToModel(dummyClient, serverEntry);

            // Assert
            Assert.That(dummyClient.Dictionary.Count(), Is.EqualTo(1));
            Assert.That(dummyClient.Dictionary["1"].Value, Is.EqualTo("4.6"));
            Assert.That(dummyClient.Dictionary["1"].Enum, Is.EqualTo("ValueA"));

            Assert.That(serverEntry.SubEntries[5].SubEntries.Count, Is.EqualTo(1));
            Assert.That(serverEntry.SubEntries[5].SubEntries[0].SubEntries[0].Value.Current, Is.EqualTo("4.6"));
            Assert.That(serverEntry.SubEntries[5].SubEntries[0].SubEntries[1].Value.Current, Is.EqualTo("ValueA"));
        }

        private EntryModelDummyServer CreateDummyServer()
        {
            return new EntryModelDummyServer
            {
                Value = 73,
                Text = "A Text",
                HasAnything = true,
                Class = new EntryModelSubClassDummyServer { Value = 0.5f, Enum = DummyEnum.ValueB },
                Collection =
                [
                    new EntryModelSubClassDummyServer { Value = 0.1f, Enum = DummyEnum.ValueB },
                    new EntryModelSubClassDummyServer { Value = 0.2f, Enum = DummyEnum.ValueA }
                ],
                Dictionary = new Dictionary<int, EntryModelSubClassDummyServer>
                {
                    { 1, new EntryModelSubClassDummyServer { Value = 15.8f, Enum = DummyEnum.Unset } },
                    { 2, new EntryModelSubClassDummyServer { Value = 435.2f, Enum = DummyEnum.ValueA } }
                }
            };
        }

        private EntryModelDummyClient CreateDummyClient()
        {
            return new EntryModelDummyClient
            {
                Value = 76,
                HasAnything = false,
                Class = new EntryModelSubClassDummyClient { Value = "0.2", Enum = "Unset" },
                Collection = new EntryCollection<EntryModelSubClassDummyClient>(EntryConvert.EncodeObject(
                    new EntryModelListHelperDummy
                    {
                        Collection =
                        [
                            new EntryModelSubClassDummyClient { Value = "0.8", Enum = "ValueA" },
                            new EntryModelSubClassDummyClient { Value = "0.4", Enum = "Unset" }
                        ]
                    }).SubEntries[0]),
                Dictionary = new EntryDictionary<EntryModelSubClassDummyClient>(EntryConvert.EncodeObject(
                    new EntryModelDictionaryHelperDummy
                    {
                        Dictionary = new Dictionary<int, EntryModelSubClassDummyClient>
                        {
                            {1, new EntryModelSubClassDummyClient { Value = "4.6", Enum = "ValueA" } },
                            {2, new EntryModelSubClassDummyClient { Value = "79.1", Enum = "ValueB" } }
                        }
                    }
                    ).SubEntries[0])
            };
        }
    }
}
