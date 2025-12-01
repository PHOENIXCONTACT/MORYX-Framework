// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests;

public class AllowedDeniedValuesClass
{
    [AllowedValues(DummyEnum.ValueA, DummyEnum.ValueB)]
    public DummyEnum AllowedValues { get; set; }

    [DeniedValues(DummyEnum.Unset)]
    public DummyEnum DeniedValues { get; set; }

    [AllowedValues(1, 5, 20)]
    public int AllowedIntValues { get; set; }
}
