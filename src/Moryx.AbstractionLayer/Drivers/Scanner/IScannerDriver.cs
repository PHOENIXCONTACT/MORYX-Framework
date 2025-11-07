// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#nullable enable

using Moryx.AbstractionLayer.Drivers.InOut;

namespace Moryx.AbstractionLayer.Drivers.Scanner;

/// <summary>
/// Common interface for barcode / QR-Code scanners
/// </summary>
public interface IScannerDriver : IInputDriver<ScannerResult>
{
}
