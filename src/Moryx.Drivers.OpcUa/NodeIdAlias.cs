﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

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