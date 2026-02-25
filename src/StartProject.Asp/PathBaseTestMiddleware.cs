// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Buffers;
using System.Text;

/// <summary>
/// Middleware for testing purposes.
/// </summary>
/// <param name="requiredPathBase"></param>
public class PathBaseTestMiddleware(string requiredPathBase)
{
    /// <summary>
    /// Middleware that checks if the PathBase was set correctly.
    /// Needs to be placed after the middleware that sets the PathBase,
    /// most likely UsePathBase or the UseForwardedHeaders middleware.
    /// 
    /// If the pathBase is not set or set to the wrong value, this middleware will fail the request
    /// to make problems obvious during tests.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task BlockRequestsWithoutPathBase(HttpContext context, RequestDelegate next)
    {
        var pathBase = context.Request.PathBase;
        if (pathBase == requiredPathBase)
        {
            await next(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.BodyWriter.Write(Encoding.UTF8.GetBytes("Blocked of for test"));
        }
    }
}
