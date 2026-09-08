// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Moryx.Logging;

public static class ModuleLoggerExtensions
{
    public static bool TrySetProperty(
    this ILogger logger,
    string key,
    object? value)
    {
        if (logger is not ModuleLogger moduleLogger)
        {
            return false;
        }
        moduleLogger.SetProperty(key, value);
        return true;
    }
    public static bool TryRemoveProperty(
    this ILogger logger,
    string key)
    {
        return logger is ModuleLogger moduleLogger &&
        moduleLogger.RemoveProperty(key);
    }

}
