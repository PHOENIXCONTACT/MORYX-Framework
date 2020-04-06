// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Marvin.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
{
    [TestFixture]
    public class EditorVisibleSerializationTests
    {
        [Test(Description = "Retrieve only all visible methods")]
        public void GetMethodsWhereEditorVisibleAttributeIsSet()
        {
            // Arrange
            
            // Act
            var methods = EntryConvert.EncodeMethods(new EditorVisibleMixed(), new EditorVisibleSerialization()).ToList();
            
            // Assert
            Assert.AreEqual(2, methods.Count);
            Assert.AreEqual(nameof(EditorVisibleMixed.Method1), methods[0].Name);
            Assert.AreEqual("Method3", methods[1].Name);
        }

        [Test(Description = "Retrieve only all visible properties")]
        public void GetPropertiesWhereEditorVisibleAttributeIsSet()
        {
            // Arrange

            // Act
            var entry = EntryConvert.EncodeObject(new EditorVisibleMixed(), new EditorVisibleSerialization());

            // Assert
            Assert.IsNotNull(entry);
            Assert.AreEqual(4, entry.SubEntries.Count);
            Assert.AreEqual(nameof(EditorVisibleMixed.Property1), entry.SubEntries[0].Name);
            Assert.AreEqual("Property2", entry.SubEntries[1].Name);
            Assert.AreEqual(nameof(EditorVisibleMixed.Property3), entry.SubEntries[2].Name);
            Assert.AreEqual(nameof(EditorVisibleMixed.Property5), entry.SubEntries[3].Name);
        }

        [Test(Description = "Retrieve only all visible methods and properties on a class where no EditorVisible attributes are set")]
        public void GetNoMethodsAndPropertiesWhereNoEditorVisibleAttributeIsSet()
        {
            // Arrange

            // Act
            var entries = EntryConvert.EncodeObject(new NoEditorVisibleSet(), new EditorVisibleSerialization());

            // Assert
            Assert.AreEqual(0, entries.SubEntries.Count);
        }
    }
}
