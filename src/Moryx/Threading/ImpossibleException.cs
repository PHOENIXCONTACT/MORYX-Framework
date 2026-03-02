// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Threading;

/// <summary>
/// Describes an impossible exception.
/// </summary>
[Obsolete("Will be removed in the next major")]
public class ImpossibleException : Exception
{
    /// <summary>
    /// Constructor for an impossible exception.
    /// </summary>
    public ImpossibleException()
    {
    }
    /// <summary>
    /// Constructor for an impossible exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ImpossibleException(string message) : base(message)
    {
    }
    /// <summary>
    /// Constructor for an impossible exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">Inner exception if existing.</param>
    public ImpossibleException(string message, Exception inner) : base(message, inner)
    {
    }
}
