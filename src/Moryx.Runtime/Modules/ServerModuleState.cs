// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules;

/// <summary>
/// Different health states a server module can have
/// </summary>
public enum ServerModuleState
{
    /// <summary>
    /// Initial value
    /// </summary>
    Stopped = 0x0,

    /// <summary>
    /// Module is initializing
    /// </summary>
    Initializing = 0x2,

    /// <summary>
    /// Service is ready to be started
    /// </summary>
    Ready = 0x1,

    /// <summary>
    /// Service is starting
    /// </summary>
    Starting = 0x3,

    /// <summary>
    /// Service is running
    /// </summary>
    Running = 0x8,

    /// <summary>
    /// Service is stopping
    /// </summary>
    Stopping = 0xA,

    /// <summary>
    /// Service failed
    /// </summary>
    Failure = 0x4,

    /// <summary>
    /// Service Missing
    /// </summary>
    Missing = 0x5
}