// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Rfid;

/// <summary>
/// Represents the possible outcomes of an RFID tag kill (erase) operation.
/// </summary>
public enum KillingResult
{
    /// <summary>
    /// The tag was successfully killed.
    /// </summary>
    Success = 0,

    /// <summary>
    /// Kill command failed, but the tag cannot be read afterward.
    /// </summary>
    FailedButTagUnreadable = 1,

    /// <summary>
    /// Operation aborted because another tag was detected before completion.
    /// </summary>
    AbortedDueToOtherTagDetected = 2,

    /// <summary>
    /// Kill operation failed; the tag remains active.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Operation aborted because the target tag was not detected.
    /// </summary>
    AbortedTagNotFound = 4,

    /// <summary>
    /// Operation aborted due to too many tags in the field.
    /// </summary>
    AbortedTooManyTags = 5,

    /// <summary>
    /// Operation aborted because an unexpected tag was detected.
    /// </summary>
    AbortedUnexpectedTag = 6,

    /// <summary>
    /// A technical error occurred, preventing the kill operation.
    /// </summary>
    TechnicalError = 7,

    /// <summary>
    /// Kill operation was manually or system-aborted.
    /// </summary>
    Aborted = 8,
}
