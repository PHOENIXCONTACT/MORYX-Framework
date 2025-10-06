// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Threading;
using Moryx.Configuration;
using Moryx.Runtime.Kernel.Tests.Dummys;
using NUnit.Framework;

namespace Moryx.Runtime.Kernel.Tests.Configuration
{
    /// <summary>
    /// Tests for the runtime config manager
    /// </summary>
    [TestFixture]
    public class RuntimeConfigManagerTests
    {
        private string _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        private ConfigManager _manager;

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            _manager = new ConfigManager();
            _manager.ConfigDirectory = _tempDirectory;

            DeleteTempFolder();
            CreateTempFolder();
        }

        /// <summary>
        /// Testing the save configuration functionality with liveupdate.
        /// </summary>
        /// <param name="liveUpdate">if set to <c>true</c> [live update].</param>
        [TestCase(true, Description = "Save with live update")]
        [TestCase(false, Description = "Save without live update")]
        public void SaveTest(bool liveUpdate)
        {
            DeleteTempFolder();
            CreateTempFolder();

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            var configChangedEvent = false;

            // create a config by saving it.
            _manager.SaveConfiguration(new RuntimeConfigManagerTestConfig1(), liveUpdate);

            // try to read the config
            var config = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>(false);
            Assert.That(config, Is.Not.Null, "Config not saved!");
            // get a copy of the config
            var copyOfConfig = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>(true);

            // add the config changed event
            config.ConfigChanged += delegate (object sender, ConfigChangedEventArgs args)
            {
                configChangedEvent = true;
                Assert.That(args.Contains(() => copyOfConfig.BooleanField), Is.True, "the changed event do not acknowlege the correct property.");
                manualResetEvent.Set();
            };

            // change a property of the copied config (the original will not cause any event on save)
            copyOfConfig.BooleanField = true;
            _manager.SaveConfiguration(copyOfConfig, liveUpdate);

            // wait for the event
            manualResetEvent.WaitOne(1000);

            // check if the event has been rised or not.
            Assert.That(liveUpdate == configChangedEvent, Is.True);
        }

        /// <summary>
        /// Testing the save configuration functionality
        /// </summary>
        [Test(Description = "Testing the save configuration functionality")]
        public void SaveTest()
        {
            DeleteTempFolder();
            CreateTempFolder();

            _manager.SaveConfiguration(new RuntimeConfigManagerTestConfig1());

            var config = _manager.GetConfiguration(typeof(RuntimeConfigManagerTestConfig1), false);

            Assert.That(config, Is.Not.Null, "Config not saved!");
        }

        /// <summary>
        /// Check if fill empty properties function loads the default values.
        /// </summary>
        [Test(Description = "Check if fill empty properties function loads the default values.")]
        public void FillEmptyTest()
        {
            // create a config item
            var emptyConfig = new RuntimeConfigManagerTestConfig1();

            // check if all system default values are not the same like the configured default values.
            Assert.That(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.False, "Nullable Boolean: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.BooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.False, "Boolean: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, Is.False, "Byte: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, Is.False, "Double: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, Is.False, "Enum: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, Is.False, "Integer: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, Is.False, "Long: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, Is.False, "String: The system default value equals the test setup default value.");

            // load the default values for unset properties
            _manager.FillEmpty(emptyConfig);

            // check if all properties got their default values.
            Assert.That(emptyConfig.BooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.True, "Boolean field has been initialized!");
            Assert.That(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, Is.True, "Enum field has not been initialized!");
            Assert.That(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.True, "Nullable boolean field has not been initialized!");
            Assert.That(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, Is.True, "Double field has not been initialized!");
            Assert.That(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, Is.True, "Integer field has not been initialized!");
            Assert.That(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, Is.True, "String field has not been initialized!");
            Assert.That(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, Is.True, "Byte field has not been initialized!");
            Assert.That(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, Is.True, "Long field has not been initialized!");
        }

        /// <summary>
        /// Check if fill empty properties function fills only 'empty' properties.
        /// </summary>
        [Test(Description = "Check if fill empty properties function fills only 'empty' properties.")]
        public void DoNotOverrideSetFieldsTest()
        {
            // create a new config
            var emptyConfig = new RuntimeConfigManagerTestConfig1();

            // set the value of the properties to non-system-default
            emptyConfig.ByteField = 129;
            emptyConfig.DoubleField = 0.1;
            emptyConfig.EnumField = TestConfig1Enum.EnumValue2;
            emptyConfig.IntField = 130;
            emptyConfig.LongField = 140;
            emptyConfig.NullableBooleanField = false;
            emptyConfig.StringField = "";
            // it makes no sense to test boolean here!

            // check if all properties are now not the same like the configured default values.
            Assert.That(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.False, "Nullable Boolean: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.BooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.False, "Boolean: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, Is.False, "Byte: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, Is.False, "Double: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, Is.False, "Enum: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, Is.False, "Integer: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, Is.False, "Long: The system default value equals the test setup default value.");
            Assert.That(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, Is.False, "String: The system default value equals the test setup default value.");

            // fillup empty properties
            _manager.FillEmpty(emptyConfig);

            // check that no property value has been overwritten.
            Assert.That(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, Is.False, "Enum field has been overwritten!");
            Assert.That(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, Is.False, "Nullable boolean field has not been overwritten!");
            Assert.That(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, Is.False, "Double field has not been overwritten!");
            Assert.That(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, Is.False, "Integer field has not been overwritten!");
            Assert.That(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, Is.False, "String field has not been overwritten!");
            Assert.That(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, Is.False, "Byte field has not been overwritten!");
            Assert.That(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, Is.False, "Long field has not been overwritten!");
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        [TearDown]
        public void Dispose()
        {
            DeleteTempFolder();
        }

        /// <summary>
        /// Creates the temporary folder.
        /// </summary>
        private void CreateTempFolder()
        {
            Assert.That(Directory.Exists(_tempDirectory), Is.False);
            Directory.CreateDirectory(_tempDirectory);
        }

        /// <summary>
        /// Deletes the temporary folder.
        /// </summary>
        private void DeleteTempFolder()
        {
            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory, true);

            Assert.That(Directory.Exists(_tempDirectory), Is.False);
        }
    }
}
