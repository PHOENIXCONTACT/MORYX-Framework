// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.Exceptions;

/// <summary>
/// Exception thrown, when a method is not found on a resource
/// </summary>
/// <param name="resource"></param>
/// <param name="method"></param>
public class MethodNotFoundException(string resource, string method) : Exception($"Method '{method}' not found, on the given resource '{resource}'")
{
}
