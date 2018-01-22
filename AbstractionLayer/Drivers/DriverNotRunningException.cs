using System.Runtime.Serialization;

namespace Marvin.AbstractionLayer.Drivers
{
    /// <summary>
    /// Exception for busy drivers. The driver is running but cannot handle requests
    /// </summary>
    public class DriverNotRunningException : MarvinException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DriverNotRunningException"/> class.
        /// </summary>
        public DriverNotRunningException()
            : base("Cannot handle request. Driver is not in state " + StateClassification.Running + "!")
        {

        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public DriverNotRunningException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
        }

    }
}