// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

// Further modifications by Phoenix Contact GmbH & Co. KG. 2026
using Microsoft.AspNetCore.Routing;

namespace Moryx.AspNetCore.Mqtt.AttributeRouting;

/// <summary>
/// Represents an instance of a Mqtt route context.
/// </summary>
public sealed class MqttRouteContext(string requestUrl)
{
    /// <summary>
    /// Url of the request
    /// </summary>
    public string RequestUrl { get; } = requestUrl;
    /// <summary>
    /// Route that matches the current request.
    /// </summary>
    public MqttRoute? Route { get; set; }
    /// <summary>
    /// Parameters of the current route
    /// </summary>
    public RouteValueDictionary Parameters { get; set; } = [];
}
