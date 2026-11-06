// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;

namespace Moryx.Tests.Serialization;

public class MethodClassDummy
{
    public MethodClassDummy()
    {
    }

    public MethodClassDummy(int number) : this(number, string.Empty)
    {
    }

    public MethodClassDummy(int number, string name)
    {
        Number = number;
        Name = name;
    }

    public int Number { get; private set; }

    public string Name { get; private set; }

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
}
