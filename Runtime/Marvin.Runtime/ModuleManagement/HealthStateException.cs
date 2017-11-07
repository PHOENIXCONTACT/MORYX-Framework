﻿using System.Runtime.Serialization;

namespace Marvin.Runtime.ModuleManagement
{
    /// <summary>
    /// Exception thrown if server module wasn't ready for requested operation
    /// </summary>
    public class HealthStateException : MarvinException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HealthStateException()
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public HealthStateException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
            RequiredStates = (ServerModuleState[])si.GetValue("RequiredStates", typeof(ServerModuleState[]));
            Current = (ServerModuleState)si.GetValue("Current", typeof(ServerModuleState));
        }

        /// <summary>
        /// Create a new instance of the <see cref="HealthStateException"/> to notify a facade user of an
        /// invalid operation
        /// </summary>
        /// <param name="current">Current state of the module</param>
        /// <param name="requiredStates">Required state of the module</param>
        public HealthStateException(ServerModuleState current, params ServerModuleState[] requiredStates)
        {
            RequiredStates = requiredStates;
            Current = current;
        }

        /// <summary>
        /// State required for desired operation
        /// </summary>
        public ServerModuleState[] RequiredStates { get; private set; }
        /// <summary>
        /// Current state of the module
        /// </summary>
        public ServerModuleState Current { get; private set; }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("Current HealthState {0} of service does not allow requested operation. Required states are: {1})",
                                     Current, string.Join(", ", RequiredStates));
            }
        }

        /// <summary>
        /// When overridden in a derived class, sets the System.Runtime.Serialization.SerializationInfo
        ///  with information about the exception.
        /// </summary>
        /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized 
        /// object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual
        /// information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The info parameter is a null reference</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("RequiredStates", RequiredStates);
            info.AddValue("Current", Current);
        }
    }
}
