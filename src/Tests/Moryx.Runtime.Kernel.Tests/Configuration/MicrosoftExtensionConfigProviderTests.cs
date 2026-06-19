// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Configuration;
using Moryx.Runtime.Kernel.Configuration;
using NUnit.Framework;
using static Moryx.Runtime.Kernel.Tests.Configuration.DefaultValues;

namespace Moryx.Runtime.Kernel.Tests.Configuration;

public class MicrosoftExtensionConfigProviderTests
{
    [Test]
    public void PropertiesThatAlreadyContainAValueAreIgnored()
    {
        // Arrange
        var testConfig = new TestConfig()
        {
            DummyNumber = 1,
            DummyShort = 2,
            DummyString = "Blabla",
            Child = new()
        };

        var provider = BuildProvider(new()
        {
            ["Config:DummyNumber"] = "99",
            ["Config:DummyShort"] = "99",
            ["Config:DummyString"] = "99",
            ["Config:Child:DummyDouble"] = "5.5",
        });

        // Act

        provider.Handle(new([GetStackFrame(testConfig, nameof(TestConfig.DummyNumber))]));
        provider.Handle(new([GetStackFrame(testConfig, nameof(TestConfig.DummyShort))]));
        provider.Handle(new([GetStackFrame(testConfig, nameof(TestConfig.DummyString))]));
        provider.Handle(new([GetStackFrame(testConfig, nameof(TestConfig.Child))]));

        // Assert

        using var _ = Assert.EnterMultipleScope();
        Assert.That(testConfig.DummyNumber, Is.EqualTo(1));
        Assert.That(testConfig.DummyShort, Is.EqualTo(2));
        Assert.That(testConfig.DummyString, Is.EqualTo("Blabla"));
        Assert.That(testConfig.Child.DummyDouble, Is.EqualTo(0.0));

    }

    [TestCase(typeof(SecretConfig), "Moryx.Runtime.Kernel.Tests.Configuration.DefaultValues+SecretConfig")] // Prefix defaults to the fully qualified type name
    [TestCase(typeof(SecretConfigWithAttribute), "Secrets")] // The config with the InjectedConfigAttribute overwrites the default
    public void PrefixCanBeChangedByAttribute(Type configType, string expectedPrefix)
    {
        // Arrange
        const string Expected = "provided";
        var provider = BuildProvider(
            new Dictionary<string, string?>
            {
                [$"{expectedPrefix}:ApiKey"] = Expected
            }
        );

        var testConfig = (SecretConfig)Activator.CreateInstance(configType);
        var propertyName = nameof(SecretConfig.ServiceApiKey);
        var property = testConfig.GetType().GetProperty(propertyName);

        // Act
        var result = provider.Handle(testConfig, property);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.ServiceApiKey, Is.EqualTo(Expected));
    }

    [TestCase(0)]
    [TestCase(1)]
    public void IntValueCanBeSet(int seed)
    {
        var number = new Random(seed).Next();
        var provider = BuildProvider(
            new Dictionary<string, string?>
            {
                ["Secrets:SecretPin"] = number.ToString(CultureInfo.InvariantCulture)
            }
        );
        var testConfig = new SecretConfigWithOtherValueTypes();

        var result = provider.Handle(testConfig, testConfig.GetType().GetProperty(nameof(SecretConfigWithOtherValueTypes.SecretPin)));

        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.SecretPin, Is.EqualTo(number));
    }

    [Test]
    public void NestedConfigIgnoresClassLevelAttribute()
    {
        // Arrange
        const string Expected = "provided";
        const string Unexpected = "Should not be provided";

        var provider = BuildProvider(
            new Dictionary<string, string?>
            {
                ["Config:Nested:ConfigValue"] = Expected, // The nesting follows the property names correctly
                ["Config:ConfigValue"] = Unexpected // Nested is the config key from the InnerNestedSecretConfig and should not be used
            }
        );

        var testConfig = new ExampleConfig() { Nested = new() };

        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(testConfig, nameof(ExampleConfig.Nested)),
            GetStackFrame(testConfig.Nested, nameof(ExampleConfig.ConfigValue))
        ]);

        // Act
        var result = provider.Handle(stack);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.Nested.ConfigValue, Is.EqualTo(Expected));

    }

    [TestCase("Secrets:Pwd", ".:Pwd")]
    [TestCase("Secrets:Pwd", "Secrets:Pwd")]
    [TestCase("Pwd", "Pwd")]
    public void ConfigKeyCanBeConfiguredThroughASeparateProperty(string configKey, string propertyValue)
    {
        // Arrange
        const string Expected = "provided";
        var provider = BuildProvider(
            new()
            {
                [configKey] = Expected,
            }
        );

        var testConfig = new SecretConfigWithOtherValueTypes() { PasswordConfigKey = configKey };

        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(testConfig, nameof(SecretConfigWithOtherValueTypes.Password)),
        ]);

        // Act
        var result = provider.Handle(stack);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.Password, Is.EqualTo(Expected));
    }

    [TestCase("Secrets:ApiKey", "Secrets")]
    [TestCase("Other:ApiKey", "Other")]
    public void ConfigPrefixCanBeConfiguredThroughASeparateProperty(string configKey, string propertyValue)
    {
        // Arrange
        const string Expected = "provided";
        var provider = BuildProvider(
            new Dictionary<string, string?>
            {
                [configKey] = Expected,
            }
        );

        var testConfig = new ExampleConfigWithConfiguredPrefix() { Prefix = propertyValue };

        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(testConfig, nameof(ExampleConfigWithConfiguredPrefix.ConfigWithFixedRelativeKey)),
        ]);

        // Act
        var result = provider.Handle(stack);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.ConfigWithFixedRelativeKey, Is.EqualTo(Expected));
    }

    [Test]
    public void CanFillListsOfBasicTypes()
    {
        // Arrange
        List<int> Expected = [4, 2];
        var provider = BuildProvider(
            new Dictionary<string, string?>
            {
                ["Config:Basic:0"] = "4",
                ["Config:Basic:1"] = "2",
            }
        );

        var testConfig = new ExampleConfig();

        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(testConfig, nameof(ExampleConfig.Basic)),
        ]);

        // Act
        var result = provider.Handle(stack);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.Basic, Is.EqualTo(Expected));
    }

    [Test]
    public void CanFillListsOfComplexTypes()
    {
        // Arrange
        List<string> Expected = ["4", "2"];
        var provider = BuildProvider(new()
        {
            ["Config:Objects:0:ConfigValue"] = "4",
            ["Config:Objects:1:ConfigValue"] = "2",
        }
        );
        var testConfig = new ExampleConfig();

        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(testConfig, nameof(ExampleConfig.Objects)),
        ]);

        // Act
        var result = provider.Handle(stack);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.Objects.Select(o => o.ConfigValue).ToList(), Is.EqualTo(Expected));
    }

    [Test]
    public void CanFillNestedValuesWithEnumerableInHistory()
    {
        // Arrange
        const string Expected = "4";
        var provider = BuildProvider(
            new Dictionary<string, string?>
            {
                ["Config:Objects:1:ConfigValue"] = Expected,
            }
        );

        var testConfig = new ExampleConfig()
        {
            Objects = [
                new(), new()
            ]
        };

        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(testConfig, nameof(ExampleConfig.Objects)),
            new ExecutorLevel(testConfig.Objects, null, new Dictionary<string, object>() { ["Index"] = 1 }),
            GetStackFrame(testConfig.Objects[0], nameof(ExampleConfig.ConfigValue)),
        ]);

        // Act
        var result = provider.Handle(stack);

        // Assert
        using var _ = Assert.EnterMultipleScope();
        Assert.That(result, Is.EqualTo(ValueProviderResult.Handled));
        Assert.That(testConfig.Objects[0].ConfigValue, Is.EqualTo(Expected));
    }

    [TestCase(null)]
    [TestCase("")]
    public void InvalidConfigKeyFallsBackToTheDefault(string configValue)
    {
        const string expected = "test";
        var provider = BuildProvider(new()
        {
            ["Secrets:Password"] = expected
        });

        var config = new SecretConfigWithOtherValueTypes()
        {
            PasswordConfigKey = configValue,
        };
        var stack = new Stack<ExecutorLevel>([
            GetStackFrame(config, nameof(SecretConfigWithOtherValueTypes.Password)),
        ]);

        // Act
        var _ = provider.Handle(stack);

        // Assert
        Assert.That(config.Password, Is.EqualTo(expected));
    }

    internal static IConfigurationRoot BuildConfig(Dictionary<string, string> configDict)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();
        return configuration;
    }
    private static MicrosoftExtensionsConfigProvider BuildProvider(Dictionary<string, string> configDict)
    {
        var configuration = BuildConfig(configDict);
        return new MicrosoftExtensionsConfigProvider(configuration, new NullLogger<MicrosoftExtensionsConfigProvider>());
    }

    private static ExecutorLevel GetStackFrame(object config, string name)
    {
        return new ExecutorLevel(config, config.GetType().GetProperty(name), new());
    }

    [ProvidedConfig("Config")]
    public class ExampleConfig
    {
        public int ExampleInt { get; set; }
        public string ConfigValue { get; set; }

        public List<int> Basic { get; set; }

        public List<ExampleConfig> Objects { get; set; }
        public ExampleConfig Nested { get; set; }

        public string PasswordConfigKey { get; set; }

        [ProvidedConfig(nameof(PasswordConfigKey), true)]
        public string PasswordWithConfigurableKey { get; set; }

        [ProvidedConfig(".:ApiKey")]
        public string ConfigWithFixedRelativeKey { get; set; }

        [ProvidedConfig("Fixed:Key")]
        public string ConfigWithFixedAbsoluteKey { get; set; }
    }

    [ProvidedConfig(nameof(Prefix), fromProperty: true)]
    public class ExampleConfigWithConfiguredPrefix : ExampleConfig
    {
        [DataMember]
        public string Prefix { get; set; }
    }

}
