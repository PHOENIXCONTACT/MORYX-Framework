// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class CombinedIdentityTests
{
    #region Factory

    private static IEnumerable<TestCaseData> EmptyInputKinds() =>
    [
        new TestCaseData(null),
        new TestCaseData([]),
        new TestCaseData(NullIdentity.Instance),
        new TestCaseData([NullIdentity.Instance]),
    ];

    [TestCaseSource(nameof(EmptyInputKinds))]
    public void From_WithEmptyInput_ReturnsNullIdentity(params IIdentity[] emptyInput)
    {
        Assert.That(CombinedIdentity.From(emptyInput), Is.SameAs(NullIdentity.Instance));
    }

    private static IEnumerable<TestCaseData> SingleInputKinds() =>
    [
        new TestCaseData(new BatchIdentity("A"), new BatchIdentity("A")),
        new TestCaseData(new IIdentity[] { new BatchIdentity("A"), NullIdentity.Instance }, new BatchIdentity("A")),
    ];

    [TestCaseSource(nameof(SingleInputKinds))]
    public void From_WithSingleInput_ReturnsSingleIdentity(IEnumerable<IIdentity> singleInput, IIdentity singleOutput)
    {
        Assert.That(CombinedIdentity.From(singleInput), Is.SameAs(singleOutput));
    }

    [Test]
    public void From_WithMultipleIdentities_ReturnsCombinedIdentity()
    {
        var result = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(result, Is.InstanceOf<CombinedIdentity>());
    }

    [Test]
    public void From_WithNullIdentityInMixedList_FiltersIt()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");

        // Act
        var result = CombinedIdentity.From([a, NullIdentity.Instance, b]);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(2).And.Contain(a).And.Contain(b));
    }

    #endregion

    #region Identifier

    [Test]
    public void Identifier_OfSingleEntry_ReturnsInnerIdentifier()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("ABC"));
        Assert.That(sut.Identifier, Is.EqualTo("ABC"));
    }

    [Test]
    public void Identifier_OfMultipleEntries_JoinedWithPipeSeparator()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.Identifier, Is.EqualTo("A | B"));
    }

    #endregion

    #region Canonical ordering

    [Test]
    public void GetAll_WithSameType_OrderedAlphabeticallyByIdentifier()
    {
        // Arrange
        var z = new BatchIdentity("Z");
        var a = new BatchIdentity("A");
        var m = new BatchIdentity("M");

        // Act
        var sut = CombinedIdentity.From(z, m, a);

        // Assert
        Assert.That(sut.GetAll(), Is.EqualTo(new IIdentity[] { a, m, z }));
    }

    [Test]
    public void GetAll_WithDifferentTypes_OrderedByFullTypeName()
    {
        // Arrange
        var stub = new StubIdentity("A");
        var batch = new BatchIdentity("A");

        // Act
        var sut = CombinedIdentity.From(stub, batch);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.GetAll()[0], Is.InstanceOf<BatchIdentity>());
            Assert.That(sut.GetAll()[1], Is.InstanceOf<StubIdentity>());
        });
    }

    [Test]
    public void Identifier_ReflectsCanonicalOrder()
    {
        // Arrange — emptyInput is reverse alphabetical
        var sut = CombinedIdentity.From(new BatchIdentity("C"), new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.Identifier, Is.EqualTo("A | B | C"));
    }

    #endregion

    #region Flattening

    [Test]
    public void From_WithNestedCombinedIdentity_IsFlattened()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var inner = CombinedIdentity.From(a, b);
        var c = new BatchIdentity("C");

        // Act
        var sut = CombinedIdentity.From(inner, c);

        // Assert
        Assert.That(sut.GetAll(), Has.Count.EqualTo(3).And.Contain(a).And.Contain(b).And.Contain(c));
    }

    [Test]
    public void From_WithDeeplyNestedCombinedIdentity_IsFullyFlattened()
    {
        // Arrange — CombinedIdentity wrapping a CombinedIdentity wrapping singles
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var c = new BatchIdentity("C");
        var level1 = CombinedIdentity.From(a, b);
        var level2 = CombinedIdentity.From(level1, c);

        // Act
        var sut = CombinedIdentity.From(level2);

        Assert.That(sut.GetAll(), Has.Count.EqualTo(3).And.Contain(a).And.Contain(b).And.Contain(c));
    }

    #endregion

    #region SetIdentifier

    [Test]
    public void SetIdentifier_ThrowsInvalidOperationException()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.Throws<InvalidOperationException>(() => sut.SetIdentifier("X"));
    }

    #endregion

    #region Equals

    [Test]
    public void Equals_WithSameIdentities_ReturnsTrue()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        Assert.That(CombinedIdentity.From(a, b).Equals(CombinedIdentity.From(a, b)));
    }

    [Test]
    public void Equals_WithSameIdentitiesInReverseOrder_ReturnsTrue()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        Assert.That(CombinedIdentity.From(a, b).Equals(CombinedIdentity.From(b, a)));
    }

    [Test]
    public void Equals_WithDifferentIdentifierValues_ReturnsFalse()
    {
        var x = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        var y = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("C"));
        Assert.That(x.Equals(y), Is.False);
    }

    [Test]
    public void Equals_WithDifferentCount_ReturnsFalse()
    {
        var x = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        var y = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"));
        Assert.That(x.Equals(y), Is.False);
    }

    [Test]
    public void Equals_WithNull_ReturnsFalse()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.Equals(null), Is.False);
    }

    #endregion

    #region GetHashCode

    [Test]
    public void GetHashCode_EqualInstances_ReturnSameValue()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var x = CombinedIdentity.From(a, b);
        var y = CombinedIdentity.From(b, a);
        Assert.That(x.GetHashCode(), Is.EqualTo(y.GetHashCode()));
    }

    #endregion

    #region MatchedBy

    [Test]
    public void MatchedBy_WithSingleCandidateWhenCurrentHasExactlyThatOne_ReturnsTrue()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"));
        Assert.That(sut.MatchedBy(new BatchIdentity("A")));
    }

    [Test]
    public void MatchedBy_WithSingleCandidateWhenCurrentHasMultiple_ReturnsFalse()
    {
        // A single identity cannot cover a combined with more than one entry
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.MatchedBy(new BatchIdentity("A")), Is.False);
    }

    [Test]
    public void MatchedBy_WithCombinedSupersetCandidate_ReturnsTrue()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.MatchedBy(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"))));
    }

    [Test]
    public void MatchedBy_WithFullSetCandidate_ReturnsTrue()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var sut = CombinedIdentity.From(a, b);
        Assert.That(sut.MatchedBy(CombinedIdentity.From(a, b)));
    }

    [Test]
    public void MatchedBy_WithCombinedSubsetCandidate_ReturnsFalse()
    {
        // Candidate does not cover all identities of this combined
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"));
        Assert.That(sut.MatchedBy(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"))), Is.False);
    }

    #endregion

    #region Matches

    [Test]
    public void Matches_WithSingleIdentityContained_ReturnsTrue()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.Matches(new BatchIdentity("B")));
    }

    [Test]
    public void Matches_WithSingleIdentityNotContained_ReturnsFalse()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.Matches(new BatchIdentity("C")), Is.False);
    }

    [Test]
    public void Matches_WithSubsetRequirement_ReturnsTrue()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"));
        Assert.That(sut.Matches(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("C"))));
    }

    [Test]
    public void Matches_WithFullSetRequirement_ReturnsTrue()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var sut = CombinedIdentity.From(a, b);
        Assert.That(sut.Matches(CombinedIdentity.From(a, b)));
    }

    [Test]
    public void Matches_WithRequirementContainsUnknownIdentity_ReturnsFalse()
    {
        var sut = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(sut.Matches(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("C"))), Is.False);
    }

    #endregion

    private sealed class StubIdentity(string identifier) : IIdentity
    {
        public string Identifier { get; } = identifier;
        public void SetIdentifier(string id) => throw new NotSupportedException();
        public bool Equals(IIdentity other) => other is StubIdentity s && s.Identifier == Identifier;
    }
}
