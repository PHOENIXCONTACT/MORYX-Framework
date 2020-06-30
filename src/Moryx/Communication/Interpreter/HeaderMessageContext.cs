// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Communication
{
    /// <summary>
    /// Enum determining the state of parsing the bytes
    /// </summary>
    public enum ParsingState
    {
        /// <summary>
        /// Currently parsing the header
        /// </summary>
        ReadingHeader,
        /// <summary>
        /// Currently parsing the payload
        /// </summary>
        ReadingPayload,
    }

    /// <summary>
    /// Transmission for header based messages
    /// </summary>
    public class HeaderMessageContext<THeader> : ReadContext
        where THeader : IBinaryHeader, new()
    {
        /// <summary>
        /// Header of this message
        /// </summary>
        public THeader Header { get; set; }

        /// <summary>
        /// Currrent parsing state
        /// </summary>
        public ParsingState State { get; set; }
    }
}
