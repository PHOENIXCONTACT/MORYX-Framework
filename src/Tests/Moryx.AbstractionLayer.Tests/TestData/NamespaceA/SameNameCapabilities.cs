// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Resources.TestData.NamespaceA;

public class SameNameCapabilities : CapabilitiesBase
{
    protected override bool ProvidedBy(ICapabilities provided)
    {
        return true;
    }
}
