// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.States;

namespace Moryx.Material.Management.Components;

[Component(LifeCycle.Singleton, typeof(IMaterialFlowHandler))]
internal class MaterialFlowHandler : IMaterialFlowHandler, ILoggingComponent
{
    public IModuleLogger Logger { get; set; }

    public IContainerPool Pool { get; set; }

    public IResourceManagement ResourceManagement { get; set; }

    public void Start() { }
    public void Stop() { }

    public async Task<IMaterialContainer> RequestMaterialAsync(MaterialRequest request, Type targetContainerType)
    {
        request.Id ??= Guid.NewGuid().ToString();

        // Create a virtual container resource
        var containerId = await ResourceManagement.CreateUnsafeAsync(targetContainerType, resource =>
        {
            resource.Name = $"Request-{request.Id}";
            resource.Description = "Generated, virtual material container for requested material";

            var container = (IMaterialContainer)resource;
            container.Identity = request.ContainerIdentity;
            container.UpdateMaterial(new MaterialUpdate
            {
                Kind = UpdateKind.MaterialType | UpdateKind.FillingLevel,
                Material = request.Material,
                Quantity = request.RequestedQuantity,
                Unit = request.Unit
            });
            return Task.CompletedTask;
        }, CancellationToken.None);

        // ToDo: Make error message more helpful
        var created = Pool.Get(containerId)
            ?? throw new InvalidOperationException($"Created container {containerId} not found in pool");

        var requestedState = new RequestedStateInformation
        {
            RequestId = request.Id,
            ExpectedArrival = request.ExpectedArrival
        };

        created.TransitionTo(requestedState);

        return created;
    }

    public Task CancelMaterialRequestAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IMaterialContainer> AnnounceMaterialAsync(MaterialAnnouncement announcement, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DropMaterialAnnouncementAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RegisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IMaterialContainer> PreAdviceMaterialAsync(IMaterialContainer container, PreAdviceDepartureReason departureReason)
    {
        var outbound = new OutboundStateInformation { DepartureReason = departureReason };
        container.TransitionTo(outbound);
        return Task.FromResult(container);
    }

    public Task DeregisterContainerAsync(IMaterialContainer container, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public event EventHandler<StateChangedEventArgs>? StateChanged;
}
