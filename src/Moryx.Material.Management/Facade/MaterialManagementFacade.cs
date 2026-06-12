// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Lineage;
using Moryx.Material.Management.Components;
using Moryx.Material.States;
using Moryx.Runtime.Modules;

namespace Moryx.Material.Management;

internal class MaterialManagementFacade : IMaterialManagement, IFacadeControl
{
    public IContainerPool Pool { get; set; } = null!;

    public IContainerStateHandler StateHandler { get; set; } = null!;

    public IFulfillmentMatcher Matcher { get; set; } = null!;

    public ILineageEventStorage LineageStorage { get; set; } = null!;

    public IResourceManagement ResourceManagement { get; set; } = null!;

    public Action ValidateHealthState { get; set; } = null!;

    public void Activate()
    {
        StateHandler.StateChanged += OnStateChanged;
        StateHandler.ContainerAvailable += OnContainerAvailable;
        StateHandler.ContainerDeregistered += OnContainerDeregistered;
        StateHandler.MaterialRequested += OnMaterialRequested;
        StateHandler.MaterialInbound += OnMaterialInbound;
        StateHandler.MaterialOutbound += OnMaterialOutbound;

        Pool.ContainerAdded += OnContainerAdded;
        Pool.ContainerRemoved += OnContainerRemoved;

        LineageStorage.Recorded += OnLineageRecorded;
    }

    public void Deactivate()
    {
        StateHandler.StateChanged -= OnStateChanged;
        StateHandler.ContainerAvailable -= OnContainerAvailable;
        StateHandler.ContainerDeregistered -= OnContainerDeregistered;
        StateHandler.MaterialRequested -= OnMaterialRequested;
        StateHandler.MaterialInbound -= OnMaterialInbound;
        StateHandler.MaterialOutbound -= OnMaterialOutbound;

        Pool.ContainerAdded -= OnContainerAdded;
        Pool.ContainerRemoved -= OnContainerRemoved;

        LineageStorage.Recorded -= OnLineageRecorded;
    }

    #region Queries

    public IReadOnlyList<IMaterialContainer> GetContainers()
    {
        ValidateHealthState();
        return Pool.GetAll();
    }

    public IReadOnlyList<IMaterialContainer> GetContainers(Func<IMaterialContainer, bool> filter)
    {
        ValidateHealthState();
        return Pool.GetAll(filter);
    }

    public IMaterialContainer? GetContainer(long id)
    {
        ValidateHealthState();
        return Pool.Get(id);
    }

    #endregion

    #region State Transitions

    public async Task<IMaterialContainer> RequestMaterialAsync(MaterialRequest request, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        if (request == null) throw new ArgumentNullException(nameof(request));

        request.Id ??= Guid.NewGuid();

        // Create a virtual container resource
        var containerId = await ResourceManagement.CreateUnsafeAsync(typeof(BasicMaterialContainer), resource =>
        {
            var container = (BasicMaterialContainer)resource;
            container.Name = $"Request-{request.Id}";
            container.Identity = request.ContainerIdentity;
            container.Material = request.Material;
            container.Quantity = request.RequestedQuantity;
            container.Unit = request.Unit;
            return Task.CompletedTask;
        }, cancellationToken);

        var created = Pool.Get(containerId)
            ?? throw new InvalidOperationException($"Created container {containerId} not found in pool");

        var requestedState = new RequestedState
        {
            RequestId = request.Id,
            ExpectedArrival = request.ExpectedArrival
        };

        await StateHandler.TransitionAsync(created, requestedState, cancellationToken);
        return created;
    }

    public async Task<IMaterialContainer> AnnounceMaterialAsync(MaterialAnnouncement announcement, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        if (announcement == null) throw new ArgumentNullException(nameof(announcement));

        announcement.Id ??= Guid.NewGuid();

        // Try to fulfill an existing request
        var existing = Matcher.TryMatchAnnouncement(announcement);
        if (existing != null)
        {
            // Match: transition existing virtual container to Inbound
            if (existing.State is RequestedState rs)
                rs.IsPartiallyFulfilled = true;

            if (announcement.ContainerIdentity != null)
                existing.Identity = announcement.ContainerIdentity;

            var inboundState = new InboundState
            {
                AnnouncementId = announcement.Id,
                ExpectedArrival = announcement.ExpectedArrival,
                RequestReference = announcement.RequestReference
            };

            await StateHandler.TransitionAsync(existing, inboundState, cancellationToken);
            return existing;
        }

        // No matching request - create a new virtual container
        var containerId = await ResourceManagement.CreateUnsafeAsync(typeof(BasicMaterialContainer), resource =>
        {
            var container = (BasicMaterialContainer)resource;
            container.Name = $"Announcement-{announcement.Id}";
            container.Identity = announcement.ContainerIdentity;
            container.Material = announcement.Material;
            container.Quantity = announcement.AnnouncedQuantity;
            container.Unit = announcement.Unit;
            return Task.CompletedTask;
        }, cancellationToken);

        var created = Pool.Get(containerId)
            ?? throw new InvalidOperationException($"Created container {containerId} not found in pool");

        var newInbound = new InboundState
        {
            AnnouncementId = announcement.Id,
            ExpectedArrival = announcement.ExpectedArrival,
            RequestReference = announcement.RequestReference
        };
        await StateHandler.TransitionAsync(created, newInbound, cancellationToken);
        return created;
    }

    public async Task RegisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        if (container == null) throw new ArgumentNullException(nameof(container));

        // If a matching virtual container exists (Requested or Inbound), apply identity to it instead
        var match = Matcher.TryMatchRegistration(container);
        if (match != null)
        {
            if (container.Identity != null)
                match.Identity = container.Identity;

            await StateHandler.TransitionAsync(match, new AvailableState(), cancellationToken);

            await LineageStorage.RecordAsync(new RegisterLineageEvent
            {
                ContainerId = match.Id,
                Material = match.Material,
                Quantity = match.Quantity
            }, cancellationToken);
            return;
        }

        await StateHandler.TransitionAsync(container, new AvailableState(), cancellationToken);
        await LineageStorage.RecordAsync(new RegisterLineageEvent
        {
            ContainerId = container.Id,
            Material = container.Material,
            Quantity = container.Quantity
        }, cancellationToken);
    }

    public async Task<IMaterialContainer> PreAdviceMaterialAsync(MaterialPreAdvice preAdvice, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        if (preAdvice == null) throw new ArgumentNullException(nameof(preAdvice));
        if (preAdvice.Container == null)
            throw new ArgumentException("Container is required.", nameof(preAdvice));

        var outbound = new OutboundState { DepartureReason = preAdvice.DepartureReason };
        await StateHandler.TransitionAsync(preAdvice.Container, outbound, cancellationToken);
        return preAdvice.Container;
    }

    public async Task DeregisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        if (container == null) throw new ArgumentNullException(nameof(container));

        var finalQuantity = container.Quantity;
        await StateHandler.TransitionAsync(container, new DeregisteredState(), cancellationToken);
        await LineageStorage.RecordAsync(new DeregisterLineageEvent
        {
            ContainerId = container.Id,
            FinalQuantity = finalQuantity
        }, cancellationToken);
    }

    public async Task CancelMaterialRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();

        var match = Pool.GetAll(c => c.State is RequestedState rs && rs.RequestId == requestId).FirstOrDefault();
        if (match == null)
            throw new ArgumentException($"No pending request with id {requestId}.", nameof(requestId));

        await StateHandler.TransitionAsync(match, new DeregisteredState(), cancellationToken);
    }

    public async Task DropMaterialAnnouncementAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();

        var match = Pool.GetAll(c => c.State is InboundState ins && ins.AnnouncementId == announcementId).FirstOrDefault();
        if (match == null)
            throw new ArgumentException($"No active announcement with id {announcementId}.", nameof(announcementId));

        await StateHandler.TransitionAsync(match, new DeregisteredState(), cancellationToken);
    }

    #endregion

    #region Lineage

    public Task RecordLineageAsync(ILineageEvent lineageEvent, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        return LineageStorage.RecordAsync(lineageEvent, cancellationToken);
    }

    public IReadOnlyList<ILineageEvent> GetLineage(long containerId)
    {
        ValidateHealthState();
        return LineageStorage.GetForContainer(containerId);
    }

    public IReadOnlyList<ILineageEvent> GetLineage(Func<ILineageEvent, bool> filter)
    {
        ValidateHealthState();
        return LineageStorage.Query(filter);
    }

    #endregion

    #region Event forwarding

    private void OnContainerAdded(object? sender, IMaterialContainer container) =>
        ContainerRegistered?.Invoke(this, new MaterialContainerEventArgs(container));

    private void OnContainerRemoved(object? sender, IMaterialContainer container) =>
        ContainerDeregistered?.Invoke(this, new MaterialContainerEventArgs(container));

    private void OnStateChanged(object? sender, ContainerStateChangedEventArgs e) =>
        ContainerStateChanged?.Invoke(this, e);

    private void OnContainerAvailable(object? sender, MaterialContainerEventArgs e) =>
        ContainerAvailable?.Invoke(this, e);

    private void OnContainerDeregistered(object? sender, MaterialContainerEventArgs e) =>
        ContainerDeregistered?.Invoke(this, e);

    private void OnMaterialRequested(object? sender, MaterialContainerEventArgs e) =>
        MaterialRequested?.Invoke(this, e);

    private void OnMaterialInbound(object? sender, MaterialContainerEventArgs e) =>
        MaterialInbound?.Invoke(this, e);

    private void OnMaterialOutbound(object? sender, MaterialContainerEventArgs e) =>
        MaterialOutbound?.Invoke(this, e);

    private void OnLineageRecorded(object? sender, ILineageEvent e) =>
        LineageRecorded?.Invoke(this, new LineageRecordedEventArgs(e));

    #endregion

    public event EventHandler<MaterialContainerEventArgs>? ContainerRegistered;
    public event EventHandler<MaterialContainerEventArgs>? ContainerDeregistered;
    public event EventHandler<ContainerStateChangedEventArgs>? ContainerStateChanged;
    public event EventHandler<MaterialContainerEventArgs>? MaterialRequested;
    public event EventHandler<MaterialContainerEventArgs>? MaterialInbound;
    public event EventHandler<MaterialContainerEventArgs>? ContainerAvailable;
    public event EventHandler<MaterialContainerEventArgs>? MaterialOutbound;
    public event EventHandler<LineageRecordedEventArgs>? LineageRecorded;
}
