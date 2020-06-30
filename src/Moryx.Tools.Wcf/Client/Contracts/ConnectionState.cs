// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Tools.Wcf
{
    internal static class StateTransformer
    {
        public static ConnectionState FromInternal(InternalConnectionState internalState,
            ConnectionState currentState)
        {
            switch (internalState)
            {
                case InternalConnectionState.New:
                    return ConnectionState.New;

                case InternalConnectionState.Success:
                    return ConnectionState.Success;

                case InternalConnectionState.FailedTry:
                    return ConnectionState.FailedTry;

                case InternalConnectionState.VersionMatch:
                    return currentState;

                case InternalConnectionState.VersionMissmatch:
                    return ConnectionState.VersionMissmatch;

                case InternalConnectionState.ConnectionLost:
                    return ConnectionState.ConnectionLost;

                case InternalConnectionState.Closing:
                    return ConnectionState.Closing;

                case InternalConnectionState.Closed:
                    return ConnectionState.Closed;

                default:
                    throw new ArgumentException(string.Format("Can't convert unknown state '{0}'", internalState),
                        "internalState");
            }
        }
    }


    /// <summary>
    /// The current state of a WCF connection
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
        VersionMissmatch,

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

    /// <summary>
    /// The current state of a WCF connection
    /// </summary>
    internal enum InternalConnectionState
    {
        /// <summary>
        /// Client is new.
        /// </summary>
        New,

        /// <summary>
        /// Versions of client and server match
        /// </summary>
        VersionMatch,

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
        VersionMissmatch,

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
