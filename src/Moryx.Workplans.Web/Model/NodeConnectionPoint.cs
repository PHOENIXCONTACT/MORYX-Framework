// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Endpoint
{
    public class NodeConnectionPoint
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public List<NodeConnector> Connections { get; set; } = new List<NodeConnector>();
    }
}

