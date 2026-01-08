// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Drivers.OpcUa;

internal class NodeIdAlias
{
    public string Alias { get; set; }
    public string NodeId { get; set; }

    public override string ToString()
    {
        return Alias;
    }
}