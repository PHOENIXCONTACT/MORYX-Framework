// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;

namespace Moryx.Logging;

/// <summary>
/// Logger instance used within one module. All entries will be logged in the modules context.
/// </summary>
public interface IModuleLogger : ILogger, INamedChildContainer<IModuleLogger>
{
}