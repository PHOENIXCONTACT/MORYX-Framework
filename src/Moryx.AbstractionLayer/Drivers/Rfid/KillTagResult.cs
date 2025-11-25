// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Rfid;

/// <summary>
/// Result for the <see cref="IRfidDriver"/> killing operation
/// </summary>
public class KillTagResult
{
    /// <summary>
    /// Result of the killing operation
    /// </summary>
    public KillingResult Result { get; set; }

    /// <summary>
    /// Tag which should be killed
    /// </summary>
    public RfidTag Tag { get; set; }
}
