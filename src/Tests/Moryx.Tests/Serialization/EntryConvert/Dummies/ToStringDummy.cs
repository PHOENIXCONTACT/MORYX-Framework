// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Tests.Serialization;

[DisplayName("To String Dummy")]
public class ToStringDummy
{
    public string Name { get; set; }

    public override string ToString() => $"Dummy {Name}";
}
