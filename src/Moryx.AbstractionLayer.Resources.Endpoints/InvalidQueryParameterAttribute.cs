// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Moryx.AbstractionLayer.Resources.Endpoints;

/// <summary>
/// The attribute controls the selection of an action method based on the existance of a query parameter <paramref name="paramName"/>.
/// </summary>
/// <param name="paramName">The query parameter to mark this method as invalid for selection</param>
public class InvalidQueryParameterAttribute(string paramName) : ActionMethodSelectorAttribute
{
    /// <inheritdoc />
    public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action) =>
        !routeContext.HttpContext.Request.Query.ContainsKey(paramName);
}
