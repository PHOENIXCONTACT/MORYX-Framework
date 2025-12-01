// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.Mqtt.Messages;

/// <summary>
/// Interface for messages send via MQTT5. Can be used to get or set the MQTT 5 response topic
/// </summary>
public interface IRespondableMessage
{
    /// <summary>
    /// Topic the response is expected at
    /// </summary>
    public string ResponseIdentifier { get; set; }
}
