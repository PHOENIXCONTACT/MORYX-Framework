// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Configuration;
using NUnit.Framework;

namespace Moryx.Tests.Configuration.ValueProvider
{
    [TestFixture]
    public class ValueProviderTests
    {
        [Test]
        public void SetDefaultsOnlyOnWritableProperties()
        {
            // Arrange
            var config = new TestConfig1();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number));
            Assert.That(config.DummyNumberReadOnly, Is.EqualTo(1024));
            Assert.That(config.DummyNumber2, Is.EqualTo(25));
            Assert.That(config.DummyText, Is.EqualTo(DefaultValues.Text));
        }

        [Test]
        public void SetDefaultsOnlyPropertiesWithDefaultValueAttribute()
        {
            // Arrange
            var config = new TestConfig1();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            //Assert
            Assert.That(config.DummyNumber2, Is.EqualTo(25));
        }

        [Test]
        public void UseNoValueProviderLeadsToNoChanges()
        {
            // Arrange
            var config = new TestConfig1();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings());

            // Assert
            Assert.That(config.DummyNumber, Is.EqualTo(0));
            Assert.That(config.DummyNumberReadOnly, Is.EqualTo(1024));
            Assert.That(config.DummyNumber2, Is.EqualTo(25));
            Assert.That(config.DummyText, Is.Null);
        }

        [Test]
        public void SetDefaultsOnComplexConfig()
        {
            // Arrange
            var config = new TestConfig2();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            Assert.That(config.Config, Is.Not.Null);
            Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number));
            Assert.That(config.Config.DummyNumber, Is.EqualTo(DefaultValues.Number));
            Assert.That(config.Config.DummyNumberReadOnly, Is.EqualTo(1024));
            Assert.That(config.Config.DummyText, Is.EqualTo(DefaultValues.Text));
        }

        [Test]
        public void AdditionalFilterTest()
        {
            // Arrange
            var config = new TestConfig2();

            // Act
            ValueProviderExecutor.Execute(config,
                new ValueProviderExecutorSettings().AddDefaultValueProvider()
                    .AddFilter(new NoStringValueProviderFilter()));

            // Assert
            Assert.That(config.Config, Is.Not.Null);
            Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number));
            Assert.That(config.Config.DummyNumber, Is.EqualTo(DefaultValues.Number));
            Assert.That(config.Config.DummyNumberReadOnly, Is.EqualTo(1024));
            Assert.That(config.Config.DummyText, Is.Null);
        }

        [Test]
        public void EnumerableTest()
        {
            // Arrange
            var config = new TestConfig3 { Configs = [new TestConfig1(), new TestConfig1(), new TestConfig1()] };

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            foreach (var subConfig in config.Configs)
            {
                Assert.That(subConfig.DummyNumber, Is.EqualTo(DefaultValues.Number));
                Assert.That(subConfig.DummyNumberReadOnly, Is.EqualTo(1024));
                Assert.That(subConfig.DummyNumber2, Is.EqualTo(25));
                Assert.That(subConfig.DummyText, Is.EqualTo(DefaultValues.Text));
            }

            Assert.That(config.DummyNumber, Is.EqualTo(DefaultValues.Number));
        }

        [Test]
        public void ListOfPrimitivesTest()
        {
            // Arrange
            var config = new TestConfig4();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            Assert.That(config.Numbers, Is.Not.Null);
            Assert.That(config.Strings, Is.Not.Null);
            Assert.That(config.ArrayNumbers, Is.Not.Null);
            Assert.That(config.EnumerableNumbers, Is.Null);
        }

        [Test]
        public void PropertyWithoutDefaultCtorDoesNotThrow()
        {
            // Arrange
            var config = new TestConfig5();
            var settings = new ValueProviderExecutorSettings().AddDefaultValueProvider();
            
            // Act
            // Assert
            Assert.DoesNotThrow(() => ValueProviderExecutor.Execute(config, settings));
        }

        [Test]
        public void NullableValueTypesHandledCorrectly()
        {
            var config = new TestConfig6();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider().AddProvider(new ThreeProvider()));

            Assert.That(config.WithDefaultValue, Is.EqualTo(5), "Not null default value was not applied");
            Assert.That(config.WithDefaultValueNull, Is.Null, "Did not respect default value null");
            Assert.That(config.WithoutDefaultValue, Is.EqualTo(3), "DefaultValueProvider did handle the field without a DefaultValue Attribute");
        }
    }
}
