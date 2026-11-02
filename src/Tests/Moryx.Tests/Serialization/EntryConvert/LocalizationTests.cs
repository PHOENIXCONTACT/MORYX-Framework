// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Resources;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with localized <see cref="DisplayAttribute"/>, <see cref="DisplayNameAttribute"/>
/// and <see cref="DescriptionAttribute"/> within the <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class LocalizationTests
{
    private readonly CultureInfo _germanCulture = new("de");
    private readonly CultureInfo _invariantCulture = new("en");

    [Test]
    public void PropertyLocalization()
    {
        var resourceManager = strings.ResourceManager;
        var germanCulture = new CultureInfo("de");
        var invariantCulture = new CultureInfo("en");

        // Switch to german
        Thread.CurrentThread.CurrentUICulture = germanCulture;
        var expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), germanCulture);
        var expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), germanCulture);
        var encoded = EntryConvert.EncodeClass(typeof(LocalizedClassDummy));

        Assert.That(encoded.SubEntry(nameof(LocalizedClassDummy.PropDisplayAttribute)).DisplayName, Is.EqualTo(expectedDisplayPropName));
        Assert.That(encoded.SubEntry(nameof(LocalizedClassDummy.PropDisplayAttribute)).Description, Is.EqualTo(expectedDisplayPropDescription));
        Assert.That(encoded.SubEntry(nameof(LocalizedClassDummy.PropDisplayNameAttribute)).DisplayName, Is.EqualTo(LocalizedClassDummy.PropDisplayNameAttributeDisplayName));

        // Switch to invariant
        Thread.CurrentThread.CurrentUICulture = invariantCulture;
        expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), invariantCulture);
        expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), invariantCulture);
        encoded = EntryConvert.EncodeClass(typeof(LocalizedClassDummy));

        Assert.That(encoded.SubEntry(nameof(LocalizedClassDummy.PropDisplayAttribute)).DisplayName, Is.EqualTo(expectedDisplayPropName));
        Assert.That(encoded.SubEntry(nameof(LocalizedClassDummy.PropDisplayAttribute)).Description, Is.EqualTo(expectedDisplayPropDescription));
        Assert.That(encoded.SubEntry(nameof(LocalizedClassDummy.PropDisplayNameAttribute)).DisplayName, Is.EqualTo(LocalizedClassDummy.PropDisplayNameAttributeDisplayName));
    }

    [Test]
    public void NonLocalized()
    {
        // Arrange
        var dummyClass = new ClassDummy();

        // Act
        Thread.CurrentThread.CurrentUICulture = _germanCulture;
        var entriesGerman = EntryConvert.EncodeObject(dummyClass);

        // Assert
        Assert.That(entriesGerman.SubEntry(nameof(ClassDummy.SingleClass)).DisplayName, Is.EqualTo(nameof(ClassDummy.SingleClass)));
        Assert.That(entriesGerman.SubEntry(nameof(ClassDummy.SingleClass)).Description, Is.Null);
    }

    [Test]
    public void LocalizedClassDisplay()
    {
        // Arrange
        var subClass = new LocalizedClassDummy();

        // Act
        Thread.CurrentThread.CurrentUICulture = _germanCulture;
        var entriesGerman = EntryConvert.EncodeObject(subClass);

        Thread.CurrentThread.CurrentUICulture = _invariantCulture;
        var entriesInvariant = EntryConvert.EncodeObject(subClass);

        // Assert
        Assert.That(strings.ResourceManager.GetString(nameof(strings.ClassName), _germanCulture), Is.EqualTo(entriesGerman.DisplayName));
        Assert.That(strings.ResourceManager.GetString(nameof(strings.ClassName), _invariantCulture), Is.EqualTo(entriesInvariant.DisplayName));
    }

    [Test]
    public void MethodLocalization()
    {
        // Arrange
        var methodClass = new LocalizedClassDummy();

        // Act
        Thread.CurrentThread.CurrentUICulture = _germanCulture;
        var entriesGerman = EntryConvert.EncodeMethods(methodClass).ToArray();

        Thread.CurrentThread.CurrentUICulture = _invariantCulture;
        var entriesInvariant = EntryConvert.EncodeMethods(methodClass).ToArray();

        // Assert
        Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _germanCulture), Is.EqualTo(entriesGerman[0].DisplayName));
        Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _germanCulture), Is.EqualTo(entriesGerman[0].Description));
        Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _germanCulture), Is.EqualTo(entriesGerman[0].Parameters.SubEntry("evacuatePeople").DisplayName));
        Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _germanCulture), Is.EqualTo(entriesGerman[0].Parameters.SubEntry("evacuatePeople").Description));

        Assert.That(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _germanCulture), Is.EqualTo(entriesGerman[0].Parameters.SubEntry("nameOfTerminator").DisplayName));
        Assert.That(entriesGerman[0].Parameters.SubEntry("nameOfTerminator").Description, Is.Null);

        Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _invariantCulture), Is.EqualTo(entriesInvariant[0].DisplayName));
        Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _invariantCulture), Is.EqualTo(entriesInvariant[0].Description));

        Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _invariantCulture), Is.EqualTo(entriesInvariant[0].Parameters.SubEntry("evacuatePeople").DisplayName));
        Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _invariantCulture), Is.EqualTo(entriesInvariant[0].Parameters.SubEntry("evacuatePeople").Description));

        Assert.That(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _invariantCulture), Is.EqualTo(entriesInvariant[0].Parameters.SubEntry("nameOfTerminator").DisplayName));
        Assert.That(entriesInvariant[0].Parameters.SubEntry("nameOfTerminator").Description, Is.Null);
    }
}
