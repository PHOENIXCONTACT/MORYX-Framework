// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Simple DTO for a resource
/// </summary>
/// <param name="Id">Identifier of the resource</param>
/// <param name="Name">Name of the Resource</param>
public record ResourceModel(string Identifier, string Name);

