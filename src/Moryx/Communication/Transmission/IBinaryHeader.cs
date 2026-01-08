// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Base class for binary headers
    /// </summary>
    public interface IBinaryHeader : IByteSerializable
    {
        /// <summary>
        /// Length of the payload
        /// </summary>
        int PayloadLength { get; set; }
    }
}
