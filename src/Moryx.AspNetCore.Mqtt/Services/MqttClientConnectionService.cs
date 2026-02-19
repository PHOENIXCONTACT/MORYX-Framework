// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Hosting;
using Moryx.AspNetCore.Mqtt.Components;
using MQTTnet;

namespace Moryx.AspNetCore.Mqtt.Services;

/// <summary>
/// Handle all the <see cref="IMqttClient"/> connection logic
/// </summary>
internal sealed class MqttClientConnectionService : IHostedService
{
    private readonly MqttClientOptions _options;
    private readonly IManagedMqttClient _client;

    public MqttClientConnectionService(MqttClientUserOptions options, IManagedMqttClient client)
    {
        var clientOptionBuilder = new MqttClientOptionsBuilder();
        clientOptionBuilder.WithConnectionConfig(options.Connection);
        _options = clientOptionBuilder.Build();
        _client = client;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => _client.StartAsync(_options, cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken)
        => _client.StopAsync(cancellationToken);
}
