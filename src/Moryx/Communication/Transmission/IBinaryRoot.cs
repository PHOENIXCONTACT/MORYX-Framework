// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Interface for root classes of message definitions
    /// </summary>
    public interface IBinaryRoot<THeader> : IByteSerializable
        where THeader : IBinaryHeader
    {
        /// <summary>
        /// Generate header for this message
        /// </summary>
        THeader Header { get; set; }
    }
}
