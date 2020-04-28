// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Marvin.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
{
    [TestFixture]
    public class EditorBrowsableSerializationTests
    {
        [Test(Description = "Retrieve only all visible methods")]
        public void GetMethodsWhereEditorBrowsableAttributeIsSet()
        {
            // Arrange

            // Act
            var methods = EntryConvert.EncodeMethods(new EditorBrowsableMixed(), new EditorBrowsableSerialization()).ToList();

            // Assert
            Assert.AreEqual(2, methods.Count);
            Assert.AreEqual(nameof(EditorBrowsableMixed.Method1), methods[0].Name);
            Assert.AreEqual("Method3", methods[1].Name);
        }

        [Test(Description = "Retrieve only all visible properties")]
        public void GetPropertiesWhereEditorBrowsableAttributeIsSet()
        {
            // Arrange

            // Act
            var entry = EntryConvert.EncodeObject(new EditorBrowsableMixed(), new EditorBrowsableSerialization());

            // Assert
            Assert.IsNotNull(entry);
            Assert.AreEqual(4, entry.SubEntries.Count);
            Assert.AreEqual(nameof(EditorBrowsableMixed.Property1), entry.SubEntries[0].DisplayName);
            Assert.AreEqual("Property2", entry.SubEntries[1].DisplayName);
            Assert.AreEqual(nameof(EditorBrowsableMixed.Property3), entry.SubEntries[2].DisplayName);
            Assert.AreEqual(nameof(EditorBrowsableMixed.Property5), entry.SubEntries[3].DisplayName);
        }

        [Test(Description = "Retrieve only all visible methods and properties on a class where no EditorBrowsableAttributes are set")]
        public void GetNoMethodsAndPropertiesWhereNoEditorBrowsableAttributeIsSet()
        {
            // Arrange

            // Act
            var entries = EntryConvert.EncodeObject(new NoEditorBrowsableSet(), new EditorBrowsableSerialization());

            // Assert
            Assert.AreEqual(0, entries.SubEntries.Count);
        }
    }
}