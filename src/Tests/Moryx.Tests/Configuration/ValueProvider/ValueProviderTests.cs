// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using NUnit.Framework;

namespace Marvin.Tests.Configuration.ValueProvider
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
            Assert.AreEqual(DefaultValues.Number, config.DummyNumber);
            Assert.AreEqual(1024, config.DummyNumberReadOnly);
            Assert.AreEqual(25, config.DummyNumber2);
            Assert.AreEqual(DefaultValues.Text, config.DummyText);
        }

        [Test]
        public void SetDefaultsOnlyPropertiesWithDefaultValueAttribute()
        {
            // Arrange
            var config = new TestConfig1();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            //Assert
            Assert.AreEqual(25, config.DummyNumber2);
        }

        [Test]
        public void UseNoValueProviderLeadsToNoChanges()
        {
            // Arrange
            var config = new TestConfig1();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings());

            // Assert
            Assert.AreEqual(0, config.DummyNumber);
            Assert.AreEqual(1024, config.DummyNumberReadOnly);
            Assert.AreEqual(25, config.DummyNumber2);
            Assert.IsNull(config.DummyText);
        }

        [Test]
        public void SetDefaultsOnComplexConfig()
        {
            // Arrange
            var config = new TestConfig2();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            Assert.IsNotNull(config.Config);
            Assert.AreEqual(DefaultValues.Number, config.DummyNumber);
            Assert.AreEqual(DefaultValues.Number, config.Config.DummyNumber);
            Assert.AreEqual(1024, config.Config.DummyNumberReadOnly);
            Assert.AreEqual(DefaultValues.Text, config.Config.DummyText);
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
            Assert.IsNotNull(config.Config);
            Assert.AreEqual(DefaultValues.Number, config.DummyNumber);
            Assert.AreEqual(DefaultValues.Number, config.Config.DummyNumber);
            Assert.AreEqual(1024, config.Config.DummyNumberReadOnly);
            Assert.IsNull(config.Config.DummyText);
        }

        [Test]
        public void EnumerableTest()
        {
            // Arrange
            var config = new TestConfig3 { Configs = new List<TestConfig1> { new TestConfig1(), new TestConfig1(), new TestConfig1() } };

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            foreach (var subConfig in config.Configs)
            {
                Assert.AreEqual(DefaultValues.Number, subConfig.DummyNumber);
                Assert.AreEqual(1024, subConfig.DummyNumberReadOnly);
                Assert.AreEqual(25, subConfig.DummyNumber2);
                Assert.AreEqual(DefaultValues.Text, subConfig.DummyText);
            }

            Assert.AreEqual(DefaultValues.Number, config.DummyNumber);
        }

        [Test]
        public void ListOfPrimitivesTest()
        {
            // Arrange
            var config = new TestConfig4();

            // Act
            ValueProviderExecutor.Execute(config, new ValueProviderExecutorSettings().AddDefaultValueProvider());

            // Assert
            Assert.NotNull(config.Numbers);
            Assert.NotNull(config.Strings);
            Assert.NotNull(config.ArrayNumbers);
            Assert.IsNull(config.EnumerableNumbers);
        }
    }
}
