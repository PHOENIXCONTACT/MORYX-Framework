// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Exception for busy drivers. The driver is running but cannot handle requests
    /// </summary>
    public class DriverNotRunningException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotRunningException"/> class.
        /// </summary>
        public DriverNotRunningException()
            : base("Cannot handle request. Driver is not in state " + StateClassification.Running + "!")
        {
        }
    }
}
