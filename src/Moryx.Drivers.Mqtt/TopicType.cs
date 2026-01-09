// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.Mqtt;

/// <summary>
/// Type of the topic. Describes if messages can be published or received from this topic.
/// </summary>
public enum TopicType
{
    /// <summary>
    /// Topic can publish and receive
    /// </summary>
    BiDirectional,
    /// <summary>
    /// Topic only receives messages
    /// </summary>
    SubscribeOnly,
    /// <summary>
    /// Topic only publishes messages
    /// </summary>
    PublishOnly
}
