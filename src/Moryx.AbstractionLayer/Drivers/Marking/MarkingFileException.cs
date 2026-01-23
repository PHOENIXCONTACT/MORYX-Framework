// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Marking;

/// <summary>
/// Exception type which will be used for errors during setting the marking file of the <see cref="IMarkingLaserDriver"/>
/// </summary>
public class MarkingFileException : DriverException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkingFileException"/> class.
    /// </summary>
    public MarkingFileException()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkingFileException"/> class.
    /// </summary>
    public MarkingFileException(string message)
        : base(message)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkingFileException"/> class.
    /// </summary>
    public MarkingFileException(string message, Exception innerException)
        : base(message, innerException)
    {

    }
}
