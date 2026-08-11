// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using Moryx.AspNetCore.Mqtt.AttributeRouting;
using Moryx.AspNetCore.Mqtt.Endpoints;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Builders;

/// <summary>
/// Defines a contract for an Endpoint in an application. A route builder specifies the possible Mqtt routes for
/// an application.
/// </summary>
public interface IMqttRouteBuilder
{
    /// <summary>
    /// Adds a <see cref="IMqttEndpoint"/> to the <see cref="IMqttRouteBuilder"/> that matches the topic
    /// for the specified pattern.
    /// </summary>
    /// <param name="topicTemplate">The topic pattern.</param>
    /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
    MqttRouteBuilder MapGet([StringSyntax("Route")] string topicTemplate, Func<MqttEndpointContext, MqttApplicationMessage> requestDelegate);

    /// <summary>
    /// Adds a <see cref="IMqttEndpoint"/> to the <see cref="IMqttRouteBuilder"/> . This behave like a stream endpoint.
    /// </summary>
    /// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
    MqttRouteBuilder MapPost(Action<ChannelWriter<MqttApplicationMessage>> requestDelegate);

    /// <summary>
    /// Returns a list that contains all the <see cref="MqttRoute"/>
    /// </summary>
    /// <returns></returns>
    IEnumerable<MqttRoute> Build();
}
