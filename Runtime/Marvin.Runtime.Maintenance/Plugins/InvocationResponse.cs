using System;
using Marvin.Tools;

namespace Marvin.Runtime.Maintenance.Plugins
{
    public class InvocationResponse
    {
        public InvocationResponse()
        {
            Success = true;
        }

        public InvocationResponse(Exception e)
        {
            Success = false;
            ExceptionType = e.GetType().ToString();
            ErrorMessage = ExceptionPrinter.Print(e);
        }

        public InvocationResponse(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; set; }

        public string ExceptionType { get; set; }

        public string ErrorMessage { get; set; }
    }
}
