// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Processes.Endpoints.EventHandlers;
using Newtonsoft.Json;

namespace Moryx.ControlSystem.Processes.Endpoints.StreamServices;

/// <summary>
/// Provides the streaming functionality for the process holder Group
/// </summary>
/// <param name="resourceManagement"></param>
/// <param name="_serializerSettings"></param>
internal class ProcessHolderGroupStream(
    IResourceManagement resourceManagement,
    JsonSerializerSettings _serializerSettings)
{
    /// <summary>
    /// Starts the Process Holder Group stream
    /// </summary>
    /// <param name="response">the http response object</param>
    /// <param name="cancelToken"></param>
    /// <returns></returns>
    public async Task Start(HttpContext context, CancellationToken cancelToken)
    {
        context.Response.Headers.ContentType = "text/event-stream";

        //using this because resourceManagement.GetResources<T> is very restricted in the sense
        // that casting at line 55 doesn't work
        var groups = resourceManagement.GetAllResources<IProcessHolderGroup>(_ => true);
        var allPositions = resourceManagement.GetAllResources<IProcessHolderPosition>(_ => true);

        // Define event handling
        var groupChannel = Channel.CreateUnbounded<ProcessHolderGroupChangedEventArg>();
        var processChanged = ProcessHolderEventHandlers.OnProcessChanged(groupChannel);
        var groupChanged = ProcessHolderEventHandlers.OnGroupChanged(groupChannel);
        var resourceAdded = ProcessHolderEventHandlers.OnResourceAdded(groupChannel, groupChanged);
        var resourceRemoved = ProcessHolderEventHandlers.OnResourceRemoved(processChanged, groupChanged);
        var resetExecuted = ProcessHolderEventHandlers.OnResetExecuted(groupChannel);

        foreach (var position in allPositions)
        {
            position.ProcessChanged += processChanged;
            position.ResetExecuted += resetExecuted;
        }

        foreach (var group in groups)
        {
            (group as ProcessHolderGroup).Changed += groupChanged;
        }

        resourceManagement.ResourceAdded += resourceAdded;
        resourceManagement.ResourceRemoved += resourceRemoved;
        try
        {
            // Create infinite loop awaiting changes or cancellation
            while (!cancelToken.IsCancellationRequested)
            {
                // Write groups
                var groupChangeArgs = await groupChannel.Reader.ReadAsync(cancelToken);
                if (groupChangeArgs != null)
                {
                    var json = JsonConvert.SerializeObject(groupChangeArgs.Group, _serializerSettings);
                    await context.Response.WriteAsync($"data: {json}\r\r", cancelToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ChannelClosedException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        finally
        {
            foreach (var position in allPositions)
            {
                position.ProcessChanged -= processChanged;
                position.ResetExecuted -= resetExecuted;
            }
            foreach (var group in groups)
            {
                (group as ProcessHolderGroup).Changed -= groupChanged;
            }
            resourceManagement.ResourceAdded -= resourceAdded;
            resourceManagement.ResourceRemoved -= resourceRemoved;

            groupChannel.Writer.TryComplete();
        }
        await context.Response.CompleteAsync();
    }
}
