// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Transmission for protocols based on transmission
    /// </summary>
    public class DelimitedMessageContext : ReadContext
    {
        /// <summary>
        /// Start of message
        /// </summary>
        public int MessageStart { get; set; }

        /// <summary>
        /// End of the read bytes
        /// </summary>
        public int MessageEnd { get; set; }

        /// <summary>
        /// Flag if the message start was allready found
        /// </summary>
        public bool StartFound { get; set; }
    }
}
