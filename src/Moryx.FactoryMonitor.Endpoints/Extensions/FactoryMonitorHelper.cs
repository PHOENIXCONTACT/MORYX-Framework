// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.Orders;
using Newtonsoft.Json;

namespace Moryx.FactoryMonitor.Endpoints.Extensions;

internal static class FactoryMonitorHelper
{
    public static void OrderStarted(OperationStartedEventArgs orderEventArg, Action<string, object> broadcast)
    {
        var orderModel = Converter.Converter.ToOrderModel(orderEventArg.Operation);
        broadcast("processes", orderModel);

    }

    public static void OrderUpdated(OperationChangedEventArgs orderEventArg, Action<string, object> broadcast)
    {
        if (orderEventArg.Operation.State is not OperationStateClassification.Running) return;

        var orderReferenceModel = Converter.Converter.ToOrderChangedModel(orderEventArg.Operation);
        broadcast("processes", orderReferenceModel);
    }

    public static void PublishCellUpdate(CellStateChangedModel cellModel, Action<string, object> broadcast)
    {
        broadcast("cellStateChangedModel", cellModel);
    }

    public static void ActivityUpdated(ActivityUpdatedEventArgs activityEventArg, List<ICell> cells, Resource resource,
        List<OrderModel> orderModels, Action<string, object> broadcast)
    {
        if (activityEventArg.Progress == ActivityProgress.Ready)
        {
            return;
        }

        if (cells.All(x => x.Id != activityEventArg.Activity.Tracing.ResourceId))
        {
            return;
        }

        var cell = resource as ICell;
        if (cell == null)
        {
            return;
        }

        var activityChangedModel = cell.GetActivityChangedModel(activityEventArg.Activity, orderModels);
        broadcast("activityChangedModel", activityChangedModel);

        var cellStateChangedModel = cell.GetCellStateChangedModel(activityEventArg.Progress, resource);
        broadcast("cellStateChangedModel", cellStateChangedModel);
    }

    public static void ResourceUpdated(IResourceManagement resourceManager,
        Func<IMachineLocation, bool> cellFilter, Converter.Converter converter, Action<string, object> broadcast)
    {
        var cells = resourceManager.GetResources(cellFilter)
            .Select(location => location.Machine)
            .Cast<ICell>();

        foreach (var cell in cells)
        {
            var resourceChangedModel = cell.GetResourceChangedModel(converter, resourceManager, cellFilter);
            broadcast("resourceChangedModel", resourceChangedModel);
        }
    }

    public static List<TransportRouteModel> CreateRoutes(IReadOnlyList<IMachineLocation> locations)
    {
        var routes = new List<TransportRouteModel>();

        foreach (var location in locations)
        {
            var result = location.Destinations.Select(x => new TransportPathModel
            {
                Destination = Converter.Converter.ToCellLocationModel(x.Destination),
                Origin = Converter.Converter.ToCellLocationModel(x.Origin),
                WayPoints = x.WayPoints
            }).Select(Converter.Converter.ToTransportRouteModel).ToList();

            routes.AddRange(result);
        }
        return routes;
    }
}
