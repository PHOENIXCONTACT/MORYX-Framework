// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Threading;
using Moryx.Configuration;
using Moryx.Runtime.Kernel.Tests.Dummys;
using NUnit.Framework;

namespace Moryx.Runtime.Kernel.Tests
{
    /// <summary>
    /// Tests for the runtime config manager
    /// </summary>
    [TestFixture]
    public class RuntimeConfigManagerTests
    {
        private string _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        private RuntimeConfigManager _manager;

        /// <summary>
        /// Initializes this test.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            _manager = new RuntimeConfigManager();
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
            Assert.NotNull(config, "Config not saved!");
            // get a copy of the config
            var copyOfConfig = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>(true);

            // add the config changed event
            config.ConfigChanged += delegate(object sender, ConfigChangedEventArgs args)
            {
                configChangedEvent = true;
                Assert.True(args.Contains(() => copyOfConfig.BooleanField), "the changed event do not acknowlege the correct property.");
                manualResetEvent.Set();
            };

            // change a property of the copied config (the original will not cause any event on save)
            copyOfConfig.BooleanField = true;
            _manager.SaveConfiguration(copyOfConfig, liveUpdate);

            // wait for the event
            manualResetEvent.WaitOne(1000);

            // check if the event has been rised or not.
            Assert.True(liveUpdate == configChangedEvent);
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

            Assert.NotNull(config, "Config not saved!");
        }

        /// <summary>
        /// Tests the clear cache method
        /// </summary>
        [Test]
        public void ClearCacheTest()
        {
            var random = new Random();

            // add a config
            _manager.SaveConfiguration(new RuntimeConfigManagerTestConfig1());

            // read the config
            var config = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>();

            // change a value of the config
            config.IntField = random.Next();

            // read the config again
            var config1 = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>();

            Assert.AreEqual(config, config1, "GetConfig do not return the same config from the cache.");

            _manager.ClearCache();

            // get a new instance from the cache
            var config2 = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>();

            // check if the cache has been cleared
            Assert.AreNotEqual(config, config2, "The cache has not been cleared correctly, we get the same instance again.");
            Assert.False(config.IntField == config2.IntField, "The new value should not be saved.");

            // get config from cache
            config = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>();
            // change its value
            config.IntField = random.Next();
            // get a second config but now a copy
            config1 = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>(true);

            // check if we got a copy from the cached item
            Assert.AreNotEqual(config, config1, "We do not get a copy of the config, we get the same instance again.");
            Assert.AreNotEqual(config.IntField, config1.IntField);

            // get a config copy
            config = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>(true);
            // and change its value
            config.IntField = random.Next();
            // get a second copy
            config1 = _manager.GetConfiguration<RuntimeConfigManagerTestConfig1>();

            // check if we got a two different instances
            Assert.AreNotEqual(config, config1, "We do not get a copy of the config, we get the same instance again.");
            Assert.AreNotEqual(config.IntField, config1.IntField, "Changes on a copied item should not affect to other instances!");
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
            Assert.False(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Nullable Boolean: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.BooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Boolean: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, "Byte: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, "Double: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, "Enum: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, "Integer: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, "Long: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, "String: The system default value equals the test setup default value.");

            // load the default values for unset properties
            _manager.FillEmpty(emptyConfig);

            // check if all properties got their default values.
            Assert.True(emptyConfig.BooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Boolean field has been initialized!");
            Assert.True(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, "Enum field has not been initialized!");
            Assert.True(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Nullable boolean field has not been initialized!");
            Assert.True(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, "Double field has not been initialized!");
            Assert.True(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, "Integer field has not been initialized!");
            Assert.True(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, "String field has not been initialized!");
            Assert.True(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, "Byte field has not been initialized!");
            Assert.True(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, "Long field has not been initialized!");
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
            Assert.False(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Nullable Boolean: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.BooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Boolean: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, "Byte: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, "Double: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, "Enum: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, "Integer: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, "Long: The system default value equals the test setup default value.");
            Assert.False(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, "String: The system default value equals the test setup default value.");

            // fillup empty properties
            _manager.FillEmpty(emptyConfig);

            // check that no property value has been overwritten.
            Assert.False(emptyConfig.EnumField == RuntimeConfigManagerTestConfig1.EnumFieldDefault, "Enum field has been overwritten!");
            Assert.False(emptyConfig.NullableBooleanField == RuntimeConfigManagerTestConfig1.BooleanFieldDefault, "Nullable boolean field has not been overwritten!");
            Assert.False(emptyConfig.DoubleField.CompareTo(RuntimeConfigManagerTestConfig1.DoubleFieldDefault) == 0, "Double field has not been overwritten!");
            Assert.False(emptyConfig.IntField == RuntimeConfigManagerTestConfig1.IntFieldDefault, "Integer field has not been overwritten!");
            Assert.False(emptyConfig.StringField == RuntimeConfigManagerTestConfig1.StringFieldDefault, "String field has not been overwritten!");
            Assert.False(emptyConfig.ByteField == RuntimeConfigManagerTestConfig1.ByteFieldDefault, "Byte field has not been overwritten!");
            Assert.False(emptyConfig.LongField == RuntimeConfigManagerTestConfig1.LongFieldDefault, "Long field has not been overwritten!");
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
            Assert.False(Directory.Exists(_tempDirectory));
            Directory.CreateDirectory(_tempDirectory);
        }

        /// <summary>
        /// Deletes the temporary folder.
        /// </summary>
        private void DeleteTempFolder()
        {
            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory, true);

            Assert.False(Directory.Exists(_tempDirectory));
        }
    }
}
