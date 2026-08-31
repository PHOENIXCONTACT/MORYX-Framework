// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Moryx.AspNetCore;

/// <summary>
/// Adds fluent shortcuts for the standard HTTP problem responses (400, 404, 409, 422)
/// on top of a controller's <c>Problem()</c> call. Status code, RFC type URI, title
/// and detail are filled in per method; <c>Instance</c> and <c>traceId</c> remain as
/// set by <see cref="ProblemDetailsExtensions.AddMoryxProblemDetails"/>.
/// </summary>
/// <remarks>
/// Typical usage in a controller:
/// <code>
/// return Problem().NotFound(title, detail);
/// </code>
/// </remarks>
public static class ProblemDetailsFluentExtensions
{
    // RFC 9110 reference URIs used as ProblemDetails.Type
    private const string RfcBadRequest    = "https://www.rfc-editor.org/rfc/rfc9110.html#name-400-bad-request";
    private const string RfcNotFound      = "https://www.rfc-editor.org/rfc/rfc9110.html#name-404-not-found";
    private const string RfcConflict      = "https://www.rfc-editor.org/rfc/rfc9110.html#name-409-conflict";
    private const string RfcUnprocessable = "https://www.rfc-editor.org/rfc/rfc9110.html#name-422-unprocessable-content";

    /// <summary>Turns the result into a 400 Bad Request problem details response.</summary>
    public static ObjectResult BadRequest(this ObjectResult result, string title, string detail)
        => Apply(result, StatusCodes.Status400BadRequest, RfcBadRequest, title, detail);

    /// <summary>Turns the result into a 404 Not Found problem details response.</summary>
    public static ObjectResult NotFound(this ObjectResult result, string title, string detail)
        => Apply(result, StatusCodes.Status404NotFound, RfcNotFound, title, detail);

    /// <summary>Turns the result into a 409 Conflict problem details response.</summary>
    public static ObjectResult Conflict(this ObjectResult result, string title, string detail)
        => Apply(result, StatusCodes.Status409Conflict, RfcConflict, title, detail);

    /// <summary>Turns the result into a 422 Unprocessable Content problem details response.</summary>
    public static ObjectResult UnprocessableContent(this ObjectResult result, string title, string detail)
        => Apply(result, StatusCodes.Status422UnprocessableEntity, RfcUnprocessable, title, detail);

    private static ObjectResult Apply(ObjectResult result, int status, string type, string title, string detail)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.Value is ProblemDetails problem)
        {
            problem.Status = status;
            problem.Type   = type;
            problem.Title  = title;
            problem.Detail = detail;
        }
        result.StatusCode = status;
        return result;
    }
}
