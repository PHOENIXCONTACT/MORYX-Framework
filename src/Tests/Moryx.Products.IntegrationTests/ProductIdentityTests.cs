// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.AbstractionLayer.Products;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests;

[TestFixture]
public class ProductIdentityTests
{
    [TestCase("ABC-01", "ABC", 1)]
    [TestCase("ABC-DEF-01", "ABC-DEF", 1)]
    [TestCase("123-456-12", "123-456", 12)]
    [TestCase("12345678-ABCD-42", "12345678-ABCD", 42)]
    [TestCase("My-Custom-Identifier-7", "My-Custom-Identifier", 7)]
    public void Parse_CustomIdentifiers_ReturnsExpectedIdentity(string identityString, string expectedIdentifier, short expectedRevision)
    {
        // Act
        var result = ProductIdentity.Parse(identityString);

        // Assert
        Assert.That(result.Identifier, Is.EqualTo(expectedIdentifier));
        Assert.That(result.Revision, Is.EqualTo(expectedRevision));
    }

    [TestCase("")]
    [TestCase("ABC")]
    [TestCase("-1")]
    [TestCase("ABC-")]
    [TestCase("ABC-DEF")]
    [TestCase("ABC-XYZ")]
    public void Parse_InvalidIdentity_ThrowsFormatException(string identityString)
    {
        Assert.Throws<FormatException>(() => ProductIdentity.Parse(identityString));
    }

    [Test]
    public void TryParse_CustomIdentifier_ReturnsTrue()
    {
        // Act
        var result = ProductIdentity.TryParse("12345678-ABCD-42", out var identity);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(identity, Is.Not.Null);
        Assert.That(identity.Identifier, Is.EqualTo("12345678-ABCD"));
        Assert.That(identity.Revision, Is.EqualTo(42));
    }
}
