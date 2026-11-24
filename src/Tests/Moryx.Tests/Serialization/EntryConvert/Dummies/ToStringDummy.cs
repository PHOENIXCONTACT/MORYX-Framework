// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests.Serialization;

public class ToStringDummy
{
    public string Name { get; set; }

    public override string ToString() => $"Dummy {Name}";
}
