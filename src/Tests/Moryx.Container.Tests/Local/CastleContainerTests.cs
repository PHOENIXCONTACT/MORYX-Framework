// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container.TestTools;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Container.Tests;

[TestFixture]
public class CastleContainerTests
{
    private CastleContainer _container;

    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        ReflectionTool.TestMode = true;
        AppDomainBuilder.LoadAssemblies();
    }

    [SetUp]
    public void Init()
    {
        _container = new CastleContainer(new Dictionary<Type, string>());
        _container.LoadFromAssembly(GetType().Assembly);
    }

    [Test]
    public void LocalsFound()
    {
        var localComponent = _container.Resolve<ILocalComponent>();
        var localFactory = _container.Resolve<ILocalFactory>();

        Assert.That(localComponent, Is.Not.Null, "Failed to resolve local component!");
        Assert.That(localFactory, Is.Not.Null, "Failed to resolve local factory");
    }

    [Test]
    public void DefaultFactory()
    {
        var factory = _container.Resolve<ILocalFactory>();
        var comp = factory.Create();

        Assert.That(comp.GetType(), Is.EqualTo(typeof(LocalComponent)), "Factory not working!");
    }

    [Test]
    public void NameBasedFactory()
    {
        var factory = _container.Resolve<INamedComponentFactory>();
        var componentA = factory.Create(NamedComponentA.ComponentName);
        var componentB = factory.Create(NamedComponentB.ComponentName);

        Assert.That(componentA.GetName(), Is.EqualTo(NamedComponentA.ComponentName), "Component A not resolved by name!");
        Assert.That(componentB.GetName(), Is.EqualTo(NamedComponentB.ComponentName), "Component B not resolved by name!");
    }

    [TestCase(null, Description = "NameBasedComponentSelector throws ArgumentException when null is passed as name")]
    [TestCase("", Description = "NameBasedComponentSelector throws ArgumentException when empty string is passed as name")]
    public void NameBasedComponentSelectorThrowsOnNullOrEmpty(string name)
    {
        // Arrange
        var factory = _container.Resolve<INamedComponentFactory>();

        // Act & Assert
        Assert.That(() => factory.Create(name), Throws.InstanceOf<ArgumentException>());
    }

    [TestCase(null, Description = "ConfigBasedComponentSelector throws ArgumentException when PluginName is null")]
    [TestCase("", Description = "ConfigBasedComponentSelector throws ArgumentException when PluginName is empty")]
    public void ConfigBasedComponentSelectorThrowsOnNullOrEmptyPluginName(string pluginName)
    {
        // Arrange
        _container.LoadComponents<IRootClass>();
        _container.Register<IRootClassFactory>();
        var factory = _container.Resolve<IRootClassFactory>();
        var config = new RootClassFactoryConfig { PluginName = pluginName };

        // Act & Assert
        Assert.That(() => factory.Create(config), Throws.InstanceOf<ArgumentException>());
    }

    [TearDown]
    public void Destroy()
    {
        _container.Destroy();
    }
}
