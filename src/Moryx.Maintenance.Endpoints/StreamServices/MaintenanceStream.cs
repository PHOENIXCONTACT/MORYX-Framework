// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Moryx.Maintenance.Management.Mappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.Maintenance.Endpoints.StreamServices;

internal sealed class MaintenanceStream(IMaintenanceManagement maintenanceManagement)
{
    private static JsonSerializerSettings CreateSerializerSettings()
    {
        var serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return serializerSettings;
    }

    public async Task Start(
        HttpContext context,
        CancellationToken cancelToken)
    {
        context.Response.Headers.ContentType = "text/event-stream";
        var serializationSettings = CreateSerializerSettings();

        // Define event handling
        var channel = Channel.CreateUnbounded<MaintenanceEventArgs>();
        var maintenanceEvent = MaintenanceEvent(channel);

        maintenanceManagement.MaintenanceStarted += maintenanceEvent;
        maintenanceManagement.MaintenanceOrderAcknowledged += maintenanceEvent;

        try
        {
            // Create infinite loop awaiting changes or cancellation
            while (!cancelToken.IsCancellationRequested)
            {
                // Write
                var changeArgs = await channel.Reader.ReadAsync(cancelToken);
                if (changeArgs != null)
                {
                    var json = JsonConvert.SerializeObject(changeArgs.MaintenanceOrder, serializationSettings);
                    await context.Response.WriteAsync($"data: {json}\r\r", cancelToken);
                }
            }
        }
        finally
        {
            maintenanceManagement.MaintenanceOrderAcknowledged -= maintenanceEvent;
            maintenanceManagement.MaintenanceStarted -= maintenanceEvent;

            channel.Writer.TryComplete();
        }
        await context.Response.CompleteAsync();
    }

    private static EventHandler<MaintenanceOrder> MaintenanceEvent(
        Channel<MaintenanceEventArgs> groupChannel)
      => new((obj, e) =>
      {
          groupChannel.Writer.TryWrite(new MaintenanceEventArgs(e.ToDto()));
      });

}
