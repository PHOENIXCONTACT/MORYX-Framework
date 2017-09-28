using System;
using System.Runtime.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf
{
    /// <summary>
    /// Contains detailed information on a serializable exception
    /// </summary>
    [DataContract]
    public class SerializableException
    {
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
        [DataMember]
        public string ExceptionTypeName { get; private set; }

        /// <summary>
        /// The message of the exception.
        /// </summary>
        [DataMember]
        public string Message { get; private set; }

        /// <summary>
        /// Starck trace of the exception.
        /// </summary>
        [DataMember]
        public string StackTrace { get; private set; }

        /// <summary>
        /// Contains an inner exception of type <see cref="SerializableException"/> if exists.
        /// </summary>
        [DataMember]
        public SerializableException InnerException { get; private set; }
    }
}
