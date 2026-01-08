// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

public class NodeConnectionPoint
{
    public int Index { get; set; }

    public string Name { get; set; }

    public List<NodeConnector> Connections { get; set; } = new List<NodeConnector>();
}