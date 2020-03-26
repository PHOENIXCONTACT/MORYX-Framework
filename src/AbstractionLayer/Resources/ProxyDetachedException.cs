// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Exception thrown by the resource proxy fetched from the facade if it was detached and can no longer
    /// interact with the target instance.
    /// </summary>
    public class ProxyDetachedException : Exception
    {
        /// <summary>
        /// Create new instance of the detached excpetion
        /// </summary>
        public ProxyDetachedException() : base("The proxy was detached and can no longer be used!")
        {
        }
    }
}
