// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// The reason why a client is not connected anymore.
    /// </summary>
    internal enum DisconnectReason
    {
        /// <summary>
        /// The connection was closed.
        /// </summary>
        Closed,

        /// <summary>
        /// The connection was faulted.
        /// </summary>
        Faulted
    }
}
