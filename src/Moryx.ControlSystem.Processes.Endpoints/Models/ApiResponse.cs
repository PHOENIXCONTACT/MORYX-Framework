// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.Processes.Endpoints;

/// <summary>
/// Decorates the API response
/// </summary>
public class ApiResponse<T>(T data) where T : notnull
{
    public ApiResponse(T data, ApiError[] errors) : this(data)
    {
        Errors = errors;
    }

    /// <summary>
    /// The response to send to the client
    /// </summary>
    public T Data { get; } = data;

    /// <summary>
    /// Errors during the processing of the request
    /// </summary>
    public ApiError[] Errors { get; } = [];

    /// <summary>
    /// Returns the status of the request. True if the request was successful.
    /// </summary>
    public bool Succeeded => Errors.Length > 0;

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; } = DateTime.Now;
}

/// <summary>
/// Describes an error that was raised in the backend
/// </summary>
/// <param name="ErrorCode"></param>
/// <param name="Message"></param>
public record ApiError(string ErrorCode, string Message, Exception Exception = null);
