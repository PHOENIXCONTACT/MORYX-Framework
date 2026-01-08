// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Resources.Benchmarking.Messages;

/// <summary>
/// Message specific to notify arrival of a piece in the cell
/// </summary>
public class WorkpieceArrivedMessage
{
    /// <summary>
    /// Id of the process that will be started in the cell
    /// </summary>
    public long ProcessId { get; set; }
}