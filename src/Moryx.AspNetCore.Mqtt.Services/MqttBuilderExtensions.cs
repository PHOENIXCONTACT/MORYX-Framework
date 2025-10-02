// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AspNetCore.Mqtt.Builders;

namespace Moryx.AspNetCore.Mqtt.Services;

/// <summary>
/// Extensions to add Mqtt services to the builder
/// </summary>
public static class MqttBuilderExtensions
{

    /// <summary>
    /// Adds the resource synchronization service to the mqtt builder
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMqttBuilder AddResourceSynchronization(this IMqttBuilder builder)
    {
        builder.AddMqttService<ResourceSynchronizationService>();
        return builder;
    }

    /// <summary>
    /// Adds the resource event service to the mqtt builder
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMqttBuilder AddResourceEvents(this IMqttBuilder builder)
    {
        builder.AddMqttService<ResourceEventService>();
        return builder;
    }
}