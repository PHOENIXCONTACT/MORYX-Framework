
// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Moryx.AspNetCore.Mqtt.AttributeRouting;

/// <summary>
/// Represents an instance of an Mqtt route. 
/// </summary>
/// <param name="template">Route template</param>
/// <param name="handlerMethod">Handler Method for the route</param>
public sealed class MqttRoute(RouteTemplate template, Delegate handlerMethod)
{
    /// <summary>
    /// Template of the current route
    /// </summary>
    public RouteTemplate Template => template;

    /// <summary>
    /// Handle method for the current route
    /// </summary>
    public Delegate HandlerMethod => handlerMethod;

    /// <summary>
    /// Given a <paramref name="requestUrl"/>, checks if the context route matches the current route.
    /// </summary>
    /// <param name="requestUrl"></param>
    /// <returns></returns>
    public bool Match(string requestUrl)
    {
        var defaults = new RouteValueDictionary();
        var matcher = new TemplateMatcher(template, defaults);
        return matcher.TryMatch(requestUrl, defaults);
    }
}

