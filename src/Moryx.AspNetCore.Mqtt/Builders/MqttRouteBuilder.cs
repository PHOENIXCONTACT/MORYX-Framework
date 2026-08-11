// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using Microsoft.AspNetCore.Routing.Template;
using Moryx.AspNetCore.Mqtt.AttributeRouting;
using Moryx.AspNetCore.Mqtt.Endpoints;
using MQTTnet;
namespace Moryx.AspNetCore.Mqtt.Builders;

/// <summary>
/// Implementation of the <see cref="IMqttRouteBuilder"/>
/// </summary>
public class MqttRouteBuilder : IMqttRouteBuilder
{
    private readonly List<MqttRoute> _routes = [];

    //<inherit>
    public MqttRouteBuilder MapGet([StringSyntax("Route")] string topicTemplate, Func<MqttEndpointContext, MqttApplicationMessage> requestDelegate)
    {
        _routes.Add(new MqttRoute(TemplateParser.Parse(topicTemplate), requestDelegate));
        return this;
    }

    //<inherit>
    public MqttRouteBuilder MapPost(Action<ChannelWriter<MqttApplicationMessage>> requestDelegate)
    {
        _routes.Add(new MqttRoute(TemplateParser.Parse(string.Empty), requestDelegate));
        return this;
    }

    //<inherit>
    public IEnumerable<MqttRoute> Build()
        => _routes;
}
