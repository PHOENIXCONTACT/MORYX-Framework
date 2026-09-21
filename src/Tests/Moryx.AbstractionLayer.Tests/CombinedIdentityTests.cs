// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Identity;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class CombinedIdentityTests : IdentityTestBase
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
        new TestCaseData(new IIdentity[] {_aIdentity }, _aIdentity),
        new TestCaseData(new IIdentity[] { _aIdentity, NullIdentity.Instance }, _aIdentity),
    ];

    [TestCaseSource(nameof(SingleInputKinds))]
    public void From_WithSingleInput_ReturnsSingleIdentity(IEnumerable<IIdentity> singleInput, IIdentity singleOutput)
    {
        Assert.That(CombinedIdentity.From(singleInput), Is.SameAs(singleOutput));
    }

    [Test]
    public void From_WithMultipleIdentities_ReturnsCombinedIdentity()
    {
        var result = CombinedIdentity.From(_aIdentity, _bIdentity);

        Assert.That(result, Is.InstanceOf<CombinedIdentity>());
    }

    [Test]
    public void From_WithNullIdentityInMixedList_FiltersIt()
    {
        var result = CombinedIdentity.From([_aIdentity, NullIdentity.Instance, _bIdentity]);

        Assert.That(result.GetAll(), Has.Count.EqualTo(2).And.Contain(_aIdentity).And.Contain(_bIdentity));
    }

    #endregion

    #region Identifier

    [Test]
    public void Identifier_OfSingleEntry_ReturnsInnerIdentifier()
    {
        var identity = CombinedIdentity.From(new BatchIdentity("ABC"));

        Assert.That(identity.Identifier, Is.EqualTo("ABC"));
    }

    [Test]
    public void Identifier_OfMultipleEntries_JoinedWithPipeSeparator()
    {
        Assert.That(_abIdentity.Identifier, Is.EqualTo("A | B"));
    }

    #endregion

    #region Canonical ordering

    [Test]
    public void GetAll_WithSameType_OrderedAlphabeticallyByIdentifier()
    {
        // Arrange
        var identity = CombinedIdentity.From(_cIdentity, _bIdentity, _aIdentity);

        // Act
        var result = identity.GetAll();

        // Assert
        Assert.That(result, Is.EqualTo([_aIdentity, _bIdentity, _cIdentity]));
    }

    [Test]
    public void GetAll_WithDifferentTypes_OrderedByFullTypeName()
    {
        // Arrange
        var stubIdentity = new StubIdentity("A");

        // Act
        var identity = CombinedIdentity.From(stubIdentity, _aIdentity);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(identity.GetAll()[0], Is.InstanceOf<BatchIdentity>());
            Assert.That(identity.GetAll()[1], Is.InstanceOf<StubIdentity>());
        });
    }

    [Test]
    public void Identifier_ReflectsCanonicalOrder()
    {
        var identity = CombinedIdentity.From(_cIdentity, _aIdentity, _bIdentity);

        Assert.That(identity.Identifier, Is.EqualTo("A | B | C"));
    }

    #endregion

    #region Flattening

    [Test]
    public void From_WithNestedCombinedIdentity_IsFlattened()
    {
        // Arrange
        var identity = CombinedIdentity.From(_abIdentity, _cIdentity);

        // Act
        var result = identity.GetAll();

        // Assert
        Assert.That(result, Has.Count.EqualTo(3).And.Contain(_aIdentity).And.Contain(_bIdentity).And.Contain(_cIdentity));
    }

    [Test]
    public void From_WithDeeplyNestedCombinedIdentity_IsFullyFlattened()
    {
        // Arrange
        var level2 = CombinedIdentity.From(_abIdentity, _cIdentity);
        var newIdentity = new BatchIdentity("Z");
        var level3 = CombinedIdentity.From(level2, newIdentity);

        // Act
        var result = level3.GetAll();

        // Assert
        Assert.That(result, Has.Count.EqualTo(4).And.Contain(_aIdentity).And.Contain(_bIdentity)
            .And.Contain(_cIdentity).And.Contain(newIdentity));
    }

    #endregion

    #region SetIdentifier

    [Test]
    public void SetIdentifier_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => _abIdentity.SetIdentifier("X"));
    }

    #endregion

    #region Equals

    [Test]
    public void Equals_WithSameIdentities_ReturnsTrue()
    {
        Assert.That(_abIdentity.Equals(_abIdentity));
    }

    [Test]
    public void Equals_WithDifferentIdentifierValues_ReturnsFalse()
    {
        var acIdentity = CombinedIdentity.From(_aIdentity, _cIdentity);

        Assert.That(_abIdentity.Equals(acIdentity), Is.False);
    }

    [Test]
    public void Equals_WithDifferentCount_ReturnsFalse()
    {
        Assert.That(_abIdentity.Equals(_abcIdentity), Is.False);
    }

    [Test]
    public void Equals_WithNull_ReturnsFalse()
    {
        Assert.That(_abIdentity.Equals(null), Is.False);
    }

    #endregion

    #region GetHashCode

    [Test]
    public void GetHashCode_EqualInstances_ReturnSameValue()
    {
        Assert.That(_abIdentity.GetHashCode(), Is.EqualTo(_abIdentity.GetHashCode()));
    }

    #endregion

    #region MatchedBy

    [Test]
    public void MatchedBy_WithSingleCandidateWhenCurrentHasMultiple_ReturnsFalse()
    {
        Assert.That(_abIdentity.MatchedBy(_aIdentity), Is.False);
    }

    [Test]
    public void MatchedBy_WithCombinedSupersetCandidate_ReturnsTrue()
    {
        Assert.That(_abIdentity.MatchedBy(_abcIdentity));
    }

    [Test]
    public void MatchedBy_WithFullSetCandidate_ReturnsTrue()
    {
        Assert.That(_abIdentity.MatchedBy(_abIdentity));
    }

    [Test]
    public void MatchedBy_WithCombinedSubsetCandidate_ReturnsFalse()
    {
        Assert.That(_abcIdentity.MatchedBy(_abIdentity), Is.False);
    }

    #endregion

    #region Matches

    [Test]
    public void Matches_WithSingleIdentityContained_ReturnsTrue()
    {
        Assert.That(_abIdentity.Matches(_bIdentity));
    }

    [Test]
    public void Matches_WithSingleIdentityNotContained_ReturnsFalse()
    {
        Assert.That(_abIdentity.Matches(_cIdentity), Is.False);
    }

    [Test]
    public void Matches_WithSubsetRequirement_ReturnsTrue()
    {
        Assert.That(_abcIdentity.Matches(_abIdentity));
    }

    [Test]
    public void Matches_WithFullSetRequirement_ReturnsTrue()
    {
        Assert.That(_abIdentity.Matches(_abIdentity));
    }

    [Test]
    public void Matches_WithRequirementContainsUnknownIdentity_ReturnsFalse()
    {
        var acIdentity = CombinedIdentity.From(_aIdentity, _cIdentity);

        Assert.That(_abIdentity.Matches(acIdentity), Is.False);
    }

    #endregion

    private sealed class StubIdentity(string identifier) : IIdentity
    {
        public string Identifier { get; } = identifier;
        public void SetIdentifier(string id) => throw new NotSupportedException();
        public bool Equals(IIdentity other) => other is StubIdentity s && s.Identifier == Identifier;
    }
}
