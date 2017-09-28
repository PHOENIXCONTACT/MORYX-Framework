using System;
using System.Collections.Generic;
using Marvin.Container.TestPlugin;
using Marvin.Container.TestRootPlugin;
using Marvin.Container.TestTools;
using Marvin.Tools;
using NUnit.Framework;

namespace Marvin.Container.Tests
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
            var container = CreateContainer(null);

            // Act
            var root = container.Resolve<IRootClass>();

            // Assert
            Assert.NotNull(root, "Root not resolved.");
            Assert.NotNull(root.ConfiguredComponent, "root.ConfiguredComponent not resolved.");
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
            Assert.AreEqual(ConfiguredComponentB.PluginName, component.GetName());
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
            Assert.NotNull(root, "Root not resolved.");
            Assert.NotNull(root.ConfiguredComponent, "root.ConfiguredComponent not resolved.");
            Assert.AreEqual(ConfiguredComponentB.PluginName, root.ConfiguredComponent.GetName(), "Strategy was not overridden!");
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
            Assert.NotNull(root, "Root not resolved.");
            Assert.NotNull(root.ConfiguredComponent, "root.ConfiguredComponent not resolved.");
            Assert.AreEqual(ConfiguredComponentA.PluginName, root.ConfiguredComponent.GetName(), "Strategy was not overridden!");
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
            Assert.AreEqual(rootName, root.GetName(), "Strategy was not overridden!");
            Assert.AreEqual(pluginName, root.ConfiguredComponent.GetName(), "Strategy was not overridden!");
        }

        private static IContainer CreateContainer(IDictionary<Type, string> config)
        {
            var container = new ServerLocalContainer(config);

            container.LoadComponents<IRootClass>();
            container.LoadComponents<IConfiguredComponent>();

            container.Register<IRootClassFactory>();

            return container;
        }
    }
}