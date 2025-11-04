// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers
{
    /// <summary>
    /// Base result object for transmissions
    /// </summary>
    public abstract class DriverResponse
    {
        /// <summary>
        /// Create success message
        /// </summary>
        protected DriverResponse()
        {
            IsSuccess = true;
        }

        /// <summary>
        /// Create failure message
        /// </summary>
        /// <param name="error">Error that caused the failure</param>
        protected DriverResponse(string error)
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
        public string Error { get; set; }
    }
}
