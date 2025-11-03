// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base result object for transmissions
    /// </summary>
    public abstract class TransmissionResult
    {
        /// <summary>
        /// Create success message
        /// </summary>
        protected TransmissionResult()
        {
            IsSuccess = true;
        }

        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="error">Error that caused the failure</param>
        protected TransmissionResult(TransmissionError error)
        {
            IsSuccess = false;
            Error = error;
        }

        /// <summary>
        /// Flag if this transmission was a success
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Error that occured
        /// </summary>
        public TransmissionError Error { get; set; }
    }

    /// <summary>
    /// Error wrapper class
    /// </summary>
    public class TransmissionError
    {
        /// <summary>
        /// Create error from error message
        /// </summary>
        public TransmissionError(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Human readable error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}", Message);
        }
    }
}
