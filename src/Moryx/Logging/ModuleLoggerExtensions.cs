// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;

namespace Moryx.Logging;

/// <summary>
/// Provides extension methods for configuring properties on a module logger.
/// </summary>
public static class ModuleLoggerExtensions
{
    /// <summary>
    /// Attempts to add or update a property on the specified moduler logger
    /// </summary>
    /// <param name="logger"> The logger instance.</param>
    /// <param name="key"> The property key.</param>
    /// <param name="value">The property value.</param>
    /// <returns>
    /// /// <see langword="true"/> if the property was set;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TrySetProperty(
    this ILogger logger,
    string key,
    object value)
    {
        if (logger is not ModuleLogger moduleLogger)
        {
            return false;
        }

        moduleLogger.SetProperty(key, value);
        return true;
    }
}
