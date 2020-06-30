// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;
using Marvin.Configuration;
using NUnit.Framework;

namespace Marvin.Tests.Configuration
{
    [TestFixture]
    public class CachedConfigManagerTests
    {
        private const string ConfigDir = "Configs";

        private string _fullConfigDir;
        private CachedConfigManager _configManager;

        [SetUp]
        public void Setup()
        {
            // Clear configs dir
            _fullConfigDir = Path.Combine(Directory.GetCurrentDirectory(), ConfigDir);
            Directory.CreateDirectory(_fullConfigDir);

            // Init config manager
            _configManager = new CachedConfigManager { ConfigDirectory = ConfigDir };
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_fullConfigDir, true);
        }

        [Test]
        public void TestGetCopy()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();
            var config2 = _configManager.GetConfiguration<TestConfig>(true);

            config1.DummyNumber++;

            Assert.AreNotEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestGetCached()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();
            var config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestClearCache()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();

            _configManager.ClearCache();

            var config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.AreNotEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestCacheOnSave()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();

            _configManager.ClearCache();
            _configManager.SaveConfiguration(config1);

            var config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestSave()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();

            config1.DummyNumber++;
            _configManager.SaveConfiguration(config1);

            var config2 = _configManager.GetConfiguration<TestConfig>(true);

            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestSaveAll_NonDefaultPersistedConfigWasNotPersisted()
        {
            var config1 = _configManager.GetConfiguration<NonPersistedTestConfig>();

            _configManager.SaveAll();

            Assert.IsFalse(Directory.GetFiles(_fullConfigDir).Any());
        }

        [Test]
        public void TestSaveAll_NonDefaultPersistingConfigWasPersistedManually()
        {
            var config1 = _configManager.GetConfiguration<NonPersistedTestConfig>();
            _configManager.SaveConfiguration(config1);

            config1.DummyNumber += 2;
            _configManager.SaveAll();

            var config2 = _configManager.GetConfiguration<NonPersistedTestConfig>(true);
            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestSaveAll_DefaultPersistingConfigWasPersisted()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();

            config1.DummyNumber += 1;
            _configManager.SaveAll();

            var config2 = _configManager.GetConfiguration<TestConfig>(true);
            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }
    }
}
