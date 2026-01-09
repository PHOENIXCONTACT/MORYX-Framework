// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Scanner;

/// <summary>
/// Options for the read of <see cref="IScannerDriver"/> code read 
/// </summary>
public class ReadCodeOptions
{
    /// <summary>
    /// Determines whether the button of the scanner can be used to initiate a scan or whether it is triggered by the system
    /// </summary>
    bool UseButton { get; set; }
}
