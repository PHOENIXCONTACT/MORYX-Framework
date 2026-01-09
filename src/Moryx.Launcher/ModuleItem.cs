// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Launcher;

public class ModuleItem
{
    /// <summary>
    ///  Unique route of the module
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// Sort index of the module
    /// </summary>
    public int SortIndex { get; set; }

    /// <summary>
    /// Title of the module
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Recognizable icon of the module
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Description of the module
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Category of the module
    /// </summary>
    public ModuleCategory Category { get; set; }
}
