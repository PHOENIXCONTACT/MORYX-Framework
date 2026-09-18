// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class IdentityExtensionsTests
{
    #region Combine

    [Test]
    public void Combine_WithSingleOther_CreatesCombinedContainingBoth()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");

        // Act
        var result = a.Combine(b);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(2).And.Contain(a).And.Contain(b));
    }

    [Test]
    public void Combine_WithMultipleOthers_CreatesCombinedContainingAll()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var c = new BatchIdentity("C");

        // Act
        var result = a.Combine(b, c);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(a).And.Contain(b).And.Contain(c));
    }

    [Test]
    public void Combine_WithEnumerableOthers_CreatesCombinedContainingAll()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var c = new BatchIdentity("C");
        IEnumerable<IIdentity> others = [b, c];

        // Act
        var result = a.Combine(others);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(a).And.Contain(b).And.Contain(c));
    }

    [Test]
    public void Combine_WhenReceiverIsCombined_FlattensResult()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var combined = CombinedIdentity.From(a, b);
        var c = new BatchIdentity("C");

        // Act
        var result = combined.Combine(c);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(a).And.Contain(b).And.Contain(c));
    }

    [Test]
    public void Combine_WhenArgumentIsCombined_FlattensResult()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var c = new BatchIdentity("C");
        var inner = CombinedIdentity.From(b, c);

        // Act
        var result = a.Combine(inner);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(a).And.Contain(b).And.Contain(c));
    }

    [Test]
    public void Combine_WithNullIdentityArgument_FiltersIt()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");

        var result = a.Combine(NullIdentity.Instance, b);

        Assert.That(result.GetAll(), Has.Count.EqualTo(2).And.Contain(a).And.Contain(b));
    }

    #endregion

    #region GetAll

    [Test]
    public void GetAll_OnSingleIdentity_ReturnsSingleElementList()
    {
        // Arrange
        IIdentity identity = new BatchIdentity("A");

        // Act
        var result = identity.GetAll();

        // Assert
        Assert.That(result, Is.EqualTo([identity]));
    }

    [Test]
    public void GetAll_OnCombinedIdentity_ReturnsAllConstituents()
    {
        // Arrange
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var combined = CombinedIdentity.From(a, b);

        // Act
        var result = combined.GetAll();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2).And.Contain(a).And.Contain(b));
    }

    #endregion

    #region Matches

    [Test]
    public void Matches_OnSingleIdentity_WithEqualRequired_ReturnsTrue()
    {
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.Matches(new BatchIdentity("A")));
    }

    [Test]
    public void Matches_OnSingleIdentity_WithDifferentRequired_ReturnsFalse()
    {
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.Matches(new BatchIdentity("B")), Is.False);
    }

    [Test]
    public void Matches_OnSingleIdentity_WithSingleEntryCombinedRequired_ReturnsTrue()
    {
        // combined([A]).Equals(singleA) = true via CombinedIdentity.EqualsSingleIdentity
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.Matches(CombinedIdentity.From(new BatchIdentity("A"))));
    }

    [Test]
    public void Matches_OnSingleIdentity_WithMultiEntryCombinedRequired_ReturnsFalse()
    {
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.Matches(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"))), Is.False);
    }

    [Test]
    public void Matches_OnCombinedIdentity_WithContainedSingle_ReturnsTrue()
    {
        var identitiy = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(identitiy.Matches(new BatchIdentity("A")));
    }

    [Test]
    public void Matches_OnCombinedIdentity_WithNotContainedSingle_ReturnsFalse()
    {
        var identitiy = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(identitiy.Matches(new BatchIdentity("C")), Is.False);
    }

    [Test]
    public void Matches_OnCombinedIdentity_WithSubsetCombined_ReturnsTrue()
    {
        var identitiy = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"));
        Assert.That(identitiy.Matches(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("C"))));
    }

    #endregion

    #region NullIdentity

    private static IEnumerable<IIdentity> AllIdentityKinds() =>
    [
        NullIdentity.Instance,
        new BatchIdentity("A"),
        CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B")),
    ];

    [TestCaseSource(nameof(AllIdentityKinds))]
    public void MatchedBy_OnNullIdentity_ReturnsTrue(IIdentity candidate)
    {
        Assert.That(NullIdentity.Instance.MatchedBy(candidate));
    }

    private static IEnumerable<TestCaseData> MatchesOnNullIdentityCases() =>
    [
        new TestCaseData(NullIdentity.Instance, true),
        new TestCaseData(new BatchIdentity("A"), false),
        new TestCaseData(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B")), false),
    ];

    [TestCaseSource(nameof(MatchesOnNullIdentityCases))]
    public void Matches_OnNullIdentity(IIdentity required, bool expected)
    {
        Assert.That(NullIdentity.Instance.Matches(required), Is.EqualTo(expected));
    }

    private static IEnumerable<IIdentity> NonNullIdentityKinds() =>
    [
        new BatchIdentity("A"),
        CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B")),
    ];

    [TestCaseSource(nameof(NonNullIdentityKinds))]
    public void Matches_WithNullIdentityRequired_ReturnsTrue(IIdentity identitiy)
    {
        // requiring nothing is satisfied by any identity
        Assert.That(identitiy.Matches(NullIdentity.Instance));
    }

    [TestCaseSource(nameof(NonNullIdentityKinds))]
    public void MatchedBy_WithNullIdentityCandidate_ReturnsFalse(IIdentity identitiy)
    {
        // absence cannot cover a real identity requirement
        Assert.That(identitiy.MatchedBy(NullIdentity.Instance), Is.False);
    }

    [Test]
    public void Matches_IsInverseOfMatchedBy_ForNullIdentity()
    {
        IIdentity nullId = NullIdentity.Instance;
        IIdentity someId = new BatchIdentity("A");

        Assert.Multiple(() =>
        {
            Assert.That(nullId.Matches(nullId), Is.EqualTo(nullId.MatchedBy(nullId)));
            Assert.That(nullId.Matches(someId), Is.EqualTo(someId.MatchedBy(nullId)));
            Assert.That(someId.Matches(nullId), Is.EqualTo(nullId.MatchedBy(someId)));
        });
    }

    #endregion

    #region MatchedBy

    [Test]
    public void MatchedBy_OnSingleIdentity_WithEqualCandidate_ReturnsTrue()
    {
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.MatchedBy(new BatchIdentity("A")));
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithDifferentCandidate_ReturnsFalse()
    {
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.MatchedBy(new BatchIdentity("B")), Is.False);
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithSingleEntryCombinedCandidate_ReturnsTrue()
    {
        // combined([A]).Equals(singleA) = true via CombinedIdentity.EqualsSingleIdentity
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.MatchedBy(CombinedIdentity.From(new BatchIdentity("A"))));
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithSupersetCombinedCandidate_ReturnsTrue()
    {
        // {A} ⊆ {A, B} — the candidate covers this identity
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.MatchedBy(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"))));
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithCombinedCandidateNotContainingThis_ReturnsFalse()
    {
        IIdentity identitiy = new BatchIdentity("A");
        Assert.That(identitiy.MatchedBy(CombinedIdentity.From(new BatchIdentity("B"), new BatchIdentity("C"))), Is.False);
    }

    [Test]
    public void MatchedBy_OnCombinedIdentity_WithFullSetCandidate_ReturnsTrue()
    {
        var a = new BatchIdentity("A");
        var b = new BatchIdentity("B");
        var identitiy = CombinedIdentity.From(a, b);
        Assert.That(identitiy.MatchedBy(CombinedIdentity.From(a, b)));
    }

    [Test]
    public void MatchedBy_OnCombinedIdentity_WithSupersetCombinedCandidate_ReturnsTrue()
    {
        var identitiy = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"));
        Assert.That(identitiy.MatchedBy(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"))));
    }

    [Test]
    public void MatchedBy_OnCombinedIdentity_WithSubsetCombinedCandidate_ReturnsFalse()
    {
        var identitiy = CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"), new BatchIdentity("C"));
        Assert.That(identitiy.MatchedBy(CombinedIdentity.From(new BatchIdentity("A"), new BatchIdentity("B"))), Is.False);
    }

    #endregion
}
