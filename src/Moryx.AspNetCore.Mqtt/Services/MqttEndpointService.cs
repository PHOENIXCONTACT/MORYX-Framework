// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Channels;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.AspNetCore.Mqtt.AttributeRouting;
using Moryx.AspNetCore.Mqtt.Builders;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.AspNetCore.Mqtt.Endpoints;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Services;

/// <summary>
/// Handle all the wiring and running of the <see cref="IMqttEndpoint"/>
/// </summary>
/// <param name="client">The <see cref="IManagedMqttClient"/> to use</param>
/// <param name="builder">The route builder </param>
/// <param name="userOptions">options for the <see cref="MqttClientUserOptions"/></param>
internal sealed class MqttEndpointService(IManagedMqttClient client, MqttRouteBuilder builder, MqttClientUserOptions userOptions, ILogger logger
    ) : IHostedService
{
    #region Properties
    IMqttRouteTable? _routeTable;
    private bool _running;
    readonly Channel<MqttApplicationMessage> _endpointMessageChannel =
        Channel.CreateUnbounded<MqttApplicationMessage>();
    readonly CancellationTokenSource _cancellationTokenSource = new();

    public IManagedMqttClient Client { get; }
        = client
          ?? throw new ArgumentNullException("No 'IHostedMqttClient' found. Make sure your added 'services.AddMqttClient()' to your program.cs");
    #endregion

    #region Hosted Service

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SetupEndpoints();
        client.ConnectedAsync += ClientConnectedAsync;
        client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
        if (client.IsConnected && !_running)
        {
            ListenToMessageChannel();
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        client.ConnectedAsync -= ClientConnectedAsync;
        client.ApplicationMessageReceivedAsync -= OnMessageReceivedAsync;
        _cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
    #endregion

    private void SetupEndpoints()
    {
        if (builder is null)
        {
            logger.LogInformation("No MQTT Endpoint found!");
            return;
        }

        var routes = builder.Build();
        if (!string.IsNullOrEmpty(userOptions.Connection.RootTopic))
        {
            try
            {
                // making sure the root topic is valid
                TemplateParser.Parse(userOptions.Connection.RootTopic);
                routes = routes.Select(x =>
                new MqttRoute(TemplateParser.Parse(string.Join('/', userOptions.Connection.RootTopic, x.Template.TemplateText)), x.HandlerMethod));
            }
            catch (Exception ex)
            {
                logger.LogError("The given RootTopic '{topic}' from your connection config is invalid. Error: '{message}'", userOptions.Connection.RootTopic, ex.Message);
            }
        }
        _routeTable = new MqttRouteTable([.. routes]);
        StartStreamEndpoints();
    }

    private void StartStreamEndpoints()
    {
        var streamRoutes = _routeTable?.Routes.Where(x => string.IsNullOrEmpty(x.Template.TemplateText)) ?? [];
        if (!streamRoutes.Any())
        {
            return;
        }

        Parallel.ForEach(streamRoutes, (streamRoute, index) =>
        {
            try
            {
                streamRoute.HandlerMethod.DynamicInvoke(_endpointMessageChannel.Writer);
            }
            catch (Exception e)
            {
                logger.LogError("MqttEndpoint error while starting the Stream endpoint. Error:{error}",
                e.Message);
            }
        });
    }

    private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var message = arg.ApplicationMessage;
        var request = new MqttRouteContext(message.Topic);
        var topic = arg.ApplicationMessage.Topic.StartsWith('/')
            ? arg.ApplicationMessage.Topic :
            "/" + arg.ApplicationMessage.Topic;
        var route = _routeTable?.Routes.FirstOrDefault(x => x.Match(topic));
        if (route is not null)
        {
            var currentRoute = new MqttRoute(route.Template, route.HandlerMethod);
            request.Route = currentRoute;
            var matcher = new TemplateMatcher(route.Template, request.Parameters);
            matcher.TryMatch(topic, request.Parameters);

            // invoke user defined function
            var endpointContext = new MqttEndpointContext(
                message,
                request.Parameters,
                DeserializeAsync);
            var @delegate = route.HandlerMethod;
            var requiredParameterType = @delegate.Method.GetParameters().FirstOrDefault()?.ParameterType;
            if (requiredParameterType == typeof(MqttEndpointContext))
            {
                var response = (MqttApplicationMessage?)@delegate?.DynamicInvoke(endpointContext);
                if (response is not null)
                {
                    var rootTopic = userOptions.Connection.RootTopic;
                    return Client.EnqueueAsync(response, CancellationToken.None);
                }
            }
            else if (requiredParameterType == typeof(ChannelWriter<MqttApplicationMessage>))
            {
                @delegate?.DynamicInvoke(endpointContext);
            }
        }

        return Task.CompletedTask;
    }

    private async ValueTask<object?> DeserializeAsync(
        MqttApplicationMessage message,
        CancellationToken token)
        => await MqttMessageSerialization.DeserializeAsync(message, userOptions.JsonSerializerOptions, token);

    private async Task ClientConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        await SubscribeToTopic();
        ListenToMessageChannel();
    }

    private void ListenToMessageChannel()
    {
        //background activity
        _ = Task.Run(async () =>
        {
            _running = true;
            while (Client.IsConnected
            && _running
            && !_cancellationTokenSource.IsCancellationRequested)
            {
                var messageToSend = await _endpointMessageChannel.Reader.ReadAsync();
                if (messageToSend is not null)
                {
                    await Client.EnqueueAsync(messageToSend, CancellationToken.None);
                }
            }
            _running = false;
        });
    }

    private async Task SubscribeToTopic()
    {
        var topicToSubscribeTo = userOptions.Connection.RootTopic.EndsWith('/')
            || string.IsNullOrEmpty(userOptions.Connection.RootTopic)
            ? userOptions.Connection.RootTopic + "#"
            : userOptions.Connection.RootTopic + "/#";
        try
        {
            await client.SubscribeAsync([new MqttTopicFilterBuilder().WithTopic(topicToSubscribeTo).Build()], CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError("Mqtt client '{id}' error while subscribing to '{topic}'. Error:{error}",
                userOptions.Connection.Id,
                topicToSubscribeTo,
                e.Message);
        }
    }
}
