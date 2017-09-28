using System;
using System.Reflection;
using System.Text;

namespace Marvin.Tools
{
    /// <summary>
    /// Helper class to convert an exception to a string
    /// </summary>
    public static class ExceptionPrinter
    {
        /// <summary>
        /// Create a print string from an exception object
        /// </summary>
        public static string Print(Exception ex)
        {
            var message = new StringBuilder();
            var currentEx = ex;
            while (currentEx != null)
            {
                message.AppendLine(currentEx.Message);
                var loadException = currentEx as ReflectionTypeLoadException;
                if (loadException != null && loadException.LoaderExceptions != null)
                    message.AppendLine($"  Loader exceptions: \n  {string.Join<Exception>("\n  ", loadException.LoaderExceptions)}");
                currentEx = currentEx.InnerException;
            }
            message.AppendLine($"Stacktrace: \n {ex.StackTrace}");
            return message.ToString();
        }
    }
}
