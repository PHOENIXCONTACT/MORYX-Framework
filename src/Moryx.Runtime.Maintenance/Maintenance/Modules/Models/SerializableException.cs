// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Maintenance.Modules
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
        public string ExceptionTypeName { get; set; }

        /// <summary>
        /// The message of the exception.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Starck trace of the exception.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Contains an inner exception of type <see cref="SerializableException"/> if exists.
        /// </summary>
        public SerializableException InnerException { get; set; }
    }
}
