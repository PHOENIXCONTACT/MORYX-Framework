using System;
using System.Runtime.Serialization;

namespace Marvin.Runtime.ModuleManagement
{
    /// <summary>
    /// Exception raised from a module on start if a dependency is missing. This will cause the framework base to
    /// enter a retry sequence
    /// </summary>
    public class MissingDependencyException : MarvinException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MissingDependencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public MissingDependencyException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
            Retries = (int)si.GetValue("Retries", typeof(int));
            AwaitTimeMs = (int)si.GetValue("AwaitTimeMs", typeof(int));
        }

        /// <summary>
        /// Create a new instance of this exception to indicate a missing dependency
        /// </summary>
        public MissingDependencyException(string message, int retries, int awaitTimeMs) : base(message)
        {
            Retries = retries;
            AwaitTimeMs = awaitTimeMs;
        }

        /// <summary>
        /// Create a new instance of this exception to indicate a missing dependency
        /// </summary>
        public MissingDependencyException(string message, int retries, int awaitTimeMs, Exception ex) : base(message, ex)
        {
            Retries = retries;
            AwaitTimeMs = awaitTimeMs;
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

            info.AddValue("Retries", Retries);
            info.AddValue("AwaitTimeMs", AwaitTimeMs);
        }

        /// <summary>
        /// Maximum number of start retries
        /// </summary>
        public int Retries { get; set; }

        /// <summary>
        /// Time to wait between tries
        /// </summary>
        public int AwaitTimeMs { get; set; }
    }
}
