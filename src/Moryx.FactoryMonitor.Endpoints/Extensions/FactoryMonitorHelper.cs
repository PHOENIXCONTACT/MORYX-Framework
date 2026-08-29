// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Resources;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.Orders;

namespace Moryx.FactoryMonitor.Endpoints.Extensions;

internal static class FactoryMonitorHelper
{
    private const string Order_Event_Type_Key = "order";
    private const string Order_Changed_Event_Type_Key = "orderChanged";
    private const string Cell_State_Event_Type_Key = "cellStateChangedModel";
    private const string Activity_Event_Type_Key = "activityChangedModel";
    private const string Recource_Event_Type_Key = "resourceChangedModel";

    public static void OrderStarted(OperationStartedEventArgs orderEventArg, List<OrderModel> models, Action<string, object> broadcast)
    {
        var orderModel = models.Single(o => o.Order == orderEventArg.Operation.Order.Number && o.Operation == orderEventArg.Operation.Number);
        broadcast(Order_Event_Type_Key, orderModel);
    }

    public static void OrderUpdated(OperationChangedEventArgs orderEventArg, Action<string, object> broadcast)
    {
        if (orderEventArg.Operation.State is not OperationStateClassification.Running)
        {
            return;
        }

        var orderReferenceModel = Converter.Converter.ToOrderChangedModel(orderEventArg.Operation);
        broadcast(Order_Changed_Event_Type_Key, orderReferenceModel);
    }

    public static void PublishCellUpdate(CellStateChangedModel cellModel, Action<string, object> broadcast)
    {
        broadcast(Cell_State_Event_Type_Key, cellModel);
    }

    public static void ActivityUpdated(ActivityUpdatedEventArgs activityEventArg, List<ICell> cells,
        List<OrderModel> orderModels, Action<string, object> broadcast)
    {
        if (activityEventArg.Progress == ActivityProgress.Ready)
        {
            return;
        }

        var cell = cells.FirstOrDefault(x => x.Id == activityEventArg.Activity.Tracing.ResourceId);
        if (cell is null)
        {
            return;
        }

        var activityChangedModel = cell.GetActivityChangedModel(activityEventArg.Activity, orderModels);
        broadcast(Activity_Event_Type_Key, activityChangedModel);

        var cellStateChangedModel = cell.GetCellStateChangedModel(activityEventArg.Progress);
        broadcast(Cell_State_Event_Type_Key, cellStateChangedModel);
    }

    // ToDo: Added and removed resources not reflected
    public static void ResourceUpdated(IResource changedResource,
        Dictionary<IMachineLocation, ICell> locationToCellMappings,
        Converter.Converter converter,
        IResourceManagement resourceManager,
        Action<string, object> broadcast)
    {
        var mapping = locationToCellMappings.FirstOrDefault(l2c => l2c.Value.Id == changedResource.Id);
        if (mapping.Key is null)
            return;

        var resourceChangedModel = mapping.Value.GetResourceChangedModel(converter, resourceManager, mapping.Key);
        broadcast(Recource_Event_Type_Key, resourceChangedModel);
    }

    public static void ResourceUpdated(IResourceManagement resourceManager,
        Func<IEnumerable<IMachineLocation>, Dictionary<IMachineLocation, ICell>> mapCellsTo,
        Converter.Converter converter,
        Action<string, object> broadcast)
    {
        var locations = resourceManager.GetResources<IMachineLocation>();
        // ToDo: Added and removed resources not reflected
        var locationToCellMappings = mapCellsTo(locations);

        foreach (var l2cMapping in locationToCellMappings)
        {
            var cell = l2cMapping.Value;
            var location = l2cMapping.Key;
            var resourceChangedModel = cell.GetResourceChangedModel(converter, resourceManager, location);
            broadcast(Recource_Event_Type_Key, resourceChangedModel);
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
