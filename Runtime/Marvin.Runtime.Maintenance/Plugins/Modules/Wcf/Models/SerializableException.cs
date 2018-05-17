using System;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Contains detailed information on a serializable exception
    /// </summary>
    public class SerializableException
    {
        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public SerializableException()
        { 
            
        }

        /// <summary>
        /// Constructor for the serializable exception with the occured exception.
        /// </summary>
        /// <param name="e">The exception which occured.</param>
        public SerializableException(Exception e)
        {
            ExceptionTypeName = e.GetType().Name;

            Message = e.Message;
            StackTrace = e.StackTrace;

            if (e.InnerException != null)
            {
                InnerException = new SerializableException(e.InnerException);
            }
        }

        /// <summary>
        /// The name of the exception type.
        /// </summary>
        public string ExceptionTypeName { get; private set; }

        /// <summary>
        /// The message of the exception.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Starck trace of the exception.
        /// </summary>
        public string StackTrace { get; private set; }

        /// <summary>
        /// Contains an inner exception of type <see cref="SerializableException"/> if exists.
        /// </summary>
        public SerializableException InnerException { get; private set; }
    }
}
