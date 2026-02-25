// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Buffers;
using System.Text;

/// <summary>
/// Middleware only for testing purposes.
/// </summary>
/// <param name="requiredPathBase"></param>
public class PathBaseTestMiddleware(string requiredPathBase)
{
    public async Task BlockRequestsWithoutPathBase(HttpContext context, RequestDelegate next)
    {
        var pathBase = context.Request.PathBase;
        if (pathBase == requiredPathBase)
        {
            await next(context);
        }
        else
        {
            context.Response.StatusCode = 404;
            context.Response.BodyWriter.Write(Encoding.UTF8.GetBytes("Blocked of for test"));
        }
    }
}