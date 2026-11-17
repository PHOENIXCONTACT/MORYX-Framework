// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

internal interface IExplicitInterface
{
    string ExplicitProperty { get; set; }
}

public class ExplicitImplementation : IExplicitInterface
{
    string IExplicitInterface.ExplicitProperty { get; set; }

    public int NormalProperty { get; set; }
}

public class BaseType
{
    public int Property1 { get; set; }
}

public class DerivedType : BaseType
{
    public int Property2 { get; set; }
}

public class NotAttributedClassMixedMembers
{
    [EntrySerialize(EntrySerializeMode.Always)]
    public string AlwaysProperty1 { get; set; } = "123456";

    [EntrySerialize(EntrySerializeMode.Never)]
#pragma warning disable IDE0051 // Remove unused private members
    private string NeverProperty1 { get; set; } = "987654";
#pragma warning restore IDE0051 // Remove unused private members

    public bool NullProperty1 { get; set; } = true;

    [EntrySerialize(EntrySerializeMode.Always)]
    public bool AlwaysMethod1() => true;

    [EntrySerialize(EntrySerializeMode.Never)]
    public string NeverMethod1() => "1234";

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0051 // Remove unused private members
    private bool NullMethod1() => true;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CA1822 // Mark members as static
}

public class NotAttributedClassOrMember
{
    public string NullProperty1 { get; set; } = "123456";

    public bool NullProperty2 { get; set; } = true;

    public bool NullMethod1() => true;

    public string NullMethod2() => "1234";
}

[EntrySerialize(EntrySerializeMode.Never)]
public class NeverAttributedClassNoMember
{
    public string NullProperty1 { get; set; } = "123456";

    public bool NullProperty2 { get; set; } = true;

    public bool NullMethod1() => true;

    public string NullMethod2() => "1234";
}

[EntrySerialize]
public class AlwaysAttributedClassAndMember
{
    [EntrySerialize]
    public string AlwaysProperty { get; set; } = "123456";
    [EntrySerialize]
    public int Property1 { get; set; }

    [EntrySerialize]
    public AlwaysAttributedClassAndMember AnotherProperty { get; set; }

    internal IExplicitInterface ExplicitInterface { get; }
}

[EntrySerialize(EntrySerializeMode.Never)]
public class NeverAttributedClassAlwaysAttributedMember
{
    [EntrySerialize(EntrySerializeMode.Always)]
    public string AlwaysProperty1 { get; set; } = "123456";

    public bool NullProperty2 { get; set; } = true;

    [EntrySerialize(EntrySerializeMode.Always)]
    public bool AlwaysMethod1() => true;

    public string NullMethod2() => "1234";
}

[EntrySerialize(EntrySerializeMode.Never)]
public class NeverAttributedBaseType
{
    public bool NullProperty1 { get; set; } = true;
}

public class NotAttributedDerivedType : NeverAttributedBaseType
{
    public string NullProperty2 { get; set; } = "789456";

    public bool NullProperty3 { get; set; }
}

[EntrySerialize]
public class AlwaysAttributedDerivedType : NeverAttributedBaseType
{
    [EntrySerialize]
    public string NullProperty2 { get; set; } = "789456";

    public bool NullProperty3 { get; set; }
}

public class SelfReferencingBase
{
    public SelfReferencingBase Self { get; set; }
}

public class SelfReferencingDerived : SelfReferencingBase
{
}
