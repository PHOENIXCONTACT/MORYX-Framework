// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Marking;

/// <summary>
/// Exception type which will be used for errors during marking execution of the <see cref="IMarkingLaserDriver"/>
/// </summary>
public class MarkingException : DriverException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkingException"/> class.
    /// </summary>
    public MarkingException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkingException"/> class.
    /// </summary>
    public MarkingException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkingException"/> class.
    /// </summary>
    public MarkingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
