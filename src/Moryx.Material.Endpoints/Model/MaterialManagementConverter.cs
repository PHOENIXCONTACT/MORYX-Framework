// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Factory;
using Moryx.Material.States;
using Moryx.Tools;

namespace Moryx.Material.Endpoints.Model;

internal static class MaterialManagementConverter
{
    #region ToModel
    public static MaterialContainerTypeModel ToModel(this Type type) => new()
    {
        FullName = type.FullName ?? "",
        DisplayName = type.GetDisplayName(),
        Description = type.GetDescription(),
        Icon = type.GetCustomAttribute<EntryVisualizationAttribute>(true)?.Icon ?? ""
    };

    public static ContainerHostModel? ToModel(this IResource? resource)
    {
        if (resource is null)
        {
            return null;
        }

        var type = resource.GetType();
        return new ContainerHostModel
        {
            Id = resource.Id,
            Name = resource.Name,
            TypeName = type.GetDisplayName(),
            TypeDescription = type.GetDescription()
        };
    }

    public static MaterialStateClassificationModel ToModel(this StateClassification state) => state switch
    {
        StateClassification.Uninitialized => MaterialStateClassificationModel.Uninitialized,
        StateClassification.Requested => MaterialStateClassificationModel.Requested,
        StateClassification.Inbound => MaterialStateClassificationModel.Inbound,
        StateClassification.Available => MaterialStateClassificationModel.Available,
        StateClassification.Outbound => MaterialStateClassificationModel.Outbound,
        StateClassification.Deregistered => MaterialStateClassificationModel.Deregistered,
        _ => MaterialStateClassificationModel.Uninitialized
    };

    public static PreAdviceDepartureReasonModel ToModel(this PreAdviceDepartureReason reason) => reason switch
    {
        PreAdviceDepartureReason.FinishedGoods => PreAdviceDepartureReasonModel.FinishedGoods,
        PreAdviceDepartureReason.UnusedMaterial => PreAdviceDepartureReasonModel.UnusedMaterial,
        PreAdviceDepartureReason.Transfer => PreAdviceDepartureReasonModel.Transfer,
        PreAdviceDepartureReason.Scrap => PreAdviceDepartureReasonModel.Scrap,
        PreAdviceDepartureReason.Other => PreAdviceDepartureReasonModel.Other,
        _ => PreAdviceDepartureReasonModel.Other
    };

    public static MaterialContainerModel ToModel(this IMaterialContainer container)
    {
        return new MaterialContainerModel
        {
            Id = container.Id,
            Name = container.Name,
            ContainerHost = container.ContainerHost.ToModel(),
            Identity = container.Identity?.Identifier,
            Material = container.Material,
            Quantity = container.Quantity,
            Unit = container.Unit,
            State = container.State.ToModel(),
            Type = container.GetResourceType().ToModel()
        };
    }

    public static IReadOnlyList<MaterialContainerModel> ToModels(this IEnumerable<IMaterialContainer> containers)
        => containers.Select(c => c.ToModel()).ToArray();

    public static PreAdviceModel ToModel(this MaterialPreAdvice preAdvice) => new()
    {
        ContainerId = preAdvice.ContainerId,
        DepartureReason = preAdvice.DepartureReason.ToModel()
    };
    #endregion

    #region ToBusiness
    public static PreAdviceDepartureReason ToBusiness(this PreAdviceDepartureReasonModel reason) => reason switch
    {
        PreAdviceDepartureReasonModel.FinishedGoods => PreAdviceDepartureReason.FinishedGoods,
        PreAdviceDepartureReasonModel.UnusedMaterial => PreAdviceDepartureReason.UnusedMaterial,
        PreAdviceDepartureReasonModel.Transfer => PreAdviceDepartureReason.Transfer,
        PreAdviceDepartureReasonModel.Scrap => PreAdviceDepartureReason.Scrap,
        PreAdviceDepartureReasonModel.Other => PreAdviceDepartureReason.Other,
        _ => PreAdviceDepartureReason.Other
    };

    public static MaterialPreAdvice ToBusiness(this PreAdviceModel model) => new()
    {
        ContainerId = model.ContainerId,
        DepartureReason = model.DepartureReason.ToBusiness()
    };
    #endregion
}
