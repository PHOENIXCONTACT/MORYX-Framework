// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AspNetCore.Mqtt.Components;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Extensions;

/// <summary>
///<see cref="MqttClientOptionsBuilder"/> extension method to configure the <see cref="IMqttClient"/>
/// </summary>
public static class MqttClientOptionsBuilderExtension
{
    /// <summary>
    /// Given a <paramref name="config"/> returns a configured <see cref="MqttApplicationMessageBuilder"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static MqttClientOptionsBuilder WithConnectionConfig(this MqttClientOptionsBuilder builder, MqttConnectionConfig config)
    {
        builder.WithClientId(config.Id)
                .WithTlsOptions(new MqttClientTlsOptions { UseTls = config.Tls })
                .WithTcpServer(config.Host, config.Port)
                .WithCleanSession(config.ReconnectWithClientSession)
                .WithWillQualityOfServiceLevel(config.QoS)
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500);

        if (string.IsNullOrEmpty(config.Username) || string.IsNullOrEmpty(config.Password))
        {
            return builder;
        }

        builder.WithCredentials(config.Username, config.Password);
        return builder;
    }
}
