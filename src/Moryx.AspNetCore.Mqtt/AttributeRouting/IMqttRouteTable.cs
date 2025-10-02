// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.AttributeRouting;

/// <summary>
/// Contains all the available Mqtt routes
/// </summary>
public interface IMqttRouteTable
{
    /// <summary>
    /// All available routes
    /// </summary>
    IReadOnlyList<MqttRoute> Routes { get; }

    /// <summary>
    /// Given a <paramref name="routeContext"/>, looks for the <see cref="MqttRoute"/> matching the context.
    /// </summary>
    MqttRoute? MatchingRoute(MqttRouteContext routeContext);
}
