// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Modules;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel;

/// <summary>
/// Class representing a missing module in the application
/// </summary>
internal class MissingServerModule : IServerModule
{
    public IServerModuleConsole Console => null;

    public ServerModuleState State => ServerModuleState.Missing;

    public string Name => GetName();

    public INotificationCollection Notifications => null;

    public event EventHandler<ModuleStateChangedEventArgs> StateChanged;

    public MissingServerModule(Type service, bool optional)
    {
        RepresentedService = service;
        Optional = optional;
    }

    /// <summary>
    /// Gets the name of the Module
    /// </summary>
    /// <returns></returns>
    private string GetName()
    {
        var isInterfaceAndHasIasFirstCharacter = RepresentedService.Name.ElementAt(0).ToString().ToLower() == "i" &&
                                                 RepresentedService.IsInterface;

        if (isInterfaceAndHasIasFirstCharacter)
            return RepresentedService.Name.Remove(0, 1); // remove "i" in the interface name
        else return RepresentedService.Name;
    }

    public void AcknowledgeNotification(IModuleNotification notification)
    {
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Type RepresentedService { get; private set; }

    public IContainer Container => throw new NotImplementedException();

    public bool Optional { get; }
}