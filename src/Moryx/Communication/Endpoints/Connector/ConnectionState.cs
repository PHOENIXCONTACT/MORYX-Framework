// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// The current state of a connection
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// Client is new.
        /// </summary>
        New,

        /// <summary>
        /// Connection established
        /// </summary>
        Success,

        /// <summary>
        /// Failed to connect to endpoint
        /// </summary>
        FailedTry,

        /// <summary>
        /// Versions of client and server don't match
        /// </summary>
        VersionMismatch,

        /// <summary>
        /// Connection to endpoint lost
        /// </summary>
        ConnectionLost,

        /// <summary>
        /// Factory is closing and offers last chance to send infos to server
        /// </summary>
        Closing,

        /// <summary>
        /// Connection to endpoint closed
        /// </summary>
        Closed
    }
}