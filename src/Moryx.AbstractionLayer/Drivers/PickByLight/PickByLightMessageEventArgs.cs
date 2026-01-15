// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#nullable enable

namespace Moryx.AbstractionLayer.Drivers.PickByLight;

/// <summary>
/// Event args used then the pick-by-light system sends a message
/// </summary>
public class PickByLightMessageEventArgs
{
    /// <summary>
    /// The sub device which has button changes
    /// </summary>
    public int? DeviceId { get; set; }

    /// <summary>
    /// Identifier of the button which has been pressed.
    /// </summary>
    public int? ButtonId { get; set; }

    /// <summary>
    /// Pick by light event action
    /// </summary>
    public PickAction? Action { get; set; }

    /// <summary>
    /// The message from the device
    /// </summary>
    public string? Message { get; set; }
}

