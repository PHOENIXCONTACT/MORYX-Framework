// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
using Moryx.Runtime.Modules;

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
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.NotificationPublisherController_NotificationNotFound });
        }

        return Converter.ToModel(notification);
    }

    [HttpGet("stream")]
    [ProducesResponseType(typeof(NotificationModel[]), StatusCodes.Status200OK)]
    [Authorize(Policy = NotificationPermissions.CanView)]
    public async Task NotificationStream(CancellationToken cancellationToken)
    {
        // TODO: do not always broadcast all notifications, separate in event-types
        // https://github.com/PHOENIXCONTACT/MORYX-Framework/issues/1231

        var channel = Channel.CreateUnbounded<SseItem<string>>();

        // Define event handlers that broadcast immediately when notifications change
        EventHandler<Notification> publishedEventHandler = (_, _) =>
            Broadcast();

        EventHandler<Notification> acknowledgedEventHandler = (_, _) =>
            Broadcast();

        EventHandler<bool> stateChangedEventHandler = (args, ready) =>
        {
            if (ready)
            {
                Broadcast();
            }
        };

        try
        {
            var result = TypedResults.ServerSentEvents(Subscribe(cancellationToken));

            // Register event handlers after result creation but before execution to ensure finally cleanup
            _notificationPublisher.Published += publishedEventHandler;
            _notificationPublisher.Acknowledged += acknowledgedEventHandler;
            if (_notificationPublisher is ILifeCycleBoundFacade lf)
            {
                lf.StateChanged += stateChangedEventHandler;
            }

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
            if (_notificationPublisher is ILifeCycleBoundFacade lf)
            {
                lf.StateChanged -= stateChangedEventHandler;
            }
        }

        return;

        async IAsyncEnumerable<SseItem<string>> Subscribe([EnumeratorCancellation] CancellationToken cancelToken)
        {
            // Send all notifications set as first item
            NotificationModel[] initialNotifications = [];

            try
            {
                initialNotifications = _notificationPublisher.GetAll()
                .Select(Converter.ToModel).ToArray();
            }
            catch (HealthStateException)
            {
                // Ignore if module is not ready yet.
            }
            yield return new SseItem<string>(JsonSerializer.Serialize(initialNotifications, _serializerOptions));

            await foreach (var data in channel.Reader.ReadAllAsync(cancelToken))
            {
                yield return data;
            }
        }

        // Local helper to broadcast all notifications to all connected clients
        void Broadcast()
        {
            var notifications = _notificationPublisher.GetAll()
                .Select(Converter.ToModel).ToArray();

            channel.Writer.TryWrite(new SseItem<string>(JsonSerializer.Serialize(notifications, _serializerOptions)));
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
        {
            return NotFound(new MoryxExceptionResponse { Title = Strings.NotificationPublisherController_NotificationNotFound });
        }

        _notificationPublisher.Acknowledge(notification);
        return Ok();
    }
}
