// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.AspNetCore.Mqtt.Builders;
using Moryx.AspNetCore.Mqtt.Components;
using Moryx.AspNetCore.Mqtt.Services;
using Moryx.Configuration;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;

namespace Moryx.AspNetCore.Mqtt;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods to add and configure <see cref="IManagedMqttClient"/>
/// </summary>
public static class ServiceCollectionExtension
{
    /// <param name="services">The service collection</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the <see cref="IManagedMqttClient"/> to the current application
        /// </summary>
        /// <param name="builder">The option builder for the <see cref="IManagedMqttClient"/></param>
        /// <returns>Return the <see cref="IMqttBuilder"/> for chained configuration</returns>
        public IMqttBuilder AddMqttClient(Action<IServiceProvider, MqttClientUserOptions> builder)
        {
            services.AddTransient<IMqttNetLogger, MqttClientLogger>();
            services.AddTransient(provider =>
            {
                var options = new MqttClientUserOptions();
                var configManager = provider.GetRequiredService<IConfigManager>();
                var config = configManager.GetConfiguration<MqttConnectionConfig>();
                // If configuration is generated, save it back to persist defaults
                if (config.ConfigState == ConfigState.Generated)
                {
                    config.ConfigState = ConfigState.Valid;
                    configManager.SaveConfiguration(config);
                }
                options.Connection = config;
                builder.Invoke(provider, options);
                return options;
            });

            return CreateBuilder(services);
        }

        /// <summary>
        /// Adds the <see cref="IManagedMqttClient"/> to the current application
        /// </summary>
        /// <returns>Return the <see cref="IMqttBuilder"/> for chained configuration</returns>
        public IMqttBuilder AddMqttClient()
        {
            return AddMqttClient(services, (_, opt) => { });
        }
    }

    private static MqttBuilder CreateBuilder(IServiceCollection services)
    {
        services.AddSingleton<IManagedMqttClient, ManagedMqttClient>(provider =>
        {
            var userOptions = provider.GetRequiredService<MqttClientUserOptions>();
            var logger = provider.GetRequiredService<IMqttNetLogger>();
            var client = new MqttClientFactory(logger).CreateMqttClient(logger);
            return new ManagedMqttClient(client, logger, userOptions);
        });

        services.AddHostedService<MqttClientConnectionService>();
        var mqttBuilder = new MqttBuilder(services);
        return mqttBuilder;
    }
}
