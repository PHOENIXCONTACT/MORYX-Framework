// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.CompilerServices;

// this a Moq specific code 
namespace Moryx.AspNetCore.Mqtt.AttributeRouting;

/// <summary>
/// Contains all the available MqttRoute instances
/// </summary>
internal class MqttRouteTable(MqttRoute[] routes) : IMqttRouteTable
{

    /// <inheritdoc />
    public virtual IReadOnlyList<MqttRoute> Routes { get; } = routes;

    /// <inheritdoc />
    public MqttRoute? MatchingRoute(MqttRouteContext context)
        => Routes.FirstOrDefault(x => x.Match(context.RequestUrl));
}

