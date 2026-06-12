// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.Lineage;

namespace Moryx.Material;

/// <summary>
/// Facade of the Material Management module.
/// </summary>
public interface IMaterialManagement
{
    /// <summary>
    /// Returns all registered containers.
    /// </summary>
    IReadOnlyList<IMaterialContainer> GetContainers();

    /// <summary>
    /// Returns all containers matching the given filter.
    /// </summary>
    IReadOnlyList<IMaterialContainer> GetContainers(Func<IMaterialContainer, bool> filter);

    /// <summary>
    /// Loads a container by its resource id.
    /// </summary>
    IMaterialContainer? GetContainer(long id);

    #region State transitions

    /// <summary>
    /// Records a material request, creating a "virtual" container in <see cref="States.RequestedState"/>.
    /// </summary>
    Task<IMaterialContainer> RequestMaterialAsync(MaterialRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records an inbound material announcement, transitioning the matching container into
    /// <see cref="States.InboundState"/> (or creating a new virtual container).
    /// </summary>
    Task<IMaterialContainer> AnnounceMaterialAsync(MaterialAnnouncement announcement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a physical container, transitioning it into <see cref="States.AvailableState"/>.
    /// </summary>
    Task RegisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a pre-advice for a container, transitioning it into <see cref="States.OutboundState"/>.
    /// </summary>
    Task<IMaterialContainer> PreAdviceMaterialAsync(MaterialPreAdvice preAdvice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregisters a container, transitioning it into <see cref="States.DeregisteredState"/>.
    /// </summary>
    Task DeregisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a pending material request.
    /// </summary>
    Task CancelMaterialRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Drops a previously placed material announcement.
    /// </summary>
    Task DropMaterialAnnouncementAsync(Guid announcementId, CancellationToken cancellationToken = default);

    #endregion

    #region Lineage

    /// <summary>
    /// Records a lineage event for an audit trail.
    /// </summary>
    Task RecordLineageAsync(ILineageEvent lineageEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all lineage events for the given container.
    /// </summary>
    IReadOnlyList<ILineageEvent> GetLineage(long containerId);

    /// <summary>
    /// Returns all lineage events matching the given filter.
    /// </summary>
    IReadOnlyList<ILineageEvent> GetLineage(Func<ILineageEvent, bool> filter);

    #endregion

    #region Events

    /// <summary>Raised when a container is registered.</summary>
    event EventHandler<MaterialContainerEventArgs>? ContainerRegistered;

    /// <summary>Raised when a container is deregistered.</summary>
    event EventHandler<MaterialContainerEventArgs>? ContainerDeregistered;

    /// <summary>Raised when a container's state changes.</summary>
    event EventHandler<ContainerStateChangedEventArgs>? ContainerStateChanged;

    /// <summary>Raised when a material request is recorded.</summary>
    event EventHandler<MaterialContainerEventArgs>? MaterialRequested;

    /// <summary>Raised when an inbound material announcement is recorded.</summary>
    event EventHandler<MaterialContainerEventArgs>? MaterialInbound;

    /// <summary>Raised when a container becomes available (registered).</summary>
    event EventHandler<MaterialContainerEventArgs>? ContainerAvailable;

    /// <summary>Raised when a pre-advice is recorded.</summary>
    event EventHandler<MaterialContainerEventArgs>? MaterialOutbound;

    /// <summary>Raised whenever a lineage event is persisted.</summary>
    event EventHandler<LineageRecordedEventArgs>? LineageRecorded;

    #endregion
}
