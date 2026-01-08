// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Moryx.AspNetCore;

public class MoryxExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var headers = string.Join(" \r\n ", context.HttpContext.Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
        context.Result = new ObjectResult(new MoryxExceptionResponse
        {
            Title = "500 - Internal Server Error",
            Exception = context.Exception.ToString() + headers
        });
    }
}