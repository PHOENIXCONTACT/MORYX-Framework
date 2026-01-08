// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Drivers.Marking;

/// <summary>
/// State classification for marking lasers
/// </summary>
[Flags]
public enum MarkingLaserStateClassification
{
    /// <inhericdoc cref="StateClassification.Offline"/>
    Offline = StateClassification.Offline,

    /// <inhericdoc cref="StateClassification.Initializing"/>
    Initializing = StateClassification.Initializing,

    /// <inhericdoc cref="StateClassification.Running"/>
    Running = StateClassification.Running,

    /// <inhericdoc cref="StateClassification.Busy"/>
    Busy = StateClassification.Busy,

    /// <inhericdoc cref="StateClassification.Maintenance"/>
    Maintenance = StateClassification.Maintenance,

    /// <inhericdoc cref="StateClassification.Error"/>
    Error = StateClassification.Error,

    /// <summary>
    /// Marking file is prepared and ready to be used for marking
    /// The laser can be running but not ready for marking.
    /// </summary>
    ReadyForMarking = Running | 1 << 17,
}
