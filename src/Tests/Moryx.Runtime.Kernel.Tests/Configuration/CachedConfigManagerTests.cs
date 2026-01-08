// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using Moryx.Configuration;
using NUnit.Framework;

namespace Moryx.Runtime.Kernel.Tests.Configuration
{
    [TestFixture]
    public class CachedConfigManagerTests
    {
        private const string ConfigDir = "Configs";

        private string _fullConfigDir;
        private ConfigManager _configManager;

        [SetUp]
        public void Setup()
        {
            // Clear configs dir
            _fullConfigDir = Path.Combine(Directory.GetCurrentDirectory(), ConfigDir);
            Directory.CreateDirectory(_fullConfigDir);

            // Init config manager
            _configManager = new ConfigManager { ConfigDirectory = ConfigDir };
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

            Assert.That(config1.DummyNumber, Is.Not.EqualTo(config2.DummyNumber));
        }

        [Test]
        public void TestGetCached()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();
            var config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.That(config2.DummyNumber, Is.EqualTo(config1.DummyNumber));
        }

        [Test]
        public void TestSave()
        {
            var config1 = _configManager.GetConfiguration<TestConfig>();

            config1.DummyNumber++;
            _configManager.SaveConfiguration(config1);

            var config2 = _configManager.GetConfiguration<TestConfig>(true);

            Assert.That(config2.DummyNumber, Is.EqualTo(config1.DummyNumber));
        }
    }
}
