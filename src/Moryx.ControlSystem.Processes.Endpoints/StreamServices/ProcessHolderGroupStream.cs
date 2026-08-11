// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Http;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Processes.Endpoints.EventHandlers;

namespace Moryx.ControlSystem.Processes.Endpoints.StreamServices;

/// <summary>
/// Provides the streaming functionality for the process holder Group
/// </summary>
internal class ProcessHolderGroupStream(IResourceManagement resourceManagement)
{
    private static readonly ConcurrentDictionary<Guid, Channel<string>> _subscribers = new();
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    /// Starts the Process Holder Group stream
    /// </summary>
    /// <param name="context">the http context object</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns></returns>
    public async Task Start(HttpContext context, CancellationToken cancellationToken)
    {
        // using unsafe because resourceManagement.GetResources<T> is very restricted in the sense
        // that casting at line 55 doesn't work
        var groups = resourceManagement.GetResourcesUnsafe<IProcessHolderGroup>(_ => true);
        var allPositions = resourceManagement.GetResourcesUnsafe<IProcessHolderPosition>(_ => true).ToArray();

        // Define event handlers using ProcessHolderEventHandlers with broadcast action
        var processChanged = ProcessHolderEventHandlers.OnProcessChanged(Broadcast);
        var groupChanged = ProcessHolderEventHandlers.OnGroupChanged(Broadcast);
        var resourceAdded = ProcessHolderEventHandlers.OnResourceAdded(groupChanged, Broadcast);
        var resourceRemoved = ProcessHolderEventHandlers.OnResourceRemoved(processChanged, groupChanged);
        var resetExecuted = ProcessHolderEventHandlers.OnResetExecuted(Broadcast);

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));

            // Register event handlers after result creation but before execution to ensure finally cleanup
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

            await result.ExecuteAsync(context);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
        }
        finally
        {
            // Unregister handlers
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
        }
    }

    /// <summary>
    /// Local helper to broadcast process holder group changes to all connected clients
    /// </summary>
    private static void Broadcast(ProcessHolderGroupModel groupModel)
    {
        foreach (var channel in _subscribers.Values)
        {
            channel.Writer.TryWrite(JsonSerializer.Serialize(groupModel, _serializerOptions));
        }
    }

    private static async IAsyncEnumerable<string> Subscribe([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var channel = Channel.CreateUnbounded<string>();
        var id = Guid.NewGuid();
        _subscribers[id] = channel;

        try
        {
            await foreach (var data in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return data;
            }
        }
        finally
        {
            _subscribers.TryRemove(id, out _);
        }
    }
}
