// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Processes;

/// <summary>
/// The failure predictor published the expected outcome of a process before
/// it is finished
/// </summary>
internal interface IFailurePredictor : IActivityPoolListener
{
    /// <summary>
    /// Event raised when a process will fail in the near future
    /// </summary>
    event EventHandler<ProcessData> ProcessWillFail;
}