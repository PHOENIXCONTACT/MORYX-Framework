// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Exception thrown when a workplan fails the validation
/// </summary>
public class WorkplanValidationException : Exception
{
    /// <summary>
    /// Errors found during validation
    /// </summary>
    public IReadOnlyList<ValidationError> Errors { get; } = [];

    /// <summary>
    /// Create exception with the default message
    /// </summary>
    public WorkplanValidationException()
        : base(Strings.WorkplanValidationException_Message)
    {
    }

    /// <summary>
    /// Create exception with a custom message
    /// </summary>
    /// <param name="message">Message of the exception</param>
    public WorkplanValidationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Create exception with a custom message and the exception that caused it
    /// </summary>
    /// <param name="message">Message of the exception</param>
    /// <param name="innerException">Exception that caused this exception</param>
    public WorkplanValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Create exception with the errors found during validation
    /// </summary>
    /// <param name="errors">Errors found during validation</param>
    public WorkplanValidationException(IReadOnlyList<ValidationError> errors)
        : this()
    {
        Errors = errors;
    }
}
