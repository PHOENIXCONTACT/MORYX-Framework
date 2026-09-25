// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class IdentityExtensionsTests : IdentityTestBase
{
    #region Combine

    [Test]
    public void Combine_WithSingleOther_CreatesCombinedContainingBoth()
    {
        var result = _aIdentity.Combine(_bIdentity);

        Assert.That(result.GetAll(), Has.Count.EqualTo(2).And.Contain(_aIdentity).And.Contain(_bIdentity));
    }

    [Test]
    public void Combine_WithMultipleOthers_CreatesCombinedContainingAll()
    {
        var result = _aIdentity.Combine(_bIdentity, _cIdentity);

        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(_aIdentity).And.Contain(_bIdentity).And.Contain(_cIdentity));
    }

    [Test]
    public void Combine_WithEnumerableOthers_CreatesCombinedContainingAll()
    {
        // Arrange
        IEnumerable<IIdentity> others = [_bIdentity, _cIdentity];

        // Act
        var result = _aIdentity.Combine(others);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(_aIdentity).And.Contain(_bIdentity).And.Contain(_cIdentity));
    }

    [Test]
    public void Combine_WhenReceiverIsCombined_FlattensResult()
    {
        var result = _abIdentity.Combine(_cIdentity);

        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(_aIdentity).And.Contain(_bIdentity).And.Contain(_cIdentity));
    }

    [Test]
    public void Combine_WhenArgumentIsCombined_FlattensResult()
    {
        // Arrange
        var inner = CombinedIdentity.From(_bIdentity, _cIdentity);

        // Act
        var result = _aIdentity.Combine(inner);

        // Assert
        Assert.That(result.GetAll(), Has.Count.EqualTo(3).And.Contain(_aIdentity).And.Contain(_bIdentity).And.Contain(_cIdentity));
    }

    [Test]
    public void Combine_WithNullIdentityArgument_FiltersIt()
    {
        var result = _aIdentity.Combine(NullIdentity.Instance, _bIdentity);

        Assert.That(result.GetAll(), Has.Count.EqualTo(2).And.Contain(_aIdentity).And.Contain(_bIdentity));
    }

    #endregion

    #region GetAll

    [Test]
    public void GetAll_OnSingleIdentity_ReturnsSingleElementList()
    {
        var result = _aIdentity.GetAll();

        Assert.That(result, Is.EqualTo([_aIdentity]));
    }

    [Test]
    public void GetAll_OnCombinedIdentity_ReturnsAllConstituents()
    {
        var result = _abIdentity.GetAll();

        Assert.That(result, Has.Count.EqualTo(2).And.Contain(_aIdentity).And.Contain(_bIdentity));
    }

    #endregion

    #region Matches

    [Test]
    public void Matches_OnSingleIdentity_WithEqualRequired_ReturnsTrue()
    {
        Assert.That(_aIdentity.Matches(_aIdentity));
    }

    [Test]
    public void Matches_OnSingleIdentity_WithDifferentRequired_ReturnsFalse()
    {
        Assert.That(_aIdentity.Matches(_bIdentity), Is.False);
    }

    [Test]
    public void Matches_OnSingleIdentity_WithSingleEntryCombinedRequired_ReturnsTrue()
    {
        Assert.That(_aIdentity.Matches(CombinedIdentity.From(_aIdentity)));
    }

    [Test]
    public void Matches_OnSingleIdentity_WithMultiEntryCombinedRequired_ReturnsFalse()
    {
        Assert.That(_aIdentity.Matches(_abIdentity), Is.False);
    }

    [Test]
    public void Matches_OnCombinedIdentity_WithContainedSingle_ReturnsTrue()
    {
        Assert.That(_abIdentity.Matches(_aIdentity));
    }

    [Test]
    public void Matches_OnCombinedIdentity_WithNotContainedSingle_ReturnsFalse()
    {
        Assert.That(_abIdentity.Matches(_cIdentity), Is.False);
    }

    [Test]
    public void Matches_OnCombinedIdentity_WithSubsetCombined_ReturnsTrue()
    {
        Assert.That(_abcIdentity.Matches(CombinedIdentity.From(_aIdentity, _cIdentity)));
    }

    #endregion

    #region NullIdentity

    private static IEnumerable<IIdentity> AllIdentityKinds() =>
    [
        NullIdentity.Instance,
        _aIdentity,
        _abIdentity,
    ];

    [TestCaseSource(nameof(AllIdentityKinds))]
    public void MatchedBy_OnNullIdentity_ReturnsTrue(IIdentity candidate)
    {
        Assert.That(NullIdentity.Instance.MatchedBy(candidate));
    }

    private static IEnumerable<TestCaseData> MatchesOnNullIdentityCases() =>
    [
        new TestCaseData(NullIdentity.Instance, true),
        new TestCaseData(_aIdentity, false),
        new TestCaseData(_abIdentity, false),
    ];

    [TestCaseSource(nameof(MatchesOnNullIdentityCases))]
    public void Matches_OnNullIdentity(IIdentity required, bool expected)
    {
        Assert.That(NullIdentity.Instance.Matches(required), Is.EqualTo(expected));
    }

    private static IEnumerable<IIdentity> NonNullIdentityKinds() =>
    [
        _aIdentity,
        _abIdentity,
    ];

    [TestCaseSource(nameof(NonNullIdentityKinds))]
    public void Matches_WithNullIdentityRequired_ReturnsTrue(IIdentity identity)
    {
        Assert.That(identity.Matches(NullIdentity.Instance));
    }

    [TestCaseSource(nameof(NonNullIdentityKinds))]
    public void MatchedBy_WithNullIdentityCandidate_ReturnsFalse(IIdentity identity)
    {
        Assert.That(identity.MatchedBy(NullIdentity.Instance), Is.False);
    }

    [Test]
    public void Matches_IsInverseOfMatchedBy_ForNullIdentity()
    {
        IIdentity nullId = NullIdentity.Instance;

        Assert.Multiple(() =>
        {
            Assert.That(nullId.Matches(nullId), Is.EqualTo(nullId.MatchedBy(nullId)));
            Assert.That(nullId.Matches(_aIdentity), Is.EqualTo(_aIdentity.MatchedBy(nullId)));
            Assert.That(_aIdentity.Matches(nullId), Is.EqualTo(nullId.MatchedBy(_aIdentity)));
        });
    }

    #endregion

    #region MatchedBy

    [Test]
    public void MatchedBy_OnSingleIdentity_WithEqualCandidate_ReturnsTrue()
    {
        Assert.That(_aIdentity.MatchedBy(_aIdentity));
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithDifferentCandidate_ReturnsFalse()
    {
        Assert.That(_aIdentity.MatchedBy(_bIdentity), Is.False);
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithSingleEntryCombinedCandidate_ReturnsTrue()
    {
        Assert.That(_aIdentity.MatchedBy(CombinedIdentity.From(_aIdentity)));
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithSupersetCombinedCandidate_ReturnsTrue()
    {
        Assert.That(_aIdentity.MatchedBy(_abIdentity));
    }

    [Test]
    public void MatchedBy_OnSingleIdentity_WithCombinedCandidateNotContainingThis_ReturnsFalse()
    {
        Assert.That(_aIdentity.MatchedBy(CombinedIdentity.From(_bIdentity, _cIdentity)), Is.False);
    }

    [Test]
    public void MatchedBy_OnCombinedIdentity_WithFullSetCandidate_ReturnsTrue()
    {
        Assert.That(_abIdentity.MatchedBy(_abIdentity));
    }

    [Test]
    public void MatchedBy_OnCombinedIdentity_WithSupersetCombinedCandidate_ReturnsTrue()
    {
        Assert.That(_abIdentity.MatchedBy(_abcIdentity));
    }

    [Test]
    public void MatchedBy_OnCombinedIdentity_WithSubsetCombinedCandidate_ReturnsFalse()
    {
        Assert.That(_abcIdentity.MatchedBy(_abIdentity), Is.False);
    }

    #endregion
}
