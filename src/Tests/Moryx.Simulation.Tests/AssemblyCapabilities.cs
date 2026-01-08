// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Simulation.Tests;

public class AssemblyCapabilities : ICapabilities
{
    public bool IsCombined => false;

    public IEnumerable<ICapabilities> GetAll()
    {
        yield return this;
    }

    public bool ProvidedBy(ICapabilities provided)
    {
        return provided is AssemblyCapabilities;
    }

    public bool Provides(ICapabilities required)
    {
        return required is AssemblyCapabilities;
    }
}