﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Message
{
    /// <summary>
    /// Common interface for messages objects that define recipient or sender
    /// Might be used by implementations of <see cref="IMessageDriver{TMessage}"/> to select the correct channel
    /// </summary>
    public interface IIdentifierMessage
    {
        /// <summary>
        /// Source or target identifier of the message
        /// </summary>
        string Identifier { get; set; }
    }
}

