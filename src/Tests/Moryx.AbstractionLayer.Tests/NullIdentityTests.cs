// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class NullIdentityTests
{
    [Test]
    public void Instance_IsSingleton()
    {
        Assert.That(NullIdentity.Instance, Is.SameAs(NullIdentity.Instance));
    }

    [Test]
    public void Identifier_IsEmpty()
    {
        Assert.That(NullIdentity.Instance.Identifier, Is.Empty);
    }

    [Test]
    public void SetIdentifier_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => NullIdentity.Instance.SetIdentifier("X"));
    }

    [Test]
    public void Equals_AnotherNullIdentity_ReturnsTrue()
    {
        Assert.That(NullIdentity.Instance.Equals(NullIdentity.Instance), Is.True);
    }

    [Test]
    public void Equals_OtherIdentityType_ReturnsFalse()
    {
        Assert.That(NullIdentity.Instance.Equals(new BatchIdentity("X")), Is.False);
    }

    [Test]
    public void Equals_Null_ReturnsFalse()
    {
        Assert.That(NullIdentity.Instance.Equals(null), Is.False);
    }
}
