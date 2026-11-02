// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests.Serialization;

public class AllowedDeniedValuesClassDummy
{
    [AllowedValues(EnumDummy.ValueA, EnumDummy.ValueB)]
    public EnumDummy AllowedValues { get; set; }

    [DeniedValues(EnumDummy.Unset)]
    public EnumDummy DeniedValues { get; set; }

    [AllowedValues(1, 5, 20)]
    public int AllowedIntValues { get; set; }
}
