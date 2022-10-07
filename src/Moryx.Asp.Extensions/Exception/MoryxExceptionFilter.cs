using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Moryx.Asp.Extensions.Exception
{
    public class MoryxExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            string headers = string.Join(" \r\n ", context.HttpContext.Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
            context.Result = new ObjectResult(new MoryxExceptionResponse
            {
                Title = "500 - Internal Server Error",
                Exception = context.Exception.ToString() + headers
            });
        }
    }
}
