// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.AspNetCore;
using Moryx.Notifications.Endpoints.Models;
using Moryx.Notifications.Endpoints.Properties;

namespace Moryx.Notifications.Endpoints;

/// <summary>
/// Definition of a REST API on the <see cref="INotificationPublisher"/> facade.
/// </summary>
[ApiController]
[Route("api/moryx/notifications/")]
[Produces("application/json")]
public class NotificationPublisherController : ControllerBase
{
    private readonly INotificationPublisher _notificationPublisher;

    private static readonly ConcurrentDictionary<Guid, Channel<SseItem<string>>> _notificationStreamSubscribers = new();
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public NotificationPublisherController(INotificationPublisher notificationPublisher)
        => _notificationPublisher = notificationPublisher;

    [HttpGet]
    [Authorize(Policy = NotificationPermissions.CanView)]
    public ActionResult<NotificationModel[]> GetAll()
    {
        return _notificationPublisher.GetAll().Select(Converter.ToModel).ToArray();
    }

    [HttpGet("{guid}")]
    [ProducesResponseType(typeof(NotificationModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = NotificationPermissions.CanView)]
    public ActionResult<NotificationModel> Get(Guid guid)
    {
        var notification = _notificationPublisher.Get(guid);
        if (notification == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.NotificationPublisherController_NotificationNotFound });

        return Converter.ToModel(notification);
    }

    [HttpGet("stream")]
    [ProducesResponseType(typeof(NotificationModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = NotificationPermissions.CanView)]
    public async Task NotificationStream(CancellationToken cancellationToken)
    {
        // TODO: do not always broadcast all notifications, separate in event-types
        // https://github.com/PHOENIXCONTACT/MORYX-Framework/issues/1231

        // Define event handlers that broadcast immediately when notifications change
        var publishedEventHandler = new EventHandler<Notification>((_, _) =>
            Broadcast());

        var acknowledgedEventHandler = new EventHandler<Notification>((_, _) =>
            Broadcast());

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));

            // Register event handlers after result creation but before execution to ensure finally cleanup
            _notificationPublisher.Published += publishedEventHandler;
            _notificationPublisher.Acknowledged += acknowledgedEventHandler;

            await result.ExecuteAsync(HttpContext);
        }
        catch (OperationCanceledException)
        {
            // client disconnected — this is expected, not an error
        }
        finally
        {
            _notificationPublisher.Published -= publishedEventHandler;
            _notificationPublisher.Acknowledged -= acknowledgedEventHandler;
        }

        return;

        async IAsyncEnumerable<SseItem<string>> Subscribe([EnumeratorCancellation] CancellationToken cancelToken)
        {
            var channel = Channel.CreateUnbounded<SseItem<string>>();
            var id = Guid.NewGuid();
            _notificationStreamSubscribers[id] = channel;

            // Send all notifications set as first item
            var initialNotifications = _notificationPublisher.GetAll()
                .Select(Converter.ToModel).ToArray();
            yield return new SseItem<string>(JsonSerializer.Serialize(initialNotifications, _serializerOptions));

            try
            {
                await foreach (var data in channel.Reader.ReadAllAsync(cancelToken))
                {
                    yield return data;
                }
            }
            finally
            {
                _notificationStreamSubscribers.TryRemove(id, out _);
            }
        }

        // Local helper to broadcast all notifications to all connected clients
        void Broadcast()
        {
            var notifications = _notificationPublisher.GetAll()
                .Select(Converter.ToModel).ToArray();

            foreach (var channel in _notificationStreamSubscribers.Values)
            {
                channel.Writer.TryWrite(new SseItem<string>(JsonSerializer.Serialize(notifications, _serializerOptions)));
            }
        }
    }

    [HttpPost("{guid}/acknowledge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
    [Authorize(Policy = NotificationPermissions.CanAcknowledge)]
    public ActionResult Acknowledge(Guid guid)
    {
        var notification = _notificationPublisher.Get(guid);
        if (notification == null)
            return NotFound(new MoryxExceptionResponse { Title = Strings.NotificationPublisherController_NotificationNotFound });

        _notificationPublisher.Acknowledge(notification);
        return Ok();
    }
}
