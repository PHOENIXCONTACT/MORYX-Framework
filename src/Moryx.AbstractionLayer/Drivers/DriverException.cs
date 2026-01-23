// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers;

/// <summary>
/// General exception for driver errors
/// </summary>
public class DriverException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DriverException"/> class.
    /// </summary>
    public DriverException()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverException"/> class.
    /// </summary>
    public DriverException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverException"/> class.
    /// </summary>
    public DriverException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}
