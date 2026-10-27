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
    private ILifeCycleBoundFacade _facade;
    private ChannelWriter<SseItem<string>> _channelWriter;
    private Action _onReady;
    private JsonSerializerOptions _serializerOptions;
    private readonly bool _treatNotLifeCycleBoundFacadeAsReady;

    /// <summary>
    /// Initializes a new instance of the <see cref="LifeCycleSseHandler"/> Class.
    /// </summary>
    /// <param name="facade">
    /// The facade that provide the current lifecycle state and state change events.
    /// </param>
    /// <param name="channelWriter">
    /// The channel writer used to publish readiness messages to the SSE streams.
    /// </param>
    /// <param name="onReady">
    /// The action invoked when the facade becomes ready, allowing the endpoint to publish its initial data.
    /// </param>
    /// <param name="serializerOptions">
    /// The Options is  used to serialize the readiness message.
    /// </param>
    /// <param name="treatNotLifeCycleBoundFacadeAsReady">
    /// Compatibility mode for implementations of a facade that may not implment ILifeCycleBoundFacade and may thus be null
    /// </param>
    public LifeCycleSseHandler(ILifeCycleBoundFacade facade, ChannelWriter<SseItem<string>> channelWriter,
        Action onReady, JsonSerializerOptions serializerOptions, bool treatNotLifeCycleBoundFacadeAsReady = true)
    {
        ArgumentNullException.ThrowIfNull(channelWriter);
        ArgumentNullException.ThrowIfNull(onReady);
        ArgumentNullException.ThrowIfNull(serializerOptions);

        _facade = facade;
        _channelWriter = channelWriter;
        _onReady = onReady;
        _serializerOptions = serializerOptions;
        _treatNotLifeCycleBoundFacadeAsReady = treatNotLifeCycleBoundFacadeAsReady;
    }

    /// <summary>
    /// Starts the sse handler
    /// </summary>
    public void Start()
    {
        _facade?.StateChanged += OnStateChanged;
        OnStateChanged(_facade, _facade?.IsActivated ?? _treatNotLifeCycleBoundFacadeAsReady);
    }

    /// <summary>
    /// Get a value indicating whether the facade is curretly ready.
    /// </summary>
    public bool IsReady => _facade?.IsActivated ?? _treatNotLifeCycleBoundFacadeAsReady;

    private void OnStateChanged(object sender, bool isReady)
    {
        _channelWriter.TryWrite(SseReadinessMessage.Create(isReady, _serializerOptions));

        if (isReady)
        {
            _onReady();
        }
    }

    /// <summary>
    /// Removes the lifecycle state change event subscription.
    /// </summary>
    public void Dispose()
    {
        _facade?.StateChanged -= OnStateChanged;
        _facade = null;
        _onReady = null;
        _channelWriter = null;
        _serializerOptions = null;
    }

}
