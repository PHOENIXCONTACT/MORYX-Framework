// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Startup.Hooks;

public interface IStartupHook
{
    /// <summary>
    /// Lower runs earlier
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Execute the main functionality of the hook
    /// </summary>
    /// <returns></returns>
    Task RunAsync();
}
