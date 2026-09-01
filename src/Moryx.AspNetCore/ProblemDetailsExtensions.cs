// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Moryx.AspNetCore;

/// <summary>
/// MORYX ProblemDetails setup: attaches a correlatable <c>traceId</c> to every
/// problem details response so client errors can be matched to server log entries.
/// </summary>
public static class ProblemDetailsExtensions
{
    /// <summary>Key under which the trace id is stored in <see cref="ProblemDetails.Extensions"/>.</summary>
    public const string TraceIdExtensionKey = "traceId";

    /// <summary>
    /// Registers ProblemDetails and appends the current request's trace id to every
    /// generated problem response.
    /// </summary>
    public static IServiceCollection AddMoryxProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions[TraceIdExtensionKey] = GetTraceId(context.HttpContext);
            };
        });
        return services;
    }

    /// <summary>
    /// Returns the current request's trace id (W3C <see cref="Activity"/>, falling back to
    /// <see cref="HttpContext.TraceIdentifier"/>). Use this in log calls to ensure the id
    /// logged on the server matches the id returned to the client.
    /// </summary>
    public static string GetTraceId(HttpContext httpContext)
    {
        return Activity.Current?.TraceId.ToString()
               ?? httpContext?.TraceIdentifier
               ?? string.Empty;
    }

    /// <summary>
    /// Adds the trace id to a manually constructed <see cref="ProblemDetails"/>.
    /// Required whenever a controller returns problem details via helpers like
    /// <c>NotFound(...)</c>, <c>BadRequest(...)</c> or <c>Conflict(...)</c>
    /// </summary>
    public static ProblemDetails WithTraceId(this ProblemDetails problemDetails, HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(problemDetails);

        problemDetails.Extensions[TraceIdExtensionKey] = GetTraceId(httpContext);
        return problemDetails;
    }
}