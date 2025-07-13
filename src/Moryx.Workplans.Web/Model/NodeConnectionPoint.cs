// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Workplans.Endpoint
{
    public class NodeConnectionPoint
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public List<NodeConnector> Connections { get; set; } = new List<NodeConnector>();
    }
}

