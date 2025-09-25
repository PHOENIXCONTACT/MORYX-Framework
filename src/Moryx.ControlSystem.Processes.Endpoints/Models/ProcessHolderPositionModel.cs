// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

namespace Moryx.ControlSystem.Processes.Endpoints;

public class ProcessHolderPositionModel
{
    public long Id { get; set; }

    public long GroupId { get; set; }

    public string Name { get; set; }

    public int Position { get; set; }

    public string Activity { get; set; }

    public string Order { get; set; }

    public string Process { get; set; }

    public bool IsEmpty { get; set; }
}
