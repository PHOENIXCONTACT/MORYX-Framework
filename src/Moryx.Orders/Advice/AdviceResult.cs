// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Advice;

/// <summary>
/// General advice result.
/// </summary>
public class AdviceResult
{
    /// <summary>
    /// Constructor for an successful result
    /// </summary>
    /// <param name="advice">Original advice</param>
    public AdviceResult(OperationAdvice advice)
    {
        Success = true;
        Advice = advice;
    }

    /// <summary>
    /// Constructor for an erroneous result
    /// </summary>
    /// <param name="advice">Original advice</param>
    /// <param name="error">Error code</param>
    public AdviceResult(OperationAdvice advice, int error) : this(advice)
    {
        Success = false;
        Error = error;
    }

    /// <summary>
    /// Original advice
    /// </summary>
    public OperationAdvice Advice { get; }

    /// <summary>
    /// Indicator if advice was successful
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// If not successful the error code
    /// </summary>
    public int Error { get; }

    /// <summary>
    /// If success or error a message can be attached
    /// </summary>
    public string Message { get; set; }
}