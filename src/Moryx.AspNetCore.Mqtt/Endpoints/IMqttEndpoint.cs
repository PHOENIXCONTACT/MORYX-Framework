// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AspNetCore.Mqtt.Builders;

namespace Moryx.AspNetCore.Mqtt.Endpoints;

/// <summary>
/// Mqtt Endpoint interface. Every endpoint will implement this interface.
/// </summary>
public interface IMqttEndpoint
{
    /// <summary>
    /// Build and map Mqtt endpoint routes with given <paramref name="routeBuilder"/>
    /// </summary>
    /// <param name="routeBuilder">Route routeBuilder</param>
    void Map(IMqttRouteBuilder routeBuilder);
}

