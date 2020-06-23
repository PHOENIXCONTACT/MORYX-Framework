// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Kernel bundle used to flag assemblies with elevated rights for cross bundle access
    /// </summary>
    public class KernelBundleAttribute : BundleAttribute
    {
        /// <summary>
        /// One and only constructor for this attribute
        /// </summary>
        public KernelBundleAttribute() : base("Runtime", RuntimePlatform.RuntimeVersion, true)
        {
        }
    }
}
