using System.IO;
using System.Linq;
using Marvin.Configuration;
using NUnit.Framework;

namespace Marvin.Tests.Configuration
{
    [TestFixture]
    public class ConfigManagerTests
    {
        private const string ConfigDir = "Configs";

        private string _fullConfigDir;
        private IConfigManager _configManager;

        [SetUp]
        public void Setup()
        {
            // Clear configs dir
            _fullConfigDir = Path.Combine(Directory.GetCurrentDirectory(), ConfigDir);
            Directory.CreateDirectory(_fullConfigDir);

            // Init config manager
            _configManager = new ConfigManager { ConfigDirectory = ConfigDir };
        }

        [Test]
        public void InitialLoad()
        {
            // Load file from empty config an check defaults
            var config = _configManager.GetConfiguration<TestConfig>();

            // Check if all properies and subclasses are filled
            Assert.AreEqual(DefaultValues.Number, config.DummyNumber, "Integer value was not initialized");
            Assert.AreEqual(DefaultValues.Number, config.DummyShort, "UShort value was not initialized");
            Assert.AreEqual(DefaultValues.Text, config.DummyString, "String was not initialized");
            Assert.NotNull(config.Child, "Child property was not initialized with class instance");
            Assert.AreEqual(DefaultValues.Decimal, config.Child.DummyDouble, "Double was not initialized");
        }

        [Test]
        public void FileCreated()
        {
            var config = _configManager.GetConfiguration<TestConfig>();
            _configManager.SaveConfiguration(config);

            Assert.True(Directory.GetFiles(_fullConfigDir).Any(), "No config file was created!");
        }

        [Test]
        public void ValuesSaved()
        {
            // Load file
            var config = _configManager.GetConfiguration<TestConfig>();
            config.DummyNumber = ModifiedValues.Number;
            config.DummyString = ModifiedValues.Text;
            config.Child.DummyDouble = ModifiedValues.Decimal;

            // Save changes
            _configManager.SaveConfiguration(config);

            // Load config again
            var reloadConfig = _configManager.GetConfiguration<TestConfig>(true);

            Assert.AreEqual(config.DummyNumber, reloadConfig.DummyNumber, "Number change was not saved");
            Assert.AreEqual(config.DummyString, reloadConfig.DummyString, "Text change was not saved");
            Assert.AreEqual(config.Child.DummyDouble, reloadConfig.Child.DummyDouble, "Decimal was not saved");
        }

        [Test]
        public void DefaultRestore()
        {
            var config = _configManager.GetConfiguration<TestConfig>();
            config.DummyNumber = 0;
            _configManager.SaveConfiguration(config);

            config = _configManager.GetConfiguration<TestConfig>();
            Assert.AreEqual(DefaultValues.Number, config.DummyNumber, "Default not restored");
        }

        [Test]
        public void ConfigSelfRepair()
        {
            const string configName = "Marvin.Tests.Configuration.TestConfig";

            // Write faulty config to file
            var fileName = Path.Combine(_fullConfigDir, configName + ConfigConstants.FileExtension);
            File.WriteAllText(fileName, FaultyConfig.Content());

            // Load config an check if present values where preserved
            var config = _configManager.GetConfiguration<TestConfig>(configName);
            Assert.AreEqual(ModifiedValues.Text, config.DummyString, "Modified value not recovered!");
            Assert.AreEqual(ModifiedValues.Decimal, config.Child.DummyDouble, "Decimal value not preserved!");
            Assert.AreEqual(DefaultValues.Number, config.DummyNumber, "Default value not restored");
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

            // The ConfigManager does not cahce at all. Therefore it should return always a copy.
            Assert.AreNotEqual(config1.DummyNumber, config2.DummyNumber);
        }

        [Test(Description = "Saves and loads a configuration by a custom name.")]
        public void ByName()
        {
            //Arrange
            const string configName = "CustomName";
            const string someString = "HelloWorld";

            //Act
            var config = _configManager.GetConfiguration<TestConfig>(configName);
            config.DummyString = someString;
            _configManager.SaveConfiguration(config, configName);

            //Assert
            var configPath = Path.Combine(_fullConfigDir, configName + ConfigConstants.FileExtension);
            Assert.IsTrue(File.Exists(configPath), "Config file was not created");

            var reloaded = _configManager.GetConfiguration<TestConfig>(configName);
            Assert.AreEqual(someString, reloaded.DummyString, "Config file was not loaded correctly");
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(_fullConfigDir, true);
        }
    }
}