// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Moryx.AbstractionLayer.Resources.Endpoints.Properties;
using Moryx.AspNetCore;

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// Provides named problem details responses for the resource endpoints (e.g. resource
/// not found, already exists, reference conflict). Builds on
/// <see cref="ProblemDetailsFluentExtensions"/> to keep <c>Instance</c> and
/// <c>traceId</c> consistent with the rest of the API.
/// </summary>
/// <remarks>
/// Usage:
/// <code>
/// return Problem().ResourceNotFound(id);
/// </code>
/// </remarks>
internal static class ResourceProblemExtensions
{
    /// <summary>404 - No resource exists with the given id.</summary>
    public static ObjectResult ResourceNotFound(this ObjectResult result, long id)
        => result.NotFound(
            Strings.ResourceModificationController_ResourceNotFound_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceNotFound_ById_Message, id));

    /// <summary>404 - The requested resource type is not registered in the type tree.</summary>
    public static ObjectResult ResourceTypeNotFound(this ObjectResult result, string type)
        => result.NotFound(
            Strings.ResourceModificationController_ResourceTypeNotFound_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceTypeNotFound_Message, type));

    /// <summary>404 - The named method does not exist on the resource.</summary>
    public static ObjectResult MethodNotFound(this ObjectResult result, string method, long id)
        => result.NotFound(
            Strings.ResourceModificationController_MethodNotFound_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_MethodNotFound_Message, method, id));

    /// <summary>422 - Method invocation raised an unhandled exception.</summary>
    public static ObjectResult MethodFailed(this ObjectResult result, string method)
        => result.UnprocessableContent(
            Strings.ResourceModificationController_MethodFailed_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_MethodFailed_Message, method));

    /// <summary>409 - Cannot create the resource because a resource with that id already exists.</summary>
    public static ObjectResult ResourceAlreadyExists(this ObjectResult result, long id)
        => result.Conflict(
            Strings.ResourceModificationController_ResourceAlreadyExists_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceAlreadyExists_Message, id));

    /// <summary>409 - Cannot delete the resource because it is still referenced.</summary>
    public static ObjectResult ResourceReferenceConflict(this ObjectResult result, long id)
        => result.Conflict(
            Strings.ResourceModificationController_ResourceConflict_Title,
            string.Format(CultureInfo.CurrentCulture,
                Strings.ResourceModificationController_ResourceConflict_Message, id));

    /// <summary>400 - Generic invalid argument response used for expected validation errors.</summary>
    public static ObjectResult InvalidArgument(this ObjectResult result)
        => result.BadRequest(
            Strings.ResourceModificationController_InvalidArgument_Title,
            Strings.ResourceModificationController_InvalidArgument_Message);
}
