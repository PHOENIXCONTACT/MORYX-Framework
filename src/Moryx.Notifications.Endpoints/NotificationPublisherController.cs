// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moryx.Asp.Extensions;
using Moryx.Notifications.Endpoints.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.Notifications.Endpoints
{
    /// <summary>
    /// Definition of a REST API on the <see cref="INotificationPublisher"/> facade.
    /// </summary>
    [ApiController]
    [Route("api/moryx/notifications/")]
    [Produces("application/json")]
    public class NotificationPublisherController : ControllerBase
    {
        private readonly INotificationPublisher _notificationPublisher;


        private static readonly JsonSerializerSettings _serializerSettings = CreateSerializerSettings();

        private static JsonSerializerSettings CreateSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return serializerSettings;
        }

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
                return NotFound(new MoryxExceptionResponse { Title = Strings.NOTIFICATION_NOT_FOUND });

            return Converter.ToModel(notification);
        }

        private TaskCompletionSource _notificationTcs;

        [HttpGet("stream")]
        [ProducesResponseType(typeof(NotificationModel[]), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.CanView)]
        public async Task NotificationStream(CancellationToken cancelToken)
        {
            var response = Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            // Define event handling
            _notificationTcs = new TaskCompletionSource();
            var eventHandler = new EventHandler<Notification>((sender, notification) => _notificationTcs.TrySetResult());
            // Register handler to facade
            _notificationPublisher.Published += eventHandler;
            _notificationPublisher.Acknowledged += eventHandler;

            try
            {
                // Create infinite loop awaiting changes or cancellation
                while (!cancelToken.IsCancellationRequested)
                {
                    // Write notifications
                    var notifications = _notificationPublisher.GetAll()
                        .Select(Converter.ToModel).ToList();
                    var json = JsonConvert.SerializeObject(notifications, _serializerSettings);

                    await response.WriteAsync($"data: {json}\r\r", cancelToken);

                    // Await task completion
                    await Task.WhenAny(_notificationTcs.Task, Task.Delay(30000, cancelToken));

                    // Create new TCS
                    _notificationTcs = new TaskCompletionSource();
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                // Unregister handler from facade
                _notificationPublisher.Published -= eventHandler;
                _notificationPublisher.Acknowledged -= eventHandler;

                _notificationTcs.TrySetCanceled();
                _notificationTcs = null;
            }

            await response.WriteAsync("retry: 1000\n");
            await response.CompleteAsync();
        }

        [HttpPost("{guid}/acknowledge")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MoryxExceptionResponse), StatusCodes.Status404NotFound)]
        [Authorize(Policy = NotificationPermissions.CanAcknowledge)]
        public ActionResult Acknowledge(Guid guid)
        {
            var notification = _notificationPublisher.Get(guid);
            if (notification == null)
                return NotFound(new MoryxExceptionResponse { Title = Strings.NOTIFICATION_NOT_FOUND });

            _notificationPublisher.Acknowledge(notification);
            return Ok();
        }
    }
}

