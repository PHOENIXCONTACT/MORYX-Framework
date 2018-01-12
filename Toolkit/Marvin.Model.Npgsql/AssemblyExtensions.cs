using System.IO;
using System.Reflection;

namespace Marvin.Model.Npgsql
{
    internal static class AssemblyExtensions
    {
        public static string GetEmbeddedResource(this Assembly assembly, string resourceName)
        {
            var adjustedPath = resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");
            resourceName = $"{assembly.GetName().Name}.{adjustedPath}";

            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;

                using (var reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
