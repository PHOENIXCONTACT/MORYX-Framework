using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
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

        //[Test(Description = "Checks if generator is 100x faster")]
        public void CloneBenchmark()
        {
            // Arrange
            var root = Create(1, 16);

            // JIT run
            var clone = root.Clone(true);
            Assert.IsTrue(Compare(root, clone), "Initial check failed!");
            var serializer = new DataContractSerializer(typeof(Entry));
            var stopWatch = new Stopwatch();

            // Run serializer
            stopWatch.Start();
            using (var memStream = new MemoryStream())
            {
                serializer.WriteObject(memStream, root);
                memStream.Seek(0, SeekOrigin.Begin);
                clone = (Entry)serializer.ReadObject(memStream);
            }
            stopWatch.Stop();
            Assert.IsTrue(Compare(root, clone), "Serializer failed!");
            var serializerTime = stopWatch.Elapsed.TotalMilliseconds;

            // Run generated
            stopWatch.Restart();
            clone = root.Clone(true);
            stopWatch.Stop();
            Assert.IsTrue(Compare(root, clone), "Generator failed");
            var generatorTime = stopWatch.Elapsed.TotalMilliseconds;

            // Compare
            var factor = serializerTime / generatorTime;
            Console.WriteLine("Speed diff: {0:F2}x faster", factor);
            //TODO: Assert.Less(100, factor, "Not faster by a factor of 100");
        }

        private static Entry Create(int id, int children)
        {
            var entry = new Entry
            {
                Description = "Some dummy entry",
                Key = new EntryKey
                {
                    Identifier = id.ToString("D5"),
                    Name = string.Format("Entry-{0}", id)
                },
                Value = new EntryValue
                {
                    Current = (id*123).ToString("D"),
                    Default = "42",
                    Type = (EntryValueType) (id%7),
                    Possible = new[] {"12334", "1123361", "11236"}
                }
            };
            for (int i = 0; i < children; i++)
            {
                entry.SubEntries.Add(Create(id * 10 + i, children / 2));
            }
            return entry;
        }

        private static bool Compare(Entry expected, Entry value)
        {
            // Compare all values
            Assert.AreEqual(expected.Description, value.Description);

            // Compare key
            Assert.AreEqual(expected.Key.Identifier, value.Key.Identifier);
            Assert.AreEqual(expected.Key.Name, expected.Key.Name);

            // Compare value
            Assert.AreEqual(expected.Value.Current, value.Value.Current);
            Assert.AreEqual(expected.Value.Default, value.Value.Default);
            Assert.AreEqual(expected.Value.Possible, expected.Value.Possible);
            Assert.AreEqual(expected.Value.Type, expected.Value.Type);

            // Continue recursive
            Assert.AreEqual(expected.SubEntries.Count, value.SubEntries.Count);
            for (var i = 0; i < expected.SubEntries.Count; i++)
            {
                Compare(expected.SubEntries[i], value.SubEntries[i]);
            }

            return true;
        }
    }
}