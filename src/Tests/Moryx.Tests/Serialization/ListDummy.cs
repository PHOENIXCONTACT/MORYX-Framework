// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests.Serialization;

public class ListDummy
{
    public byte Number { get; set; }

    [AllowedValues(1.0, 13.37, 42.0)]
    public List<double> DoubleList { get; set; }

    public List<DummyEnum> EnumList { get; set; }

    public List<SubClass> ReadOnly => [new SubClass { Foo = 4.2f }];
}