// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net.ServerSentEvents;
using System.Threading.Channels;
using Moryx.Runtime.Modules;
using System.Text.Json;

namespace Moryx.AspNetCore.ServerSentEvents;

/// <summary>
/// Handles lifecycle readiness update for the server sent event stream.
/// </summary>
public sealed class LifeCycleSseHandler : IDisposable
{
    private readonly ILifeCycleBoundFacade _lifeCycleBoundFacade;
    private readonly ChannelWriter<SseItem<string>> _channelWriter;
    private readonly Action _action;
    private readonly JsonSerializerOptions _serializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="LifeCycleSseHandler"/> Class.
    /// </summary>
    /// <param name="facade">
    /// The facade that provide the current lifecycle state and state change events.
    /// </param>
    /// <param name="channelWriter">
    /// The channel writer used to publish readiness messages to the SSE streams.
    /// </param>
    /// <param name="action">
    /// The action invoked when the facade becomes ready, allowing the endpoint to publish its initial data.
    /// </param>
    /// <param name="serializerOptions">
    /// The Options is  used to serialize the readiness message.
    /// </param>
    public LifeCycleSseHandler(ILifeCycleBoundFacade facade, ChannelWriter<SseItem<string>> channelWriter,
        Action action, JsonSerializerOptions serializerOptions)
    {
        _lifeCycleBoundFacade = facade;
        _channelWriter = channelWriter;
        _action = action;
        _serializerOptions = serializerOptions;

        _lifeCycleBoundFacade.StateChanged += OnStateChanged;
    }

    /// <summary>
    /// Get a value indicating whether the facade is curretly ready.
    /// </summary>
    public bool IsReady => _lifeCycleBoundFacade.IsActivated;

    /// <summary>
    /// Create an SSE message containing the current readiness state.
    /// </summary>
    /// <returns>
    /// An SSE item containing the current readiness state.
    /// </returns>
    public SseItem<string> CreateInitialMessage()
    {
        return SseReadinessMessage.Create(_lifeCycleBoundFacade.IsActivated, _serializerOptions);
    }

    private void OnStateChanged(object? sendor, bool isReady)
    {
        _channelWriter.TryWrite(SseReadinessMessage.Create(isReady, _serializerOptions));

        if (isReady)
        {
            _action();
        }
    }

    /// <summary>
    /// Removes the lifecycle state change event subscription.
    /// </summary>
    public void Dispose()
    {
        _lifeCycleBoundFacade.StateChanged -= OnStateChanged;
    }

}
