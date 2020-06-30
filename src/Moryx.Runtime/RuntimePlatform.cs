// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;

namespace Moryx.Runtime
{
    /// <summary>
    /// Platform for all runtime based products
    /// </summary>
    public class RuntimePlatform : Platform
    {
        /// <summary>
        /// Type of this platform characterized with enum flags
        /// </summary>
        public override PlatformType Type => PlatformType.Server;

        /// <summary>
        /// Static constructor to read the current RuntimeVersion out of the Assembly
        /// </summary>
        private RuntimePlatform()
        {
        }

        /// <summary>
        /// Set the platform instance to <see cref="RuntimePlatform"/> and provide product information
        /// </summary>
        public static void SetPlatform()
        {
            var startAssembly = Assembly.GetEntryAssembly();
            Current = new RuntimePlatform
            {
                PlatformName = startAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Runtime",
                PlatformVersion = typeof(RuntimePlatform).Assembly.GetName().Version,
                ProductName = startAssembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "MORYX Application",
                ProductVersion = new Version(startAssembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "1.0.0"),
                ProductDescription = startAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "No Description provided!",
            };
        }

        /// <summary>
        /// Current version string of the runtime platform
        /// </summary>
        public static string RuntimeVersion => Current.PlatformVersion.ToString(3);
    }
}
