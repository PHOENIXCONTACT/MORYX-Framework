// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Processes;
using Moryx.Factory;
using Moryx.FactoryMonitor.Endpoints.Model;
using Moryx.FactoryMonitor.Endpoints.Models;
using Moryx.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Timers;

namespace Moryx.FactoryMonitor.Endpoints.Extensions
{
    internal static class FactoryMonitorHelper
    {
        public static async Task OrderStarted(OperationStartedEventArgs orderEventArg, JsonSerializerSettings serializerSettings, Channel<Tuple<string, string>> _factoryChannel, CancellationToken cancelToken)
        {
            var orderModel = Converter.ToOrderModel(orderEventArg.Operation);
            await SendOrderUpdate(orderModel, serializerSettings, _factoryChannel, cancelToken);
        }

        public static async Task OrderUpdated(OperationChangedEventArgs orderEventArg, JsonSerializerSettings serializerSettings, Channel<Tuple<string, string>> _factoryChannel, CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested || orderEventArg.Operation.State is not OperationClassification.Running) return;

            var orderReferenceModel = Converter.ToOrderChangedModel(orderEventArg.Operation);
            await SendOrderUpdate(orderReferenceModel, serializerSettings, _factoryChannel, cancelToken);
        }

        public static async Task SendOrderUpdate(OrderChangedModel orderModel, JsonSerializerSettings serializerSettings, Channel<Tuple<string, string>> _factoryChannel, CancellationToken cancelToken)
        {
            var json = JsonConvert.SerializeObject(orderModel, serializerSettings);
            await _factoryChannel.Writer.WriteAsync(new Tuple<string, string>("processes", json), cancelToken);
        }

        public static async Task PublishCellUpdate(CellStateChangedModel cellModel, JsonSerializerSettings serializerSettings, Channel<Tuple<string, string>> _factoryChannel, CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested) return;
            var json = JsonConvert.SerializeObject(cellModel, serializerSettings);
            await _factoryChannel.Writer.WriteAsync(new Tuple<string, string>("cellStateChangedModel", json), cancelToken);
        }

        public static async Task ActivityUpdated(
            ActivityUpdatedEventArgs activityEventArg,
            JsonSerializerSettings serializerSettings,
            Channel<Tuple<string, string>> _factoryChannel,
            List<ICell> cells,
            Resource resource,
            Converter converter,
            List<OrderModel> orderModels,
            CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested || activityEventArg.Progress == ActivityProgress.Ready) return;

            if (!cells.Any(x => x.Id == activityEventArg.Activity.Tracing.ResourceId)) return;

            var cell = resource;
            var activityChangedModel = (cell as ICell).GetActivityChangedModel(activityEventArg.Activity, orderModels);

            var cellStateChangedModel = (cell as ICell).GetCellStateChangedModel(activityEventArg.Progress, resource);

            var json = JsonConvert.SerializeObject(activityChangedModel, serializerSettings);
            await _factoryChannel.Writer.WriteAsync(new Tuple<string, string>("activityChangedModel", json), cancelToken);

            json = JsonConvert.SerializeObject(cellStateChangedModel, serializerSettings);
            await _factoryChannel.Writer.WriteAsync(new Tuple<string, string>("cellStateChangedModel", json), cancelToken);
        }

        public static async Task ResourceUpdated(
            JsonSerializerSettings serializerSettings,
            Channel<Tuple<string, string>> _factoryChannel,
            IResourceManagement resourceManager,
            Func<IMachineLocation, bool> cellFilter,
            Converter converter,
            CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested) return;

            var cells = resourceManager.GetResources(cellFilter)
                .Select(location => location.Machine)
                .Cast<ICell>();

            foreach (var cell in cells)
                await SendResourceUpdate(cell.GetResourceChangedModel(converter, resourceManager, cellFilter), serializerSettings, _factoryChannel, cancelToken);
        }

        public static async Task SendResourceUpdate(ResourceChangedModel resourceChangedModel,
            JsonSerializerSettings serializerSettings,
            Channel<Tuple<string, string>> _factoryChannel,
            CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested) return;
            var json = JsonConvert.SerializeObject(resourceChangedModel, serializerSettings);
            await _factoryChannel.Writer.WriteAsync(new Tuple<string, string>("resourceChangedModel", json), cancelToken);
        }

        public static List<TransportRouteModel> CreateRoutes(IReadOnlyList<IMachineLocation> locations)
        {
            var routes = new List<TransportRouteModel>();

            foreach (var location in locations)
            {
                var result = location.Destinations.Select(x => new TransportPathModel
                {
                    Destination = Converter.ToCellLocationModel(x.Destination),
                    Origin = Converter.ToCellLocationModel(x.Origin),
                    WayPoints = x.WayPoints
                })?.Select(t => Converter.ToTransportRouteModel(t)).ToList();

                if (result is not null)
                    routes.AddRange(result);
            }
            return routes;
        }
    }
}

