// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Linking;

namespace Moryx.Material.IntegrationTests;

/// <summary>
/// Reference to a business object identified by a key.
/// </summary>
[DataContract]
public class TestReference(string key) : Reference
{
    [DataMember]
    public string Key { get; protected set; } = key;

    public override bool Matches(Reference? other) => other is TestReference test && test.Key == Key;

    public override string ToString() => $"Test '{Key}'";
}

/// <summary>
/// Managed variant of <see cref="TestReference"/>, like an integration would substitute it.
/// </summary>
public class ManagedTestReference : TestReference
{
    public ManagedTestReference(string key) : base(key)
    {
        State = ReferenceState.Active;
    }
}

[DataContract]
public sealed class TestLink : MaterialContainerLink<TestReference>
{
    public Task<LinkingResult> LinkAsync(string key, CancellationToken cancellationToken = default)
        => RequestAsync(new TestReference(key), cancellationToken);
}

[DataContract]
public class LinkedTestContainer : MaterialContainer
{
    [DataMember]
    public TestLink First { get; } = new();

    [DataMember]
    public TestLink Second { get; } = new();
}
