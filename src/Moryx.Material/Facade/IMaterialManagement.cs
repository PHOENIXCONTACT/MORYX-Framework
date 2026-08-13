// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using Moryx.Material.Lineage;

namespace Moryx.Material.Facade;

/// <summary>
/// Facade of the Material Management module.
/// </summary>
public interface IMaterialManagement
{
    // TODO: Decide whether we should map CREATE actions from resource facade to here
    // Pro: Validate Unique Identities on containers
    #region Create

    #endregion

    // TODO: Decide whether we should map READ actions for container from resource facade to here
    #region Read

    IReadOnlyList<Type> GetContainerTypes();

    /// <summary>
    /// Returns all registered containers.
    /// </summary>
    /// <returns>All containers known to material management.</returns>
    IReadOnlyList<IMaterialContainer> GetContainers();

    /// <summary>
    /// Returns all containers matching the given filter.
    /// </summary>
    /// <param name="filter">Predicate used to select containers.</param>
    /// <returns>Containers for which <paramref name="filter"/> returns <c>true</c>.</returns>
    IReadOnlyList<IMaterialContainer> GetContainers(Func<IMaterialContainer, bool> filter);

    /// <summary>
    /// Loads a container by its identity.
    /// </summary>
    /// <param name="identity">Identity of the requested container.</param>
    /// <returns>The matching container, or <c>null</c> if no container exists for <paramref name="identity"/>.</returns>
    IMaterialContainer? GetContainer(IIdentity identity);

    #endregion

    #region Update

    /// <summary>
    /// Records a material request, creating a virtual container in <see cref="States.RequestedStateInformation"/>.
    /// </summary>
    /// <param name="request">Material request to record.</param>
    /// <param name="targetContainerType"></param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The requested virtual container.</returns>
    Task<IMaterialContainer> RequestMaterialAsync(MaterialRequest request, Type targetContainerType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a pending material request.
    /// </summary>
    /// <param name="requestId">Identifier of the request to cancel.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    Task CancelMaterialRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records an inbound material announcement, transitioning the matching container into
    /// <see cref="States.InboundStateInformation"/> (or creating a new virtual container).
    /// </summary>
    /// <param name="announcement">Inbound material announcement to record.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The container associated with the announcement.</returns>
    Task<IMaterialContainer> AnnounceMaterialAsync(MaterialAnnouncement announcement, CancellationToken cancellationToken = default);

    /// <summary>
    /// Drops a previously placed material announcement.
    /// </summary>
    /// <param name="announcementId">Identifier of the announcement to drop.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    Task DropMaterialAnnouncementAsync(Guid announcementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a physical container, transitioning it into <see cref="States.AvailableStateInformation"/>.
    /// </summary>
    /// <param name="container">Container to register.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    Task RegisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records a pre-advice for a container, transitioning it into <see cref="States.OutboundStateInformation"/>.
    /// </summary>
    /// <param name="preAdvice">Pre-advice describing the outbound container and reason.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>The container associated with the pre-advice.</returns>
    Task<IMaterialContainer> PreAdviceMaterialAsync(MaterialPreAdvice preAdvice, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregisters a container using its <see cref="IPersistentObject.Id"/>, transitioning it into <see cref="States.DeregisteredStateInformation"/>.
    /// </summary>
    /// <param name="id">Id of the container to be deregistered</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    Task DeregisterContainerAsync(long id, CancellationToken cancellationToken = default);
    #endregion

    // TODO: Decide whether we should map DELETE actions for container from resource facade to here
    // Pro: Transition state and fire event before removing resource possible
    #region Delete

    #endregion

    #region Lineage

    /// <summary>
    /// Records a lineage event for an audit trail.
    /// </summary>
    /// <param name="lineageEvent">Lineage event to persist.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    Task RecordLineageAsync(ILineageEvent lineageEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all lineage events for the given container.
    /// </summary>
    /// <param name="containerId">Resource identity of the container whose lineage is requested.</param>
    /// <returns>Lineage events associated with <paramref name="containerId"/>.</returns>
    IReadOnlyList<ILineageEvent> GetLineage(long containerId);

    /// <summary>
    /// Returns all lineage events matching the given filter.
    /// </summary>
    /// <param name="filter">Predicate used to select lineage events.</param>
    /// <returns>Lineage events for which <paramref name="filter"/> returns <c>true</c>.</returns>
    IReadOnlyList<ILineageEvent> GetLineage(Func<ILineageEvent, bool> filter);

    #endregion

    #region Events
    /// <summary>Raised when a container's state changes.</summary>
    event EventHandler<ContainerUpdatedEventArgs>? ContainerUpdated;

    /// <summary>Raised when a container's state changes.</summary>
    event EventHandler<ContainerStateChangedEventArgs>? ContainerStateChanged;

    /// <summary>Raised whenever a lineage event is persisted.</summary>
    event EventHandler<LineageRecordedEventArgs>? LineageRecorded;

    #endregion
}
