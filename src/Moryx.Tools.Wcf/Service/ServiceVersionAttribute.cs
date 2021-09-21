// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication.Endpoints;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Attribute used to declare a services version for clients to check compliance
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    [Obsolete("Use hosting independent {EndpointAttribute} instead")]
    public class ServiceVersionAttribute : EndpointAttribute
    {
        /// <summary>
        /// Set version of the service
        /// </summary>
        /// <param name="version"></param>
        public ServiceVersionAttribute(string version)
        {
            Version = version;
        }
    }
}
