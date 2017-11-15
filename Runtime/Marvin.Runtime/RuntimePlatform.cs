using System;

namespace Marvin.Runtime
{
    /// <summary>
    /// Platform for all runtime based products
    /// </summary>
    public class RuntimePlatform : Platform
    {
        private string _productName;
        private Version _productVersion;

        /// <summary>
        /// Current version of the MarvonRuntime
        /// </summary>
        public static string RuntimeVersion;

        /// <summary>
        /// Static constructor to read the current RuntimeVersion out of the Assembly
        /// </summary>
        static RuntimePlatform()
        {
            var version = typeof(RuntimePlatform).Assembly.GetName().Version;
            RuntimeVersion = $"{version.Major}.{version.Minor}.{version.Revision}";
        }

        /// <summary>
        /// Set the platform instance to <see cref="RuntimePlatform"/> and provide product information
        /// </summary>
        /// <param name="productName">Name of the product</param>
        /// <param name="productVersion">Version of the product</param>
        public static void SetPlatform(string productName, Version productVersion)
        {
            var instance = new RuntimePlatform
            {
                _productName = productName,
                _productVersion = productVersion
            };
            CurrentPlatform = instance;
        }

        /// <summary>
        /// Type of this platform characterized with enum flags
        /// </summary>
        public override PlatformType Type => PlatformType.Server;

        /// <summary>
        /// Name of this platform
        /// </summary>
        public sealed override string PlatformName => "Runtime";

        /// <summary>
        /// Version of the platform
        /// </summary>
        public sealed override Version PlatformVersion => new Version(RuntimeVersion);

        /// <summary>
        /// Name of the product this application belongs to
        /// </summary>
        public override string ProductName => _productName;

        /// <summary>
        /// Current version of this product
        /// </summary>
        public override Version ProductVersion => _productVersion;
    }
}
