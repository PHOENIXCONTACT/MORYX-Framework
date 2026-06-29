// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
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
        manager.SaveConfiguration(config, name: "SecretConfig");
        var writtenConfig = File.ReadAllText(configPath);

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.ServiceApiKey, Is.EqualTo("from-provider"));
        Assert.That(writtenConfig, Does.Not.Contain("from-provider"));
    }

    [Test(Description = "Applies provider values to generated configs when no JSON file exists.")]
    public void ReceivesValuesFromConfiguration()
    {
        var configuration = MicrosoftExtensionConfigProviderTests.BuildConfig(new()
        {
            ["Secrets:ApiKey"] = "from-provider"
        });

        var manager = CreateManager(configuration);

        var config = manager.GetConfiguration<SecretConfigWithAttribute>(
            name: "SecretConfig",
            getCopy: true);

        using var _ = Assert.EnterMultipleScope();
        Assert.That(config.ServiceApiKey, Is.EqualTo("from-provider"));
        Assert.That(config.ConfigState, Is.EqualTo(ConfigState.Generated));
    }

    [Test(Description = "Configurable config key is supported through second field")]
    public void CanReceiveFromConfigurableConfigKey()
    {
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

        var config = manager.GetConfiguration<SecretConfigWithOtherValueTypes>(
            getCopy: true,
            name: "SecretConfig"
        );

        Assert.That(config.Password, Is.EqualTo(expected));
    }
}
