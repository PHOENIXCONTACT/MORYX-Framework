// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moryx.Configuration;
using NUnit.Framework;
using static Moryx.Runtime.Kernel.Tests.Configuration.DefaultValues;

namespace Moryx.Runtime.Kernel.Tests.Configuration;

[TestFixture]
public class ConfigManagerWithConfigProviderTests
{
    private string _tempDir;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(
            Path.GetTempPath(),
            "ConfigManagerTests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void Cleanup()
    {

        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    private ConfigManager CreateManager(IConfiguration configuration)
    {
        // Create manager through DI to be as consistent with real world use as possible
        var serviceCollection = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddLogging();

        serviceCollection.AddMoryxKernel(true);

        var provider = serviceCollection.BuildServiceProvider();

        var manager = provider.GetRequiredService<ConfigManager>();
        manager.ConfigDirectory = _tempDir;

        return manager;
    }

    [Test(Description = "Resolves an explicit configuration key placeholder from IConfiguration.")]
    public void ExplicitConfigurationKeyIsResolvedFromConfiguration()
    {
        // Arrange

        var json = """
        {
            "NormalSetting": "from-json",
        }
        """;
        File.WriteAllText(Path.Combine(_tempDir, "SecretConfig.json"), json);

        var configuration = MicrosoftExtensionConfigProviderTests.BuildConfig(new()
        {
            ["Secrets:ApiKey"] = "from-provider",
        });

        var manager = CreateManager(configuration);

        // Act

        var config = manager.GetConfiguration<SecretConfigWithAttribute>(
            name: "SecretConfig",
            getCopy: true);

        // Assert

        Assert.That(config.NormalSetting, Is.EqualTo("from-json"));
        Assert.That(config.ServiceApiKey, Is.EqualTo("from-provider"));
    }

    [Test(Description = "Makes sure injected secrets are not stored to disk (as long as they are marked with IgnoreDataMemberAttribute)")]
    public void SavingConfigurationDoesNotWriteSecretToFile()
    {
        // Arrange

        var json = """
        {
            "NormalSetting": "from-json",
        }
        """;
        var configPath = Path.Combine(_tempDir, "SecretConfig.json");
        File.WriteAllText(configPath, json);

        var configuration = MicrosoftExtensionConfigProviderTests.BuildConfig(new()
        {
            ["Secrets:ApiKey"] = "from-provider"
        });

        var manager = CreateManager(configuration);

        var config = manager.GetConfiguration<SecretConfigWithAttribute>(
            name: "SecretConfig",
            getCopy: false);

        // Act

        manager.SaveConfiguration(config, name: "SecretConfig");
        var writtenConfig = File.ReadAllText(configPath);

        // Assert

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.ServiceApiKey, Is.EqualTo("from-provider"));
        Assert.That(writtenConfig, Does.Not.Contain("from-provider"));
    }

    [Test(Description = "Applies provider values to generated configs when no JSON file exists.")]
    public void ReceivesValuesFromConfiguration()
    {
        // Arrange

        var configuration = MicrosoftExtensionConfigProviderTests.BuildConfig(new()
        {
            ["Secrets:ApiKey"] = "from-provider"
        });

        var manager = CreateManager(configuration);

        // Act

        var config = manager.GetConfiguration<SecretConfigWithAttribute>(
            name: "SecretConfig",
            getCopy: true);

        // Assert

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.ServiceApiKey, Is.EqualTo("from-provider"));
        Assert.That(config.ConfigState, Is.EqualTo(ConfigState.Generated));
    }

    [Test(Description = "Configurable config key is supported through second field")]
    public void CanReceiveFromConfigurableConfigKey()
    {
        // Arrange

        const string configKey = "TotallyArbitrary:Secret:Key";
        const string expected = "blablabalablabla";

        var json = $$"""
        {
            "PasswordConfigKey": "{{configKey}}"
        }
        """;
        var configPath = Path.Combine(_tempDir, "SecretConfig.json");
        File.WriteAllText(configPath, json);

        var configDict = new Dictionary<string, string?>
        {
            [configKey] = expected,
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        var manager = CreateManager(configuration);

        // Act

        var config = manager.GetConfiguration<SecretConfigWithOtherValueTypes>(
            getCopy: true,
            name: "SecretConfig"
        );

        // Assert

        Assert.That(config.Password, Is.EqualTo(expected));
    }

    [Test(Description = "Lists are prepopulated by configuration values")]
    public void ListArePrepopulatedByConfigurationValues()
    {
        // Arrange

        double expected = 0.5;

        var moryxConfig = $$"""
        {
        }
        """;
        var configPath = Path.Combine(_tempDir, "ExampleConfig.json");
        File.WriteAllText(configPath, moryxConfig);

        var appsettings = $$"""
        {
            "ListConfig:Content": [
                null,
                {
                    "DummyDouble": {{expected.ToString(CultureInfo.InvariantCulture)}}
                }
            ]
        }
        """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(appsettings));

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        var manager = CreateManager(configuration);

        // Act

        var config = manager.GetConfiguration<ListConfig>(
            getCopy: true,
            name: "ExampleConfig"
        );

        // Assert

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.Content.Count, Is.EqualTo(2));
        Assert.That(config.Content[0].DummyDouble, Is.EqualTo(DefaultValues.Decimal));
        Assert.That(config.Content[1].DummyDouble, Is.EqualTo(expected));
    }

    [Test(Description = """
    This test only serves to visualize the current behavior. 
    If this test fails, you can remove the matching "Caution" section in 'docs\articles\framework\configuration.md' "Nested Properties and Collections"
    that no new entries can be added to a list from ConfigProviders.
    """)]
    public void ListsAreNotExtendedFromConfigProviders()
    {
        // Arrange

        double expected = 0.5;

        var json = $$"""
        {
            Content: [
                {
                    "DummyDouble": {{expected.ToString(CultureInfo.InvariantCulture)}}
                }
            ]
        }
        """;
        var configPath = Path.Combine(_tempDir, "ExampleConfig.json");
        File.WriteAllText(configPath, json);

        var configDict = new Dictionary<string, string?>
        {
            ["ListConfig:Content:1:DummyDouble"] = expected.ToString(CultureInfo.InvariantCulture),
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        var manager = CreateManager(configuration);

        // Act

        var config = manager.GetConfiguration<ListConfig>(
            getCopy: true,
            name: "ExampleConfig"
        );

        // Assert

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.Content.Count, Is.EqualTo(1));
        Assert.That(config.Content[0].DummyDouble, Is.EqualTo(expected));
    }

    [Test(Description = """
    This test only serves to visualize the current behavior. 
    If this test fails, you can remove the matching "Caution" section in 'docs\articles\framework\configuration.md' "Nested Properties and Collections"
    that not configured list entries are ignored and the indices in the resulting List don't necessarily match the once you specified.
    """)]
    public void NotConfiguredIndicesInListsAreIgnored()
    {
        // Arrange

        double expected = 0.5;

        var json = $$"""
        {
        }
        """;
        var configPath = Path.Combine(_tempDir, "ExampleConfig.json");
        File.WriteAllText(configPath, json);

        var configDict = new Dictionary<string, string?>
        {
            ["ListConfig:Content:1:DummyDouble"] = expected.ToString(CultureInfo.InvariantCulture),
            ["ListConfig:Content:3:DummyDouble"] = 0.3.ToString(CultureInfo.InvariantCulture),
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        var manager = CreateManager(configuration);

        // Act

        var config = manager.GetConfiguration<ListConfig>(
            getCopy: true,
            name: "ExampleConfig"
        );

        // Assert

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.Content.Count, Is.EqualTo(2));
        Assert.That(config.Content[0].DummyDouble, Is.EqualTo(expected));
    }
}
