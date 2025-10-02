// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.AspNetCore.Mqtt.Components;
namespace Moryx.AspNetCore.Mqtt.Builders;

/// <summary>
/// Adds the <see cref="IManagedMqttClient"/> and related services to the <see cref="IServiceCollection"/>. 
/// </summary>
public interface IMqttBuilder
{
    /// <summary>
    /// Adds an MQTT service <typeparamref name="T"/>
    /// to the <see cref="IServiceCollection"/>
    /// </summary>
    /// <typeparam name="T">The Service type</typeparam>
    /// <returns></returns>
    IMqttBuilder AddMqttService<T>()
        where T : class, IMqttService;

    /// <summary>
    /// Adds <see cref="IMqttEndpoint"/> to the <see cref="IServiceCollection"/>. Allows the usage of MapGet and MapPost
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    IMqttBuilder AddMqttEndpoints(
      Action<MqttRouteBuilder>? builder = null);
}
