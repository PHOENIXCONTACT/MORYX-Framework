// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;

namespace Moryx.Container.Tests;

[TestFixture]
public class ServiceProviderBridgeTests
{
    [Test(Description = "Services registered in the ServiceProvider are injected into Castle components via the bridge")]
    public void InjectServiceFromServiceProviderIntoComponent()
    {
        // Arrange
        var timeProvider = TimeProvider.System;
        var serviceProvider = new ServiceProviderMock(type =>
            type == typeof(TimeProvider) ? timeProvider : null);

        var container = new CastleContainer(new Dictionary<Type, string>(), serviceProvider);
        container.Register<IComponentWithExternalDependency, ComponentWithExternalDependency>();

        // Act
        var component = container.Resolve<IComponentWithExternalDependency>();

        // Assert
        Assert.That(component, Is.Not.Null);
        Assert.That(component.TimeProvider, Is.SameAs(timeProvider));
    }

    [Test(Description = "Castle Windsor registrations take precedence over the ServiceProvider services")]
    public void CastleRegistrationTakesPrecedenceOverServiceProvider()
    {
        // Arrange
        var castleInstance = TimeProvider.System;
        var aspNetInstance = new FakeTimeProvider();

        var serviceProvider = new ServiceProviderMock(type =>
            type == typeof(TimeProvider) ? aspNetInstance : null);

        var container = new CastleContainer(new Dictionary<Type, string>(), serviceProvider);
        container.SetInstance(castleInstance);
        container.Register<IComponentWithExternalDependency, ComponentWithExternalDependency>();

        // Act
        var component = container.Resolve<IComponentWithExternalDependency>();

        // Assert
        Assert.That(component.TimeProvider, Is.SameAs(castleInstance));
    }

    [Test(Description = "Types excluded by the type filter are not resolved from the ServiceProvider")]
    public void TypeFilterExcludesFilteredTypes()
    {
        // Arrange
        var timeProvider = TimeProvider.System;
        var serviceProvider = new ServiceProviderMock(type =>
            type == typeof(TimeProvider) ? timeProvider : null);

        // Filter excludes TimeProvider
        var container = new CastleContainer(new Dictionary<Type, string>(), serviceProvider,
            type => type != typeof(TimeProvider));
        container.Register<IComponentWithExternalDependency, ComponentWithExternalDependency>();

        // Act & Assert - resolution fails because the dependency is filtered out
        Assert.That(() => container.Resolve<IComponentWithExternalDependency>(), Throws.Exception);
    }

    // TODO: Remove in next major when IServiceProvider is non-optional
    [Test(Description = "Container without IServiceProvider still resolves Castle-registered components")]
    public void ContainerWithoutServiceProviderStillWorks()
    {
        // Arrange
        var container = new CastleContainer(new Dictionary<Type, string>());
        container.LoadFromAssembly(GetType().Assembly);

        // Act
        var local = container.Resolve<ILocalComponent>();

        // Assert
        Assert.That(local, Is.Not.Null);

        container.Destroy();
    }

    public interface IComponentWithExternalDependency
    {
        TimeProvider TimeProvider { get; }
    }

    public class ComponentWithExternalDependency(TimeProvider timeProvider) : IComponentWithExternalDependency
    {
        public TimeProvider TimeProvider { get; } = timeProvider;
    }

    private class FakeTimeProvider : TimeProvider;

    private class ServiceProviderMock(Func<Type, object> resolver) : IServiceProvider
    {
        public object GetService(Type serviceType) => resolver(serviceType);
    }
}
