// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Properties;

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Exception thrown by the resource proxy fetched from the facade if it was detached and can no longer
    /// interact with the target instance.
    /// </summary>
    public class ProxyDetachedException : Exception
    {
        /// <summary>
        /// Create new instance of the detached exception
        /// </summary>
        public ProxyDetachedException() : base(Strings.ProxyDetachedException_Message)
        {
        }
    }
}
