// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Serial;

/// <summary>
/// Types of data transmission
/// </summary>
public enum DataBitTypes
{
    /// <summary>
    /// Seven bits at a time
    /// </summary>
    Seven = 7,

    /// <summary>
    /// 8 bits at a time
    /// </summary>
    Eight = 8
}