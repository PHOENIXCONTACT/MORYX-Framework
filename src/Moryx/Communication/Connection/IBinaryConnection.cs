// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Modules;

namespace Moryx.Communication
{
    /// <summary>
    /// Visionary interface for the binary connection
    /// </summary>
    public interface IBinaryConnection : IConfiguredPlugin<BinaryConnectionConfig>, IBinaryTransmission, IDisposable
    {
        /// <summary>
        /// Gets the current ConnectionState.
        /// </summary>
        /// <value>
        /// The state of the current.
        /// </value>
        BinaryConnectionState CurrentState { get; }

        /// <summary>
        /// In case we think our connection is broken we can enforce a reconnect here
        /// </summary>
        void Reconnect();

        /// <summary>
        /// Reconnect but wait for the given delay to let the other device reset as well
        /// </summary>
        /// <param name="delayMs">Delay in milliseconds</param>
        void Reconnect(int delayMs);

        /// <summary>
        /// Occurs when a connection state has to be published.
        /// </summary>
        event EventHandler<BinaryConnectionState> NotifyConnectionState;
    }
}
