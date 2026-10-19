// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests.Serialization;

public class DummyClass
{
    public int Number { get; set; }

    public string Name { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public int ReadOnly => 10;

    public SubClass SingleClass { get; set; }

    public SubClass[] SubArray { get; set; }

    public List<SubClass> SubList { get; set; }

    public IEnumerable<SubClass> SubEnumerable { get; set; }

    public IDictionary<int, SubClass> SubDictionary { get; set; }

    public DummyEnum[] EnumArray { get; set; }

    public List<DummyEnum> EnumList { get; set; }

    public IEnumerable<DummyEnum> EnumEnumerable { get; set; }

    public bool[] BoolArray { get; set; }

    public List<bool> BoolList { get; set; }

    public IEnumerable<DummyEnum> BoolEnumerable { get; set; }

    public SubClass SingleClassNonLocalized { get; set; }
}

public class SubClass
{
    public float Foo { get; set; }

    public DummyEnum Enum { get; set; }

    public DummyEnum[] EnumArray { get; set; }
}

public enum DummyEnum
{
    Unset,
    ValueA,
    ValueB
}

public class NullablePropertiesClass
{
    public int? Value { get; set; } = 0;
}

public enum DisplayNameEnum
{
    [Display(Name = DisplayNameEnumStrings.UnsetDisplayName, Description = DisplayNameEnumStrings.UnsetDescription)]
    Unset,

    [Display(Name = DisplayNameEnumStrings.ValueADisplayName)]
    ValueA,

    ValueB
}

public static class DisplayNameEnumStrings
{
    public const string UnsetDisplayName = "No Value";
    public const string UnsetDescription = "Nothing selected";
    public const string ValueADisplayName = "First Value";
}

public class DisplayNameEnumClass
{
    public DisplayNameEnum EnumProperty { get; set; }
}