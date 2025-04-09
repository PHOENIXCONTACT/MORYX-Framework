using System.Reflection;
using System.Text;

namespace Moryx.Tools
{
    /// <summary>
    /// Helper class to append additional information
    /// </summary>
    public static class ExceptionPrinter
    {
        // TODO: Remove with.NET Core, done in ReflectionTypeLoadException:
        // https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Private.CoreLib/src/System/Reflection/ReflectionTypeLoadException.cs#L49

        /// <summary>
        /// Create a print string from an exception object
        /// </summary>
        public static string Print(Exception ex)
        {
            // Start with standard info
            var message = ex.ToString();
            var builder = new Lazy<StringBuilder>(() => new StringBuilder(message).AppendLine().AppendLine("Additional information:"));

            // Append as much additional as possible
            var currentEx = ex;
            while (currentEx != null)
            {
                // Additional formatter for TypeLoad exceptions
                var loadException = currentEx as ReflectionTypeLoadException;
                foreach (var exception in loadException?.LoaderExceptions ?? Enumerable.Empty<Exception>())
                    builder.Value.AppendLine($"  Loader exception: {exception}");

                // Recursively deeper into the exception
                currentEx = currentEx.InnerException;
            }

            return builder.IsValueCreated ? builder.ToString() : message;
        }
    }
}
