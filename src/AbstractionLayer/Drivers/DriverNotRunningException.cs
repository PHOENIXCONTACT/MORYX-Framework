using System;
using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer.Drivers
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