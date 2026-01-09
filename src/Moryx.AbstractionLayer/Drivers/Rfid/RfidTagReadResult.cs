// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Rfid;

/// <summary>
/// Result of read rfid tag/tags
/// </summary>
public class RfidTagReadResult
{
    /// <summary>
    /// Read tags
    /// </summary>
    public IReadOnlyCollection<RfidTag> ReadTags { get; set; }
}
