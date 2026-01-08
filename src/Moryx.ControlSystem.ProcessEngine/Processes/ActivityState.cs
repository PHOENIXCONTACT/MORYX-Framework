// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Processes;

/// <summary>
/// The state of an <c>Activity</c>
/// </summary>
[Flags]
public enum ActivityState
{
    /// <summary>
    /// Initial state of an Activity.
    /// </summary>
    Initial = 0,

    /// <summary>
    /// This state is set after the <c>ActivityAdded</c> event was fired. 
    /// It is assumed that all conditions the Activity depends on are added to the Activity now.
    /// </summary>
    Configured = 64,

    /// <summary>
    /// Activity was aborted before it was started
    /// </summary>
    Aborted = 128,

    /// <summary>
    /// The Activity is currently running.
    /// </summary>
    Running = 256,

    /// <summary>
    /// Received the activities result and start post-processing
    /// </summary>
    ResultReceived = 512,

    /// <summary>
    /// Result was processed for activity and product instance changes
    /// </summary>
    ResultProcessed = 1024,

    /// <summary>
    /// Result was fed back into the engine and the workflow proceeded
    /// </summary>
    EngineProceeded = 2048,

    /// <summary>
    /// The Activity is finished and its <c>Result</c> is set.
    /// </summary>
    Completed = 8192,
}