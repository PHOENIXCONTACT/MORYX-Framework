// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.AbstractionLayer.Resources;
using Moryx.Material.Facade;
using Moryx.Material.Lineage;
using Moryx.Material.Management.Components;
using Moryx.Material.States;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Material.Management;

internal class MaterialManagementFacade : FacadeBase, IMaterialManagement
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IContainerPool Pool { get; set; }

    public IMaterialFlowHandler MaterialFlowHandler { get; set; }

    public IFulfillmentMatcher Matcher { get; set; }

    public ILineageEventStorage LineageStorage { get; set; }

    public IResourceManagement ResourceManagement { get; set; }

    public IResourceTypeTree ResourceTypes { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public override void Activate()
    {
        base.Activate();
        Pool.StateChanged += OnStateChanged;
        Pool.ContainerUpdated += OnContainerUpdated;
        LineageStorage.Recorded += OnLineageRecorded;
    }

    private void OnStateChanged(object? sender, ContainerStateChangedEventArgs e)
        => ContainerStateChanged?.Invoke(this, e);

    private void OnContainerUpdated(object? sender, ContainerUpdatedEventArgs e) => ContainerUpdated?.Invoke(this, e);

    private void OnLineageRecorded(object? sender, ILineageEvent e) =>
        LineageRecorded?.Invoke(this, new LineageRecordedEventArgs((IMaterialContainer)sender, e));

    public override void Deactivate()
    {
        Pool.StateChanged -= OnStateChanged;
        Pool.ContainerUpdated -= OnContainerUpdated;
        LineageStorage.Recorded -= OnLineageRecorded;
        base.Deactivate();
    }

    #region Create

    #endregion

    #region Read

    public IReadOnlyList<Type> GetContainerTypes()
    {
        ValidateHealthState();

        var supportingTypeNodes = ResourceTypes.SupportedTypes(typeof(IMaterialContainer));
        var supportingTypes = new List<Type>();
        supportingTypeNodes.ForEach(tn => ExtractAllSupportedTypes(tn, supportingTypes));
        // TODO: Add configurable filter to hide basic types, e.g. from Moryx.Material
        return supportingTypes;
    }

    private static void ExtractAllSupportedTypes(IResourceTypeNode typeNode, List<Type> supportingTypes)
    {
        // Add non-abstract type
        if (typeNode.Creatable)
        {
            supportingTypes.Add(typeNode.ResourceType);
        }

        // Add derived types which will all also be supported types
        typeNode.DerivedTypes.ForEach(dt => ExtractAllSupportedTypes(dt, supportingTypes));
    }

    public IReadOnlyList<IMaterialContainer> GetContainers()
    {
        ValidateHealthState();
        return Pool.GetAll();
    }

    public IReadOnlyList<IMaterialContainer> GetContainers(Func<IMaterialContainer, bool> filter)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(filter, nameof(filter));

        return Pool.GetAll(filter);
    }

    public IMaterialContainer? GetContainer(IIdentity identity)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(identity);

        return Pool.GetAll(c => c.Identity != null && identity.Equals(c.Identity)).FirstOrDefault();
    }

    #endregion

    #region Update

    public async Task<IMaterialContainer> RequestMaterialAsync(MaterialRequest request, Type targetContainerType, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(request);
        if (!GetContainerTypes().Contains(targetContainerType))
        {
            throw new InvalidOperationException($"{targetContainerType.Name} is not a valid typeNode of material container. " +
                "Check that the necessary packages are known to the assembly and the module configuration for enabled container types.");
        }
        cancellationToken.ThrowIfCancellationRequested();

        return await MaterialFlowHandler.RequestMaterialAsync(request, targetContainerType);
    }

    public async Task<IMaterialContainer> AnnounceMaterialAsync(MaterialAnnouncement announcement, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(announcement);

        announcement.Id ??= Guid.NewGuid().ToString();

        // Try to fulfill an existing request
        var existing = Matcher.TryMatch(announcement);
        if (existing != null)
        {
            // Match: transition existing virtual container to Inbound
            if (GetStateInformation(existing) is RequestedStateInformation rs)
            {
                rs.IsPartiallyFulfilled = true;
            }

            if (announcement.ContainerIdentity != null)
            {
                existing.Identity = announcement.ContainerIdentity;
            }

            var inboundState = new InboundStateInformation
            {
                AnnouncementId = announcement.Id,
                ExpectedArrival = announcement.ExpectedArrival,
                RequestReference = announcement.RequestReference
            };

            //await MaterialFlowHandler.TransitionAsync(existing, inboundState, cancellationToken);
            return existing;
        }

        // No matching request - create a new virtual container
        var containerId = await ResourceManagement.CreateUnsafeAsync(typeof(BasicMaterialContainer), resource =>
        {
            var container = (BasicMaterialContainer)resource;
            container.Name = $"Announcement-{announcement.Id}";
            container.Identity = announcement.ContainerIdentity;
            container.Unit = announcement.Unit;
            container.UpdateMaterial(new MaterialUpdate
            {
                Kind = UpdateKind.MaterialType | UpdateKind.FillingLevel,
                Material = announcement.Material,
                Quantity = announcement.AnnouncedQuantity,
                Unit = announcement.Unit
            });
            return Task.CompletedTask;
        }, cancellationToken);

        var created = Pool.Get(containerId)
            ?? throw new InvalidOperationException($"Created container {containerId} not found in pool");

        var newInbound = new InboundStateInformation
        {
            AnnouncementId = announcement.Id,
            ExpectedArrival = announcement.ExpectedArrival,
            RequestReference = announcement.RequestReference
        };
        //await MaterialFlowHandler.TransitionAsync(created, newInbound, cancellationToken);
        return created;
    }

    public async Task RegisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        ArgumentNullException.ThrowIfNull(container);

        // If a matching virtual container exists (Requested or Inbound), apply identity to it instead
        var match = Matcher.TryMatchRegistration(container);
        if (match != null)
        {
            if (container.Identity != null)
            {
                match.Identity = container.Identity;
            }

            //await MaterialFlowHandler.TransitionAsync(match, new AvailableStateInformation(), cancellationToken);

            await LineageStorage.RecordAsync(new RegisterLineageEvent
            {
                ContainerId = match.Id,
                Material = match.Material,
                Quantity = match.Quantity
            }, cancellationToken);
            return;
        }

        //await MaterialFlowHandler.TransitionAsync(container, new AvailableStateInformation(), cancellationToken);
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
        ArgumentNullException.ThrowIfNull(preAdvice);
        var container = Pool.Get(preAdvice.ContainerId) ??
            throw new KeyNotFoundException("Material container for pre-advice could not be found.");
        cancellationToken.ThrowIfCancellationRequested();

        return await MaterialFlowHandler.PreAdviceMaterialAsync(container, preAdvice.DepartureReason);
    }

    public async Task CancelMaterialRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();

        var match = Pool.GetAll(c => GetStateInformation(c) is RequestedStateInformation rs && string.Equals(rs.RequestId, requestId.ToString(), StringComparison.Ordinal)).FirstOrDefault();
        if (match == null)
        {
            throw new ArgumentException($"No pending request with id {requestId}.", nameof(requestId));
        }

        //await MaterialFlowHandler.TransitionAsync(match, new DeregisteredStateInformation(), cancellationToken);
    }

    public async Task DropMaterialAnnouncementAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();

        var match = Pool.GetAll(c => GetStateInformation(c) is InboundStateInformation ins && string.Equals(ins.AnnouncementId, announcementId.ToString(), StringComparison.Ordinal)).FirstOrDefault();
        if (match == null)
        {
            throw new ArgumentException($"No active announcement with id {announcementId}.", nameof(announcementId));
        }

        //await MaterialFlowHandler.TransitionAsync(match, new DeregisteredStateInformation(), cancellationToken);
    }

    #endregion

    #region Delete

    public async Task DeregisterContainerAsync(long id, CancellationToken cancellationToken = default)
    {
        ValidateHealthState();
        var container = Pool.Get(id) ??
            throw new KeyNotFoundException($"Could not find an {nameof(IMaterialContainer)} with {nameof(IPersistentObject.Id)} '{id}'");

        var finalQuantity = container.Quantity;
        await MaterialFlowHandler.DeregisterContainerAsync(container, cancellationToken);
        await LineageStorage.RecordAsync(new DeregisterLineageEvent
        {
            ContainerId = id,
            FinalQuantity = finalQuantity
        }, cancellationToken);
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

    #region Events

    public event EventHandler<ContainerStateChangedEventArgs>? ContainerStateChanged;
    public event EventHandler<ContainerUpdatedEventArgs>? ContainerUpdated;
    public event EventHandler<LineageRecordedEventArgs>? LineageRecorded;

    #endregion

    private static StateInformation? GetStateInformation(IMaterialContainer container) =>
    (container as MaterialContainer)?.StateInformation;
}
