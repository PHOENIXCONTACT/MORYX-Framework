// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Attribute used to declare a services version for clients to check compliance
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceNameAttribute : Attribute
    {
        /// <summary>
        /// Set name of the service
        /// </summary>
        /// <param name="name"></param>
        public ServiceNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the server
        /// </summary>
        public string Name { get; }
    }
}
