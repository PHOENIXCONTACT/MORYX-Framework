// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Configuration;
using Moryx.Serialization;
using NUnit.Framework;
using static Moryx.Tests.Serialization.EntryConvertTestHelper;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the handling of <see cref="PossibleValuesAttribute"/>s,
/// <see cref="AllowedValuesAttribute"/> and <see cref="DeniedValuesAttribute"/> within the <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class PossibleValuesTests
{
    [TestCase(false)]
    [TestCase(true)]
    public void PrimitiveCollectionPrototypes(bool possibleValues)
    {
        // Arrange
        var serialization = possibleValues
            ? CreatePossibleValuesSerialization(new ValueProviderExecutorSettings().AddDefaultValueProvider())
            : (ICustomSerialization)new DefaultSerialization();

        // Act
        var encoded = EntryConvert.EncodeClass(typeof(ListDummy), serialization);

        // Assert
        var doubles = encoded.SubEntry(nameof(ListDummy.DoubleList));
        Assert.That(doubles.Value.Possible[0].Key, Is.EqualTo(nameof(EntryValueType.Double)));
        Assert.That(doubles.Prototypes.Count, Is.EqualTo(1));
        Assert.That(doubles.Prototypes[0].Value.Possible.Length, Is.EqualTo(3));

        var enums = encoded.SubEntry(nameof(ListDummy.EnumList));
        Assert.That(enums.Value.Possible.Length, Is.EqualTo(3));
        Assert.That(enums.Prototypes.Count, Is.EqualTo(3));
        for (int i = 0; i < 3; i++)
            Assert.That(enums.Prototypes[i].Value.Current, Is.EqualTo(enums.Value.Possible[i].Key));
    }

    [Test]
    public void ShouldProvidePossibleValue_UsingContext()
    {
        //Arrange
        var myObject = new PossibleValuesWithContextClassDummy
        {
            AllowedValues = ["a", "b", "c"]
        };
        var serialization = CreatePossibleValuesSerialization();

        // Act
        var converted = EntryConvert.EncodeObject(myObject, serialization);

        //Assert
        Assert.That(converted.SubEntry(nameof(PossibleValuesWithContextClassDummy.SelectedValue)).Value.Possible.Length, Is.EqualTo(3));
    }

    [Test(Description = "EncodeMethod passes instance to PossibleValues context for method parameters.")]
    public void PossibleValuesForMethodParametersUsingContext()
    {
        // Arrange
        var myObject = new PossibleValuesWithContextClassDummy
        {
            AllowedValues = ["x", "y"]
        };
        var serialization = CreatePossibleValuesSerialization();

        // Act
        var methods = EntryConvert.EncodeMethods(myObject, serialization).ToArray();
        var doSomething = methods.First(m => m.Name == nameof(PossibleValuesWithContextClassDummy.DoSomething));

        // Assert
        var parameterPossible = doSomething.Parameters.SubEntry("value").Value.Possible;
        Assert.That(parameterPossible.Length, Is.EqualTo(2));
        Assert.That(parameterPossible.Select(p => p.Key), Is.EquivalentTo(new[] { "x", "y" }));
    }

    [Test]
    public void AllowedEnumValuesTest()
    {
        // Arrange
        var encoded = EntryConvert.EncodeClass(typeof(AllowedDeniedValuesClassDummy));

        // Assert
        var possible = encoded.SubEntry(nameof(AllowedDeniedValuesClassDummy.AllowedValues)).Value.Possible;
        Assert.That(possible, Has.Exactly(2).Items);

        string[] expected = [nameof(EnumDummy.ValueA), nameof(EnumDummy.ValueB)];
        Assert.That(possible.Select(p => p.Key), Is.SupersetOf(expected));
    }

    [Test]
    public void DeniedEnumValuesTest()
    {
        // Arrange
        var encoded = EntryConvert.EncodeClass(typeof(AllowedDeniedValuesClassDummy));

        // Assert
        var possible = encoded.SubEntry(nameof(AllowedDeniedValuesClassDummy.DeniedValues)).Value.Possible;
        Assert.That(possible, Has.Exactly(2).Items);

        string[] expected = [nameof(EnumDummy.ValueA), nameof(EnumDummy.ValueB)];
        Assert.That(possible.Select(p => p.Key), Is.SupersetOf(expected));
    }

    [Test(Description = "Enum possible values should use DisplayName and Description from attribute")]
    public void EnumPossibleValuesHaveDisplayNameAndDescription()
    {
        // Arrange
        var encoded = EntryConvert.EncodeClass(typeof(SubClassDummy));

        // Act
        var possibles = encoded.SubEntry(nameof(SubClassDummy.Enum)).Value.Possible;

        // Assert
        Assert.That(possibles, Has.Exactly(3).Items);

        Assert.That(possibles[0].Key, Is.EqualTo(nameof(EnumDummy.Unset)));
        Assert.That(possibles[0].DisplayName, Is.EqualTo(DisplayNameEnumStrings.UnsetDisplayName));
        Assert.That(possibles[0].Description, Is.EqualTo(DisplayNameEnumStrings.UnsetDescription));

        Assert.That(possibles[1].Key, Is.EqualTo(nameof(EnumDummy.ValueA)));
        Assert.That(possibles[1].DisplayName, Is.EqualTo(DisplayNameEnumStrings.ValueADisplayName));

        Assert.That(possibles[2].Key, Is.EqualTo(nameof(EnumDummy.ValueB)));
        Assert.That(possibles[2].DisplayName, Is.EqualTo(nameof(EnumDummy.ValueB)));
    }

    [Test]
    public void AllowedIntValuesTest()
    {
        // Arrange
        var encoded = EntryConvert.EncodeClass(typeof(AllowedDeniedValuesClassDummy));

        // Assert
        var possible = encoded.SubEntry(nameof(AllowedDeniedValuesClassDummy.AllowedIntValues)).Value.Possible;
        Assert.That(possible, Has.Exactly(3).Items);
        string[] expected = ["1", "5", "20"];
        Assert.That(possible.Select(p => p.Key), Is.SupersetOf(expected));
    }
}
