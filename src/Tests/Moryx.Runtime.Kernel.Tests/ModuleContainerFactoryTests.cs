// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moryx.Container;
using NUnit.Framework;

namespace Moryx.Runtime.Kernel.Tests;

public class ModuleContainerFactoryTests
{
    private ServiceCollection _serviceCollection;
    private TimeProvider _timeProvider;
    private IHttpClientFactory _httpClientFactory;

    [SetUp]
    public void Setup()
    {

        _serviceCollection = new ServiceCollection();
        _timeProvider = Mock.Of<TimeProvider>();
        _httpClientFactory = Mock.Of<IHttpClientFactory>();

        _serviceCollection.AddSingleton<IModuleContainerFactory, ModuleContainerFactory>();
    }

    [Test]
    public void TimeProviderDefaultsToSystemIfNoneIsRegistered()
    {
        // Arrange
        var provider = _serviceCollection.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<IModuleContainerFactory>();
        var container = factory.Create(new Dictionary<Type, string>(), this.GetType().Assembly);
        var timeProvider = container.Resolve<TimeProvider>();

        // Assert
        Assert.That(timeProvider, Is.EqualTo(TimeProvider.System));
    }

    [Test]
    public void ConfiguredTimeProviderIsPropagated()
    {
        // Arrange
        _serviceCollection.AddSingleton(_timeProvider);
        var provider = _serviceCollection.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<IModuleContainerFactory>();
        var container = factory.Create(new Dictionary<Type, string>(), this.GetType().Assembly);
        var timeProvider = container.Resolve<TimeProvider>();

        // Assert
        Assert.That(timeProvider, Is.EqualTo(_timeProvider));
    }

    [Test]
    public void ConfiguredHttpClientFactoryIsPropagated()
    {
        // Arrange
        _serviceCollection.AddSingleton(_httpClientFactory);
        var provider = _serviceCollection.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<IModuleContainerFactory>();
        var container = factory.Create(new Dictionary<Type, string>(), this.GetType().Assembly);
        var httpClientFactory = container.Resolve<IHttpClientFactory>();

        // Assert
        Assert.That(httpClientFactory, Is.EqualTo(_httpClientFactory));
    }

    [Test]
    public void ClientFactoryIsNullAndDoesNotThrowIfNotConfigured()
    {
        // Arrange
        var provider = _serviceCollection.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<IModuleContainerFactory>();
        var container = factory.Create(new Dictionary<Type, string>(), this.GetType().Assembly);

        // Assert
        IHttpClientFactory httpClientFactory = null;
        using var _ = Assert.EnterMultipleScope();
        Assert.DoesNotThrow(() => container.Resolve<IHttpClientFactory>());
        Assert.That(httpClientFactory, Is.Null);
    }
}
