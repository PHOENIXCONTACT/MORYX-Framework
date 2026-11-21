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

        var container = new CastleContainer(serviceProvider);
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

        var container = new CastleContainer(serviceProvider);
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
        var container = new CastleContainer(serviceProvider,
            type => type != typeof(TimeProvider));
        container.Register<IComponentWithExternalDependency, ComponentWithExternalDependency>();

        // Act & Assert - resolution fails because the dependency is filtered out
        Assert.That(() => container.Resolve<IComponentWithExternalDependency>(), Throws.Exception);
    }

    [Test(Description = "Container cannot be created without an IServiceProvider")]
    public void ContainerRequiresServiceProvider()
    {
        // Act & Assert
        Assert.That(() => new CastleContainer(null), Throws.ArgumentNullException);
    }

    public interface IComponentWithExternalDependency
    {
        TimeProvider TimeProvider { get; }
    }

    public class ComponentWithExternalDependency(TimeProvider timeProvider) : IComponentWithExternalDependency
    {
        public TimeProvider TimeProvider { get; } = timeProvider;
    }

    private sealed class FakeTimeProvider : TimeProvider;

    private sealed class ServiceProviderMock(Func<Type, object> resolver) : IServiceProvider
    {
        public object GetService(Type serviceType) => resolver(serviceType);
    }
}
