// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using System.Collections.Generic;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    public class DummyCapabilities2 : ICapabilities
    {
        public bool IsCombined => false;

        public IEnumerable<ICapabilities> GetAll()
        {
            yield return this;
        }

        public bool ProvidedBy(ICapabilities provided) => provided is DummyCapabilities2;

        public bool Provides(ICapabilities required) => required is DummyCapabilities2;
    }
}

