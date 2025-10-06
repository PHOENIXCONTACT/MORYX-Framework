// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Container.TestPlugin;
using Moryx.Container.TestRootPlugin;
using Moryx.Container.TestTools;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Container.Tests
{
    [TestFixture]
    public class StrategySelectionTest
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            AppDomainBuilder.LoadAssemblies();
        }

        [SetUp]
        public void SetDirectory()
        {
            ReflectionTool.TestMode = true;
        }

        [Test]
        public void DefaultInjection()
        {
            // Arrange
            var container = CreateContainer(new Dictionary<Type, string>());

            // Act
            var root = container.Resolve<IRootClass>();

            // Assert
            Assert.That(root, Is.Not.Null, "Root not resolved.");
            Assert.That(root.ConfiguredComponent, Is.Not.Null, "root.ConfiguredComponent not resolved.");
        }

        [Test]
        public void ResolveWithOverride()
        {
            // Arrange
            var container = CreateContainer(new Dictionary<Type, string>
            {
                [typeof(IConfiguredComponent)] = ConfiguredComponentB.PluginName
            });

            // Act
            var component = container.Resolve<IConfiguredComponent>();

            // Assert
            Assert.That(component.GetName(), Is.EqualTo(ConfiguredComponentB.PluginName));
        }

        [Test]
        public void InjectionOverride()
        {
            // Arrange
            var container = CreateContainer(new Dictionary<Type, string>
            {
                { typeof(IConfiguredComponent), ConfiguredComponentB.PluginName}
            });

            // Act
            var root = container.Resolve<IRootClass>();

            // Assert
            Assert.That(root, Is.Not.Null, "Root not resolved.");
            Assert.That(root.ConfiguredComponent, Is.Not.Null, "root.ConfiguredComponent not resolved.");
            Assert.That(root.ConfiguredComponent.GetName(), Is.EqualTo(ConfiguredComponentB.PluginName), "Strategy was not overridden!");
        }

        [Test]
        public void DefaultWithInjectionOverride()
        {
            // Arrange
            var container = CreateContainer(new Dictionary<Type, string>
            {
                { typeof(IConfiguredComponent), ConfiguredComponentA.PluginName }
            });

            // Act
            var root = container.Resolve<IRootClass>();

            // Assert
            Assert.That(root, Is.Not.Null, "Root not resolved.");
            Assert.That(root.ConfiguredComponent, Is.Not.Null, "root.ConfiguredComponent not resolved.");
            Assert.That(root.ConfiguredComponent.GetName(), Is.EqualTo(ConfiguredComponentA.PluginName), "Strategy was not overridden!");
        }

        [TestCase(RootClass.PluginName, ConfiguredComponentA.PluginName)]
        [TestCase(RootClass.PluginName, ConfiguredComponentB.PluginName)]
        [TestCase(RootClass.PluginName, ForeignComponent.PluginName)]
        [TestCase(RootClass.PluginName, ForeignDefaultComponent.PluginName)]
        [TestCase(ForeignRootClass.PluginName, ConfiguredComponentA.PluginName)]
        [TestCase(ForeignRootClass.PluginName, ConfiguredComponentB.PluginName)]
        [TestCase(ForeignRootClass.PluginName, ForeignComponent.PluginName)]
        [TestCase(ForeignRootClass.PluginName, ForeignDefaultComponent.PluginName)]
        public void OverrideWithFactory(string rootName, string pluginName)
        {
            // Arrange
            var container = CreateContainer(new Dictionary<Type, string>
            {
                { typeof(IConfiguredComponent), pluginName }
            });

            // Act
            var rcFac = container.Resolve<IRootClassFactory>();
            var root = rcFac.Create(new RootClassFactoryConfig
            {
                PluginName = rootName
            });

            // Assert
            Assert.That(root.GetName(), Is.EqualTo(rootName), "Strategy was not overridden!");
            Assert.That(root.ConfiguredComponent.GetName(), Is.EqualTo(pluginName), "Strategy was not overridden!");
        }

        private static IContainer CreateContainer(IDictionary<Type, string> strategies)
        {
            var container = new CastleContainer(strategies);

            container.LoadComponents<IRootClass>();
            container.LoadComponents<IConfiguredComponent>();

            container.Register<IRootClassFactory>();

            return container;
        }
    }
}
