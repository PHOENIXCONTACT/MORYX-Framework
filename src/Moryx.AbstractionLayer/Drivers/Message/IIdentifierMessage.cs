// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Common interface for messages objects that define recipient or sender
    /// </summary>
    public interface IIdentifierMessage
    {
        /// <summary>
        /// Source or target identifier of the message
        /// </summary>
        string Identifier { get; set; }
    }
}

