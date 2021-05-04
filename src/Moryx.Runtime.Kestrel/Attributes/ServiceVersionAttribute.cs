// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Attribute used to declare a services version for clients to check compliance
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceVersionAttribute : Attribute
    {
        /// <summary>
        /// Set version of the service
        /// </summary>
        /// <param name="version"></param>
        public ServiceVersionAttribute(string version)
        {
            Version = version;
        }

        /// <summary>
        /// Version of the server
        /// </summary>
        public string Version { get; }
    }
}
