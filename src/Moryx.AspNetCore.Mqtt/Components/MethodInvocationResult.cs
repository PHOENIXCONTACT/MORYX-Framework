// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Value returned when a Resource method is invoked remotely
/// </summary>
public class MethodInvocationResult
{
    /// <summary>
    /// The returned value of the method/function
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Type of the <see cref="Value"/>
    /// </summary>
    public EntryValueType ValueType { get; set; }
}
