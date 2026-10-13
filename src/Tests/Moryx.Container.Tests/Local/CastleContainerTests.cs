// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container.TestTools;
using NUnit.Framework;

namespace Moryx.Container.Tests;

[TestFixture]
public class CastleContainerTests
{
    private CastleContainer _container;

    [SetUp]
    public void Init()
    {
        _container = new CastleContainer(new Dictionary<Type, string>());
        _container.LoadFromAssembly(GetType().Assembly);
    }

    [Test]
    public void LocalsFound()
    {
        var local = _container.Resolve<ILocalComponent>();
        var localFac = _container.Resolve<ILocalFactory>();

        Assert.That(local, Is.Not.Null, "Failed to resolve local component!");
        Assert.That(localFac, Is.Not.Null, "Failed to resolve local factory");
    }

    [Test]
    public void DefaultFactory()
    {
        var fac = _container.Resolve<ILocalFactory>();
        var comp = fac.Create();

        Assert.That(comp.GetType(), Is.EqualTo(typeof(LocalComponent)), "Factory not working!");
    }

    [Test]
    public void NameBasedFactory()
    {
        var fac = _container.Resolve<INamedComponentFactory>();
        var compA = fac.Create(NamedComponentA.ComponentName);
        var compB = fac.Create(NamedComponentB.ComponentName);

        Assert.That(compA.GetName(), Is.EqualTo(NamedComponentA.ComponentName), "Component A not resolved by name!");
        Assert.That(compB.GetName(), Is.EqualTo(NamedComponentB.ComponentName), "Component B not resolved by name!");
    }

    [TestCase(null, Description = "NameBasedComponentSelector throws ArgumentException when null is passed as name")]
    [TestCase("", Description = "NameBasedComponentSelector throws ArgumentException when empty string is passed as name")]
    public void NameBasedComponentSelectorThrowsOnNullOrEmpty(string name)
    {
        // Arrange
        var fac = _container.Resolve<INamedComponentFactory>();

        // Act & Assert
        Assert.That(() => fac.Create(name), Throws.InstanceOf<ArgumentException>());
    }

    [TestCase(null, Description = "ConfigBasedComponentSelector throws ArgumentException when PluginName is null")]
    [TestCase("", Description = "ConfigBasedComponentSelector throws ArgumentException when PluginName is empty")]
    public void ConfigBasedComponentSelectorThrowsOnNullOrEmptyPluginName(string pluginName)
    {
        // Arrange
        _container.LoadComponents<IRootClass>();
        _container.Register<IRootClassFactory>();
        var fac = _container.Resolve<IRootClassFactory>();
        var config = new RootClassFactoryConfig { PluginName = pluginName };

        // Act & Assert
        Assert.That(() => fac.Create(config), Throws.InstanceOf<ArgumentException>());
    }

    [TearDown]
    public void Destroy()
    {
        _container.Destroy();
    }
}
