// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers;

/// <summary>
/// Exception for busy drivers. The driver cannot handle requests.
/// </summary>
public class DriverStateException : Exception
{
    /// <summary>
    /// Information about the required state of the driver for the execution
    /// </summary>
    public StateClassification RequiredState { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverStateException"/> class.
    /// </summary>
    public DriverStateException(StateClassification requiredState)
        : base($"Cannot handle the driver request. Driver is not in state {requiredState}!")
    {
        RequiredState = requiredState;
    }
}