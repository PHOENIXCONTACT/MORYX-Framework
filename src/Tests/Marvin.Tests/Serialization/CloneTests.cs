// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
{
    /// <summary>
    /// Unit tests for different clone mechanism
    /// </summary>
    [TestFixture]
    public class CloneTests
    {
        [Test(Description = "Make sure generated clone method is complete")]
        public void GeneratedClone()
        {
            // Arrange
            var root = Create(1, 16);

            // Act
            var clone = root.Clone(true);

            // Assert
            Compare(root, clone);
        }

        private static Entry Create(int id, int children)
        {
            var entry = new Entry
            {
                Description = "Some dummy entry",
                Identifier = id.ToString("D5"),
                DisplayName = string.Format("Entry-{0}", id),
                Value = new EntryValue
                {
                    Current = (id*123).ToString("D"),
                    Default = "42",
                    Type = (EntryValueType) (id%7),
                    UnitType = (EntryUnitType) (id%Enum.GetNames(typeof(EntryUnitType)).Length),
                    Possible = new[] {"12334", "1123361", "11236"}
                }
            };
            for (var i = 0; i < children; i++)
            {
                entry.SubEntries.Add(Create(id * 10 + i, children / 2));
            }
            return entry;
        }

        private static void Compare(Entry expected, Entry value)
        {
            // Compare all values
            Assert.AreEqual(expected.Description, value.Description);

            // Compare key
            Assert.AreEqual(expected.Identifier, value.Identifier);
            Assert.AreEqual(expected.DisplayName, expected.DisplayName);

            // Compare value
            Assert.AreEqual(expected.Value.Current, value.Value.Current);
            Assert.AreEqual(expected.Value.Default, value.Value.Default);
            Assert.AreEqual(expected.Value.Possible, value.Value.Possible);
            Assert.AreEqual(expected.Value.Type, value.Value.Type);
            Assert.AreEqual(expected.Value.UnitType, value.Value.UnitType);

            // Continue recursive
            Assert.AreEqual(expected.SubEntries.Count, value.SubEntries.Count);
            for (var i = 0; i < expected.SubEntries.Count; i++)
            {
                Compare(expected.SubEntries[i], value.SubEntries[i]);
            }
        }
    }
}
