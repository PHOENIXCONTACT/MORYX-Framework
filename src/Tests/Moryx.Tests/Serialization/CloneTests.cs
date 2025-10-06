// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests
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
                    Current = (id * 123).ToString("D"),
                    Default = "42",
                    Type = (EntryValueType)(id % 7),
                    UnitType = (EntryUnitType)(id % Enum.GetNames(typeof(EntryUnitType)).Length),
                    Possible = ["12334", "1123361", "11236"]
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
            Assert.That(value.Description, Is.EqualTo(expected.Description));

            // Compare key
            Assert.That(value.Identifier, Is.EqualTo(expected.Identifier));
            Assert.That(expected.DisplayName, Is.EqualTo(expected.DisplayName));

            // Compare value
            Assert.That(value.Value.Current, Is.EqualTo(expected.Value.Current));
            Assert.That(value.Value.Default, Is.EqualTo(expected.Value.Default));
            Assert.That(value.Value.Possible, Is.EqualTo(expected.Value.Possible));
            Assert.That(value.Value.Type, Is.EqualTo(expected.Value.Type));
            Assert.That(value.Value.UnitType, Is.EqualTo(expected.Value.UnitType));

            // Continue recursive
            Assert.That(value.SubEntries.Count, Is.EqualTo(expected.SubEntries.Count));
            for (var i = 0; i < expected.SubEntries.Count; i++)
            {
                Compare(expected.SubEntries[i], value.SubEntries[i]);
            }
        }
    }
}
