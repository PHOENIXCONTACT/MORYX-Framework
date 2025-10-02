// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AspNetCore.Mqtt.Exceptions;

/// <summary>
/// Exception thrown, when a method has an unsupported return type
/// </summary>
/// <param name="method">Method name</param>
public class UnsupportedReturnTypeException(string method) : Exception($"Method '{method}' was found, but the return type is unsupported (Task, Task<T>)")
{
}
