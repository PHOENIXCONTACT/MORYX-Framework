// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;
using Moryx.Configuration;
using NUnit.Framework;

namespace Moryx.Runtime.Kernel.Tests.Configuration;

[TestFixture]
public class ConfigManagerTests
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

    [Test]
    public void InitialLoad()
    {
        // Load file from empty config an check defaults
        var config = _configManager.GetConfiguration<TestConfig>();

        // Check if all properies and subclasses are filled
        Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number), "Integer value was not initialized");
        Assert.That(config.DummyShort, Is.EqualTo(DefaultValues.Number), "UShort value was not initialized");
        Assert.That(config.DummyString, Is.EqualTo(DefaultValues.Text), "String was not initialized");
        Assert.That(config.Child, Is.Not.Null, "Child property was not initialized with class instance");
        Assert.That(config.Child.DummyDouble, Is.EqualTo(DefaultValues.Decimal), "Double was not initialized");
    }

    [Test]
    public void FileCreated()
    {
        var config = _configManager.GetConfiguration<TestConfig>();
        _configManager.SaveConfiguration(config);

        Assert.That(Directory.GetFiles(_fullConfigDir).Any(), Is.True, "No config file was created!");
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

        Assert.That(reloadConfig.DummyNumber, Is.EqualTo(config.DummyNumber), "Number change was not saved");
        Assert.That(reloadConfig.DummyString, Is.EqualTo(config.DummyString), "Text change was not saved");
        Assert.That(reloadConfig.Child.DummyDouble, Is.EqualTo(config.Child.DummyDouble), "Decimal was not saved");
    }

    [Test]
    public void DefaultRestore()
    {
        var config = _configManager.GetConfiguration<TestConfig>();
        config.DummyNumber = 0;
        _configManager.SaveConfiguration(config);

        config = _configManager.GetConfiguration<TestConfig>(true);
        Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number), "Default not restored");
    }

    [Test]
    public void ConfigSelfRepair()
    {
        const string configName = "Moryx.Tests.Configuration.TestConfig";

        // Write faulty config to file
        var fileName = Path.Combine(_fullConfigDir, configName + ConfigManager.FileExtension);
        File.WriteAllText(fileName, FaultyConfig.Content());

        // Load config an check if present values where preserved
        var config = _configManager.GetConfiguration<TestConfig>(configName);
        Assert.That(config.DummyString, Is.EqualTo(ModifiedValues.Text), "Modified value not recovered!");
        Assert.That(config.Child.DummyDouble, Is.EqualTo(ModifiedValues.Decimal), "Decimal value not preserved!");
        Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number), "Default value not restored");
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
        var config1 = _configManager.GetConfiguration<TestConfig>(true);
        var config2 = _configManager.GetConfiguration<TestConfig>(false);

        config1.DummyNumber++;

        Assert.That(config1.DummyNumber, Is.Not.EqualTo(config2.DummyNumber));
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
        var configPath = Path.Combine(_fullConfigDir, configName + ConfigManager.FileExtension);
        Assert.That(File.Exists(configPath), "Config file was not created");

        var reloaded = _configManager.GetConfiguration<TestConfig>(configName);
        Assert.That(reloaded.DummyString, Is.EqualTo(someString), "Config file was not loaded correctly");
    }

    [TearDown]
    public void Cleanup()
    {
        Directory.Delete(_fullConfigDir, true);
    }
}