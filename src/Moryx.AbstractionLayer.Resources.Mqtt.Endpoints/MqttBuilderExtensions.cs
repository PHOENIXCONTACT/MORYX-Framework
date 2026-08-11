// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AspNetCore.Mqtt.Builders;


namespace Moryx.AbstractionLayer.Resources.Mqtt.Endpoints;

/// <summary>
/// Extensions to add Mqtt services to the builder
/// </summary>
public static class MqttBuilderExtensions
{
    /// <param name="builder"></param>
    extension(IMqttBuilder builder)
    {
        /// <summary>
        /// Adds the resource synchronization service to the mqtt builder
        /// </summary>
        /// <returns></returns>
        public IMqttBuilder AddResourceSynchronization()
        {
            builder.AddMqttService<ResourceSynchronizationService>();
            return builder;
        }

        /// <summary>
        /// Adds the resource event service to the mqtt builder
        /// </summary>
        /// <returns></returns>
        public IMqttBuilder AddResourceEvents()
        {
            builder.AddMqttService<ResourceEventService>();
            return builder;
        }
    }
}
