// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Launcher;

/// <summary>
/// Contains properties holding related information of a WebModule to process in the Shell
/// </summary>
public class WebModuleItem : ModuleItem
{
    /// <summary>
    /// Optional URL to an event stream
    /// </summary>
    public string EventStream { get; set; }
}