// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Payload sent when an event is raised on the resource
/// </summary>
public class ResourceEventPayload
{
    public required ResourceModel Resource { get; set; }
    public required string Event { get; set; }
    public object? EventData { get; set; }
}
