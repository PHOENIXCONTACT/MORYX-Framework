// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

namespace Moryx.ControlSystem.Processes.Endpoints;

public class ProcessHolderGroupModel
{
    public long Id { get; set; }
    public string Name { get; set; }

    public bool IsEmpty => Positions.All(x => x.IsEmpty);

    public List<ProcessHolderPositionModel> Positions { get; set; } = [];

    public VisualizationModel Visualization { get; set; }
}
