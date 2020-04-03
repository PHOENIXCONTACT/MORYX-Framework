// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Marvin.AbstractionLayer.Drivers.InOut;

namespace Marvin.AbstractionLayer.Drivers.Scanner
{
    /// <summary>
    /// Common interface for barcode / QR-Code scanners
    /// </summary>
    public interface IScannerDriver : IInputDriver<string>
    {
        /// <summary>
        /// Event raised when a code was read
        /// </summary>
        event EventHandler<string> CodeRead;
    }
}
