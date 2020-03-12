// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Marvin.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
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
            Assert.AreEqual(73, dummyClassClient.Value);
            Assert.AreEqual(true, dummyClassClient.HasAnything);
            Assert.AreEqual("0.5", dummyClassClient.Class.Value);
            Assert.AreEqual("ValueB", dummyClassClient.Class.Enum);
            Assert.AreEqual(2, dummyClassClient.Collection.Count());
            Assert.AreEqual(2, dummyClassClient.Dictionary.Count<EntryModelSubClassDummyClient>());
            Assert.AreEqual("15.8", dummyClassClient.Dictionary["1"].Value);
            Assert.AreEqual("Unset", dummyClassClient.Dictionary["1"].Enum);
            Assert.AreEqual("435.2", dummyClassClient.Dictionary["2"].Value);
            Assert.AreEqual("ValueA", dummyClassClient.Dictionary["2"].Enum);

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
            Assert.AreEqual(174, dummyClass.Value);
            Assert.AreEqual(false, dummyClass.HasAnything);
            Assert.AreEqual(15.8f, dummyClass.Class.Value);
            Assert.AreEqual(DummyEnum.ValueA, dummyClass.Class.Enum);
            Assert.AreEqual(1, dummyClass.Collection.Count);
            Assert.AreEqual(90f, dummyClass.Collection.First().Value);
            Assert.AreEqual(DummyEnum.ValueA, dummyClass.Collection.First().Enum);
            Assert.AreEqual(1, dummyClass.Dictionary.Count);
            Assert.AreEqual(10076f, dummyClass.Dictionary[1].Value);
            Assert.AreEqual(DummyEnum.ValueB, dummyClass.Dictionary[1].Enum);
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
            Assert.AreEqual(3, dummyClient.Collection.Count());
            Assert.IsNull(dummyClient.Collection.Last().Value);
            Assert.IsNull(dummyClient.Collection.Last().Enum);

            Assert.AreEqual(3, serverEntry.SubEntries[4].SubEntries.Count);
            Assert.AreEqual(typeof(EntryModelSubClassDummyClient).Name, serverEntry.SubEntries[4].SubEntries[2].Value.Current);
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
            Assert.AreEqual(1, dummyClient.Collection.Count());
            Assert.AreEqual("0.8", dummyClient.Collection.Last().Value);
            Assert.AreEqual("ValueA", dummyClient.Collection.Last().Enum);

            Assert.AreEqual(1, serverEntry.SubEntries[4].SubEntries.Count);
            Assert.AreEqual("0.8", serverEntry.SubEntries[4].SubEntries[0].SubEntries[0].Value.Current);
            Assert.AreEqual("ValueA", serverEntry.SubEntries[4].SubEntries[0].SubEntries[1].Value.Current);
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
            Assert.AreEqual(3, dummyClient.Dictionary.Count());
            Assert.IsNull(dummyClient.Dictionary["3"].Value);
            Assert.IsNull(dummyClient.Dictionary["3"].Enum);

            Assert.AreEqual(3, serverEntry.SubEntries[5].SubEntries.Count);
            Assert.AreEqual(typeof(EntryModelSubClassDummyClient).Name, serverEntry.SubEntries[5].SubEntries[2].Value.Current);
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
            Assert.AreEqual(1, dummyClient.Dictionary.Count());
            Assert.AreEqual("4.6", dummyClient.Dictionary["1"].Value);
            Assert.AreEqual("ValueA", dummyClient.Dictionary["1"].Enum);

            Assert.AreEqual(1, serverEntry.SubEntries[5].SubEntries.Count);
            Assert.AreEqual("4.6", serverEntry.SubEntries[5].SubEntries[0].SubEntries[0].Value.Current);
            Assert.AreEqual("ValueA", serverEntry.SubEntries[5].SubEntries[0].SubEntries[1].Value.Current);
        }

        private EntryModelDummyServer CreateDummyServer()
        {
            return new EntryModelDummyServer
            {
                Value = 73,
                Text = "A Text",
                HasAnything = true,
                Class = new EntryModelSubClassDummyServer { Value = 0.5f, Enum = DummyEnum.ValueB },
                Collection = new List<EntryModelSubClassDummyServer>
                {
                    new EntryModelSubClassDummyServer {Value = 0.1f, Enum = DummyEnum.ValueB},
                    new EntryModelSubClassDummyServer {Value = 0.2f, Enum = DummyEnum.ValueA}
                },
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
                        Collection = new List<EntryModelSubClassDummyClient>
                        {
                            new EntryModelSubClassDummyClient {Value = "0.8", Enum = "ValueA"},
                            new EntryModelSubClassDummyClient {Value = "0.4", Enum = "Unset"}
                        }
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
