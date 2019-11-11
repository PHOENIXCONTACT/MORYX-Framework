using System;

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