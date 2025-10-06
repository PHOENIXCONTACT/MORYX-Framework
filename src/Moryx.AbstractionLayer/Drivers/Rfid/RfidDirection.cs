// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Rfid
{
    /// <summary>
    /// Movement direction of an rfid tag
    /// </summary>
    public enum RfidDirection
    {
        /// <summary>
        /// No direction was detected
        /// </summary>
        Unset = 0,

        /// <summary>
        /// Direction detection from left to right
        /// </summary>
        FromLeftToRight = 1,

        /// <summary>
        /// Direction detection from right ro left
        /// </summary>
        FromRightToLeft = 2
    }
}
