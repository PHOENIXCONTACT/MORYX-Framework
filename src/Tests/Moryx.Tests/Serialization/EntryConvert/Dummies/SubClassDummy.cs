// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests.Serialization;

public sealed class SubClassDummy
{
    public float Foo { get; set; }

    public EnumDummy Enum { get; set; }

    public EnumDummy[] EnumArray { get; set; }
}
