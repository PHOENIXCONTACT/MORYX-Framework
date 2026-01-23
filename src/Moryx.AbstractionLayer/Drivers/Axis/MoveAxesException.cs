// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Axis;

/// <summary>
/// Exception type which will be used for errors during moving axes of <see cref="IAxesController"/>
/// </summary>
public class MoveAxesException : DriverException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoveAxesException"/> class.
    /// </summary>
    public MoveAxesException()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveAxesException"/> class.
    /// </summary>
    public MoveAxesException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveAxesException"/> class.
    /// </summary>
    public MoveAxesException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}
