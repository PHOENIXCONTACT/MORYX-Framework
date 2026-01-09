// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Rfid;

/// <summary>
/// Options for the read of <see cref="IRfidDriver"/> tag read 
/// </summary>
public class ReadTagOptions
{
    /// <summary>
    /// The direction the tag must move
    /// </summary>
    public RfidDirection Direction { get; set; }
}
