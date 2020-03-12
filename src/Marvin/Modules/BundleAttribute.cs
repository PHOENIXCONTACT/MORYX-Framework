// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Modules
{
    /// <summary>
    /// Attribute to flag an assembly as part of a bundle
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BundleAttribute : Attribute
    {
        /// <summary>
        /// One and only constructor for this attribute
        /// </summary>
        /// <param name="bundle">Name of the bundle</param>
        /// <param name="version">Version of the bundel</param>
        public BundleAttribute(string bundle, string version) : this(bundle, version, false)
        {
        }

        /// <summary>
        /// Protected constructor only accessible to derived attributes
        /// </summary>
        /// <param name="bundle">Name of the bundle</param>
        /// <param name="version">Version of the bundel</param>
        /// <param name="isFrameworkComponent">Flag if this assembly is part of the framework and has elevated rights</param>
        protected BundleAttribute(string bundle, string version, bool isFrameworkComponent)
        {
            Bundle = bundle;
            Version = version;
            IsFrameworkComponent = isFrameworkComponent;
        }

        /// <summary>
        /// Name of the bundle
        /// </summary>
        public string Bundle { get; private set; }

        /// <summary>
        /// Version of the bundel
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Flag to get elevated access rights
        /// </summary>
        public bool IsFrameworkComponent { get; private set; }
    }
}
