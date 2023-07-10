// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Moryx.Configuration
{
    /// <summary>
    /// Exception 
    /// </summary>
    public class InvalidConfigException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public InvalidConfigException()
        {
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        /// <param name="si">The SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
        public InvalidConfigException(SerializationInfo si, StreamingContext context)
            : base(si, context)
        {
            FaultyEntry = si.GetValue("FaultyEntry", typeof(object));
            PropertyFailure = (string)si.GetValue("PropertyFailure", typeof(string));
        }

        /// <summary>
        /// Signal an invalid value within the configuration
        /// </summary>
        /// <param name="faultyEntry">Optional subentry defining the faulty property</param>
        /// <param name="propertyFailure">Failure description containing property name and cause of failure</param>
        public InvalidConfigException(object faultyEntry, string propertyFailure)
            : base($"Invalid entry {faultyEntry.GetType().Name} in config. Error: {propertyFailure}")
        {
            FaultyEntry = faultyEntry;
            PropertyFailure = propertyFailure;
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
        [Obsolete("Override of an obsolete method.")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("FaultyEntry", FaultyEntry);
            info.AddValue("PropertyFailure", PropertyFailure);
        }

        /// <summary>
        /// Optional subentry defining the faulty property
        /// </summary>
        public object FaultyEntry { get; set; }

        /// <summary>
        /// Failure description containing property name and cause of failure
        /// </summary>
        public string PropertyFailure { get; set; }
    }
}
