// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

namespace Moryx.ControlSystem.Processes.Endpoints;

/// <summary>
/// Configure the visual appearance of a ProcessHolder
/// </summary>
public class VisualizationModel
{
    public string Icon { get; set; }

    public Category Category { get; set; }
}

/// <summary>
/// Since the ProcessHolderPosition can be inside a ProcessHolderGroup or a Cell,
/// This defines the relation of the process holder to it's parent.
/// </summary>
public enum Category
{
    ProcessHolderGroup = 1,
    ParentResource = 2,
}
