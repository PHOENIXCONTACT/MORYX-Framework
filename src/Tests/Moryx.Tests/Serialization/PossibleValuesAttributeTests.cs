// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using Moryx.Configuration;
using Moryx.Serialization;
using Moryx.Tools;
using static Moryx.Tests.Serialization.EntryConvertTestHelper;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class PossibleValuesAttributeTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        ReflectionTool.TestMode = true;
        Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
    }

    [Test(Description = "Uses the " + nameof(PossibleTypesAttribute) + " to read possible type names from given types or baseType.")]
    public void PossibleTypesAttribute()
    {
        // Arrange
        var attrWithBase = new PossibleTypesAttribute(typeof(SomeBase));
        var attrWithArray = new PossibleTypesAttribute([typeof(SomeImpl)]);
        var attrWithFull = new PossibleTypesAttribute(typeof(SomeBase)) { UseFullname = true };

        // Act
        var valuesFromBase = attrWithBase.GetValues(null, null).ToArray();
        var valuesFromArray = attrWithArray.GetValues(null, null).ToArray();
        var valuesWithFull = attrWithFull.GetValues(null, null).ToArray();

        // Assert
        // Check values from base type
        Assert.That(valuesFromBase.Length, Is.EqualTo(2));
        Assert.That(valuesFromBase[0], Is.EqualTo(nameof(SomeBase)));
        Assert.That(valuesFromBase[1], Is.EqualTo(nameof(SomeImpl)));

        // Check values from array types
        Assert.That(valuesFromArray.Length, Is.EqualTo(1));
        Assert.That(valuesFromArray[0], Is.EqualTo(nameof(SomeImpl)));

        // Check values from base type with full name
        Assert.That(valuesWithFull.Length, Is.EqualTo(2));
        Assert.That(valuesWithFull[0], Is.EqualTo(typeof(SomeBase).FullName));
        Assert.That(valuesWithFull[1], Is.EqualTo(typeof(SomeImpl).FullName));
    }

    [TestCase(false)]
    [TestCase(true)]
    [NUnit.Framework.Description("Uses the " + nameof(PossibleTypesAttribute) + " to read possible types with display name and description, keyed like the values.")]
    public void PossibleTypesAttributeProvidesDisplayNames(bool useFullname)
    {
        // Arrange
        var attr = new PossibleTypesAttribute(typeof(SomeBase)) { UseFullname = useFullname };

        // Act
        var possibles = attr.GetPossibleValues(null, null).ToArray();
        var values = attr.GetValues(null, null).ToArray();

        // Assert
        var keys = possibles.Select(p => p.Key).ToArray();
        Assert.Multiple(() =>
        {
            Assert.That(keys, Is.EqualTo(values));
            Assert.That(possibles[0].DisplayName, Is.EqualTo(nameof(SomeBase)));
            Assert.That(possibles[0].Description, Is.Null);
            Assert.That(possibles[1].DisplayName, Is.EqualTo(SomeImpl.ImplDisplayName));
            Assert.That(possibles[1].Description, Is.EqualTo(SomeImpl.ImplDescription));
        });
    }

    [Test(Description = "Attributes without own implementation provide their values as possible values named by the key.")]
    public void PossibleValuesDefaultToValues()
    {
        // Arrange
        var attr = new StateMachineKeysAttribute(typeof(MyStateBase));

        // Act
        var possibles = attr.GetPossibleValues(null, null).ToArray();
        var values = attr.GetValues(null, null).ToArray();

        // Assert
        var keys = possibles.Select(p => p.Key).ToArray();
        var displayNames = possibles.Select(p => p.DisplayName).ToArray();
        Assert.That(keys, Is.EqualTo(values));
        Assert.That(displayNames, Is.EqualTo(values));
    }

    [Test(Description = "The " + nameof(PossibleValuesSerialization) + " uses the display names of the attribute for possible values.")]
    public void SerializationUsesDisplayNamesOfAttribute()
    {
        // Arrange
        var serialization = CreatePossibleValuesSerialization();

        // Act
        var encoded = EntryConvert.EncodeClass(typeof(TypeNameDummy), serialization);

        // Assert
        var possible = encoded.SubEntry(nameof(TypeNameDummy.TypeName)).Value.Possible;
        var displayNames = possible.Select(p => p.DisplayName).ToArray();
        Assert.That(displayNames, Is.EqualTo([nameof(SomeBase), SomeImpl.ImplDisplayName]));
    }

    [Test(Description = "Uses the " + nameof(StateMachineKeysAttribute) + " to read possible state keys from the given state machine type.")]
    public void StateMachineKeysAttribute()
    {
        // Arrange
        var attr = new StateMachineKeysAttribute(typeof(MyStateBase));

        // Act
        var possibleValues = attr.GetValues(null, null).ToArray();

        // Assert
        Assert.That(possibleValues.Length, Is.EqualTo(3));
        Assert.That(possibleValues[0], Is.EqualTo(nameof(MyStateBase.StateA)));
        Assert.That(possibleValues[1], Is.EqualTo(nameof(MyStateBase.StateB)));
        Assert.That(possibleValues[2], Is.EqualTo(nameof(MyStateBase.StateC)));
    }

    public class SomeBase
    {

    }

    [DisplayName(ImplDisplayName), System.ComponentModel.Description(ImplDescription)]
    public class SomeImpl : SomeBase
    {
        public const string ImplDisplayName = "Some implementation";

        public const string ImplDescription = "Implementation of some base";
    }

    public class TypeNameDummy
    {
        [PossibleTypes(typeof(SomeBase))]
        public string TypeName { get; set; }
    }
}
