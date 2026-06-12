// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Lineage;
using Moryx.Material.States;

namespace Moryx.Material.Management.Components;

[Component(LifeCycle.Singleton, typeof(IContainerStateHandler))]
internal class ContainerStateHandler : IContainerStateHandler, ILoggingComponent
{
    public IModuleLogger Logger { get; set; } = null!;

    public ILineageEventStorage LineageStorage { get; set; } = null!;

    public void Start() { }
    public void Stop() { }

    public async Task TransitionAsync(IMaterialContainer container, MaterialContainerStateBase newState, CancellationToken cancellationToken = default)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));
        if (newState == null) throw new ArgumentNullException(nameof(newState));

        var oldState = container.State;
        var oldClassification = oldState?.Classification;

        // Apply on the resource. We use the protected internal helper if available.
        if (container is MaterialContainer baseContainer)
            baseContainer.TransitionTo(newState);
        else
            Logger?.Log(LogLevel.Warning,
                "Container {0} does not extend MaterialContainer; cannot drive transition from module.",
                container.Id);

        // Record lineage
        var lineage = new StateTransitionLineageEvent
        {
            ContainerId = container.Id,
            FromClassification = oldClassification,
            ToClassification = newState.Classification
        };
        await LineageStorage.RecordAsync(lineage, cancellationToken);

        // Raise generic event
        StateChanged?.Invoke(this, new ContainerStateChangedEventArgs(container, oldState, newState));

        // Raise specific event
        switch (newState.Classification)
        {
            case MaterialContainerStateClassification.Requested:
                MaterialRequested?.Invoke(this, new MaterialContainerEventArgs(container));
                break;
            case MaterialContainerStateClassification.Inbound:
                MaterialInbound?.Invoke(this, new MaterialContainerEventArgs(container));
                break;
            case MaterialContainerStateClassification.Available:
                ContainerAvailable?.Invoke(this, new MaterialContainerEventArgs(container));
                break;
            case MaterialContainerStateClassification.Outbound:
                MaterialOutbound?.Invoke(this, new MaterialContainerEventArgs(container));
                break;
            case MaterialContainerStateClassification.Deregistered:
                ContainerDeregistered?.Invoke(this, new MaterialContainerEventArgs(container));
                break;
        }
    }

    public event EventHandler<ContainerStateChangedEventArgs>? StateChanged;
    public event EventHandler<MaterialContainerEventArgs>? ContainerAvailable;
    public event EventHandler<MaterialContainerEventArgs>? ContainerDeregistered;
    public event EventHandler<MaterialContainerEventArgs>? MaterialRequested;
    public event EventHandler<MaterialContainerEventArgs>? MaterialInbound;
    public event EventHandler<MaterialContainerEventArgs>? MaterialOutbound;
}