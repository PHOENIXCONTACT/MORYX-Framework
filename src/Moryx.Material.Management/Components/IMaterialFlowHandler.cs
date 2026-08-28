// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Material.States;
using Moryx.Modules;

namespace Moryx.Material.Management.Components;

/// <summary>
/// Coordinates state transitions on <see cref="IMaterialContainer"/> resources, raising
/// the appropriate facade events and recording lineage entries.
/// </summary>
internal interface IMaterialFlowHandler : IPlugin
{
    Task<IMaterialContainer> RequestMaterialAsync(MaterialRequest request, Type targetContainerType);

    Task CancelMaterialRequestAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task<IMaterialContainer> AnnounceMaterialAsync(MaterialAnnouncement announcement, CancellationToken cancellationToken = default);

    Task DropMaterialAnnouncementAsync(Guid announcementId, CancellationToken cancellationToken = default);

    Task RegisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default);

    Task<IMaterialContainer> PreAdviceMaterialAsync(IMaterialContainer container, PreAdviceDepartureReason reason, CancellationToken cancellationToken);

    Task DeregisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default);
}
