// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.AspNetCore.Mqtt.Endpoints;
using Moryx.AspNetCore.Mqtt.Services;
using Moryx.Tools;

namespace Moryx.AspNetCore.Mqtt.Builders;

/// <summary>
/// Implements the <see cref="IMqttBuilder"/>
/// </summary>
/// <param name="services"></param>
public class MqttBuilder(IServiceCollection services) : IMqttBuilder
{
    // </inherit>
    public IMqttBuilder AddMqttEndpoints(Action<MqttRouteBuilder>? builder = null)
    {
        // find all endpoints via reflection
        var endpoints = ReflectionTool.GetPublicClasses<IMqttEndpoint>() ?? [];
        foreach (var endpoint in endpoints)
        {
            services.AddSingleton(endpoint);
        }

        services.AddHostedService(provider =>
        {
            var hostedClientOption = provider.GetRequiredService<MqttClientUserOptions>();

            var routeBuilder = new MqttRouteBuilder();
            foreach (var endpoint in endpoints)
            {
                var endpointInstance = (IMqttEndpoint)provider.GetRequiredService(endpoint);
                endpointInstance.Map(routeBuilder);
            }

            builder?.Invoke(routeBuilder);
            return
            new MqttEndpointService(
                provider.GetRequiredService<IManagedMqttClient>(),
                routeBuilder,
                hostedClientOption,
                provider.GetRequiredService<ILogger<MqttEndpointService>>()
                );
        });
        return this;
    }

    // </inherit>
    public IMqttBuilder AddMqttService<T>() where T : class, IMqttService
    {
        services.AddHostedService<T>();
        return this;
    }
}
