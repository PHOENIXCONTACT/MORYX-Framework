// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;

namespace Moryx
{
    /// <summary>
    /// This class provides access to the product this application belongs to and the environment it was build on.
    /// </summary>
    public class Platform
    {
        /// <summary>
        /// Platform the current application was build on.
        /// </summary>
        public static Platform Current { get; protected set; }

        /// <summary>
        /// Name of this platform
        /// </summary>
        public string PlatformName { get; protected set; }

        /// <summary>
        /// Version of the platform
        /// </summary>
        public Version PlatformVersion { get; protected set; }

        /// <summary>
        /// Name of the product this application belongs to
        /// </summary>
        public string ProductName { get; protected set; }

        /// <summary>
        /// Current version of this product
        /// </summary>
        public Version ProductVersion { get; protected set; }

        /// <summary>
        /// Description of the MORYX based product
        /// </summary>
        public string ProductDescription { get; protected set; }

        /// <summary>
        /// Static constructor to read the current RuntimeVersion out of the Assembly
        /// </summary>
        private Platform()
        {
        }

        /// <summary>
        /// Set the <see cref="Platform"/> instance to provide product information
        /// </summary>
        public static void SetPlatform()
        {
            var startAssembly = Assembly.GetEntryAssembly();
            Current = new Platform
            {
                PlatformName = startAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "MORYX Framework",
                PlatformVersion = typeof(Platform).Assembly.GetName().Version,
                ProductName = startAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "MORYX Application",
                ProductVersion = new Version(startAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "1.0.0"),
                ProductDescription = startAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "No Description provided!",
            };
        }
    }
}
