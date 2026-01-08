// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#nullable enable
using System.Diagnostics;

namespace Moryx.AbstractionLayer.Drivers.Scanner;

/// <summary>
/// Result of the <see cref="IScannerDriver"/>
/// </summary>
[DebuggerDisplay("Code = {Code}")]
public class ScannerResult
{
    /// <summary>
    /// A read code from a barcode / qr-code scanner
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Optional binary data of the scanned content
    /// </summary>
    public byte? BinaryData { get; set; }
}
