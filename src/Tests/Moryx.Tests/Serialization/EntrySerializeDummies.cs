// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

internal interface IExplicitInterface
{
    string ExplicitProperty { get; set; }
}

// ReSharper disable once InconsistentNaming
public class EntrySerialize_Explicit : IExplicitInterface
{
    string IExplicitInterface.ExplicitProperty { get; set; }

    public int NormalProperty { get; set; }
}

// ReSharper disable once InconsistentNaming
public class EntrySerialize_BaseType
{
    public int Property1 { get; set; }
}

// ReSharper disable once InconsistentNaming
public class EntrySerialize_DerivedType : EntrySerialize_BaseType
{
    public int Property2 { get; set; }
}

// ReSharper disable once InconsistentNaming
public class EntrySerialize_NotClassMixed
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

// ReSharper disable once InconsistentNaming
public class EntrySerialize_NoClassNoMember
{
    public string NullProperty1 { get; set; } = "123456";

    public bool NullProperty2 { get; set; } = true;

    public bool NullMethod1() => true;

    public string NullMethod2() => "1234";
}

// ReSharper disable once InconsistentNaming
[EntrySerialize(EntrySerializeMode.Never)]
public class EntrySerialize_NeverClassNoMember
{
    public string NullProperty1 { get; set; } = "123456";

    public bool NullProperty2 { get; set; } = true;

    public bool NullMethod1() => true;

    public string NullMethod2() => "1234";
}

[EntrySerialize]
public class EntrySerialize_AlwaysClassAlwaysMember
{
    [EntrySerialize]
    public string AlwaysProperty { get; set; } = "123456";
    [EntrySerialize]
    public int Property1 { get; set; }

    [EntrySerialize]
    public EntrySerialize_AlwaysClassAlwaysMember AnotherProperty { get; set; }

    internal IExplicitInterface ExplicitInterface { get; }
}

// ReSharper disable once InconsistentNaming
[EntrySerialize(EntrySerializeMode.Never)]
public class EntrySerialize_NeverClassAlwaysMember
{
    [EntrySerialize(EntrySerializeMode.Always)]
    public string AlwaysProperty1 { get; set; } = "123456";

    public bool NullProperty2 { get; set; } = true;

    [EntrySerialize(EntrySerializeMode.Always)]
    public bool AlwaysMethod1() => true;

    public string NullMethod2() => "1234";
}

[EntrySerialize(EntrySerializeMode.Never)]
public class EntrySerialize_InheritedBase
{
    public bool NullProperty1 { get; set; } = true;
}

public class EntrySerialize_Inherited : EntrySerialize_InheritedBase
{
    public string NullProperty2 { get; set; } = "789456";

    public bool NullProperty3 { get; set; }
}

[EntrySerialize]
public class AlwaysClass_Inherited : EntrySerialize_InheritedBase
{
    [EntrySerialize]
    public string NullProperty2 { get; set; } = "789456";

    public bool NullProperty3 { get; set; }
}

public class EntrySerialize_Methods : EntrySerialize_InheritedBase
{
    [EntrySerialize]
    public void InvocablePublic()
    {

    }

    [EntrySerialize]
    public void InvocablePublic(int intValue, string stringValue1, string stringValue2 = "testing value")
    {

    }

    [EntrySerialize]
#pragma warning disable CA1822 // Mark members as static
    internal void InvocableInternal()
#pragma warning restore CA1822 // Mark members as static
    {

    }

    [EntrySerialize]
    protected void NonInvocableProtected()
    {

    }

    [EntrySerialize]
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0051 // Remove unused private members
    private void NonInvocablePrivate()
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CA1822 // Mark members as static
    {

    }

    [EntrySerialize]
    public Task AsyncWithoutResult()
    {
        return Task.CompletedTask;
    }

    [EntrySerialize]
    public Task<string> AsyncWithStringResult()
    {
        return Task.FromResult("Test");
    }

    [EntrySerialize]
    public async Task<string> MethodWithRequiredAndOptionalParameters(string plainParameter, [Required] string requiredParameter, string nullableString = null, string defaultValueString = "Some test string")
    {
        return "Done";
    }
}
