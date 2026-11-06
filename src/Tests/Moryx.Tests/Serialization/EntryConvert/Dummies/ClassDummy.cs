// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Tests.Serialization;

public class ClassDummy
{
    public int Number { get; set; }

    public string Name { get; set; }

    public DateTime ModifiedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public SubClassDummy SingleClass { get; set; }

    public SubClassDummy[] SubArray { get; set; }

    public List<SubClassDummy> SubList { get; set; }

    public IEnumerable<SubClassDummy> SubEnumerable { get; set; }

    public IDictionary<int, SubClassDummy> SubDictionary { get; set; }

    public EnumDummy[] EnumArray { get; set; }

    public List<EnumDummy> EnumList { get; set; }

    public IEnumerable<EnumDummy> EnumEnumerable { get; set; }

    public bool[] BoolArray { get; set; }

    public List<bool> BoolList { get; set; }

    public IEnumerable<EnumDummy> BoolEnumerable { get; set; }

    public SubClassDummy GetterOnly { get; } = new();

    public int? NullablePrimitive { get; set; } = 0;

#pragma warning disable CA2201 // Do not raise reserved exception types
    public int ThrowsException => throw new Exception("BAM.");
#pragma warning restore CA2201 // Do not raise reserved exception types

    public static ClassDummy Populate() => new()
    {
        Number = 42,
        Name = "TestName",
        ModifiedAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
        CreatedAt = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero),
        SingleClass = new SubClassDummy { Foo = 1.2f, Enum = EnumDummy.ValueA, EnumArray = [EnumDummy.ValueA, EnumDummy.ValueB] },
        SubArray = [new SubClassDummy { Foo = 1.2f, Enum = EnumDummy.ValueB }],
        SubList = [new SubClassDummy { Foo = 3.4f, Enum = EnumDummy.ValueA }],
        SubEnumerable = [new SubClassDummy { Foo = 5.6f, Enum = EnumDummy.ValueB }],
        SubDictionary = new Dictionary<int, SubClassDummy>
        {
            { 1, new SubClassDummy { Foo = 1.2f, Enum = EnumDummy.ValueA } },
            { 2, new SubClassDummy { Foo = 3.4f, Enum = EnumDummy.ValueB } }
        },
        EnumArray = [EnumDummy.ValueA, EnumDummy.ValueB],
        EnumList = [EnumDummy.ValueA, EnumDummy.ValueB],
        EnumEnumerable = [EnumDummy.ValueA, EnumDummy.ValueB],
        BoolArray = [true, false, true],
        BoolList = [true, false],
        BoolEnumerable = [EnumDummy.ValueA, EnumDummy.ValueB],
        NullablePrimitive = 7
    };
}
