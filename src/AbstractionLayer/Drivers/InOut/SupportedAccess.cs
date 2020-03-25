// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.AbstractionLayer.Drivers.InOut
{
    /// <summary>
    /// Flags enums of the supported modes of an <see cref="IInOutDriver{TIn,TOut}"/>
    /// </summary>
    [Flags]
    public enum SupportedAccess
    {
        /// <summary>
        /// Driver inactive
        /// </summary>
        None = 0,

        /// <summary>
        /// Input and output is a single value with index or key
        /// </summary>
        Single = 1 << 8,
        
        /// <summary>
        /// Driver supports indexed access to values
        /// </summary>
        Index = 1 << 9,

        /// <summary>
        /// Driver supports access via keys
        /// </summary>
        Key = 1 << 10,


        /// <summary>
        /// Driver supports changed events for inputs
        /// </summary>
        Event = 1 << 16
    }
}
