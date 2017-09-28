using System.IO;
using Marvin.Configuration;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests.Configuration
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
            TestConfig config1 = _configManager.GetConfiguration<TestConfig>();
            TestConfig config2 = _configManager.GetConfiguration<TestConfig>(true);

            config1.DummyNumber++;

            Assert.AreNotEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestGetCached()
        {
            TestConfig config1 = _configManager.GetConfiguration<TestConfig>();
            TestConfig config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestClearCache()
        {
            TestConfig config1 = _configManager.GetConfiguration<TestConfig>();

            _configManager.ClearCache();

            TestConfig config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.AreNotEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestCacheOnSave()
        {
            TestConfig config1 = _configManager.GetConfiguration<TestConfig>();

            _configManager.ClearCache();
            _configManager.SaveConfiguration(config1);

            TestConfig config2 = _configManager.GetConfiguration<TestConfig>(false);

            config1.DummyNumber++;

            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestSave()
        {
            TestConfig config1 = _configManager.GetConfiguration<TestConfig>();

            config1.DummyNumber++;
            _configManager.SaveConfiguration(config1);

            TestConfig config2 = _configManager.GetConfiguration<TestConfig>(true);

            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test]
        public void TestSaveAll()
        {
            TestConfig config1 = _configManager.GetConfiguration<TestConfig>();

            config1.DummyNumber++;
            _configManager.SaveAll();

            TestConfig config2 = _configManager.GetConfiguration<TestConfig>(true);
            Assert.AreEqual(config1.DummyNumber, config2.DummyNumber);
        }
    }
}