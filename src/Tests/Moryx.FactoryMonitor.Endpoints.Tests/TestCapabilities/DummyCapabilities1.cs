﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;
using System.Collections.Generic;

namespace Moryx.FactoryMonitor.Endpoints.Tests
{
    public class DummyCapabilities1 : ICapabilities
    {
        public bool IsCombined => false;

        public IEnumerable<ICapabilities> GetAll()
        {
            yield return this;
        }

        public bool ProvidedBy(ICapabilities provided) => provided is DummyCapabilities1;

        public bool Provides(ICapabilities required) => required is DummyCapabilities1;
    }
}

