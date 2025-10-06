// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using Moryx.Serialization;
using Moryx.Tests.Serialization;
using NUnit.Framework;

namespace Moryx.Tests
{
    [TestFixture]
    public class LocalizationSerializationTests
    {
        private readonly CultureInfo _germanCulture = new CultureInfo("de");
        private readonly CultureInfo _invariantCulture = new CultureInfo("en");

        [Test]
        public void PropertyLocalization()
        {
            var resourceManager = new ResourceManager(typeof(strings));
            var germanCulture = new CultureInfo("de");
            var invariantCulture = new CultureInfo("en");

            // Switch to german
            Thread.CurrentThread.CurrentUICulture = germanCulture;
            var expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), germanCulture);
            var expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), germanCulture);
            var encoded = EntryConvert.EncodeClass(typeof(LocalizedClass));

            Assert.That(encoded.SubEntries[0].DisplayName, Is.EqualTo(expectedDisplayPropName));
            Assert.That(encoded.SubEntries[0].Description, Is.EqualTo(expectedDisplayPropDescription));
            Assert.That(encoded.SubEntries[1].DisplayName, Is.EqualTo(LocalizedClass.PropDisplayNameAttributeDisplayName));

            // Switch to invariant
            Thread.CurrentThread.CurrentUICulture = invariantCulture;
            expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), invariantCulture);
            expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), invariantCulture);
            encoded = EntryConvert.EncodeClass(typeof(LocalizedClass));

            Assert.That(encoded.SubEntries[0].DisplayName, Is.EqualTo(expectedDisplayPropName));
            Assert.That(encoded.SubEntries[0].Description, Is.EqualTo(expectedDisplayPropDescription));
            Assert.That(encoded.SubEntries[1].DisplayName, Is.EqualTo(LocalizedClass.PropDisplayNameAttributeDisplayName));
        }

        [Test]
        public void NonLocalized()
        {
            // Arrange
            var dummyClass = new DummyClass();

            // Act
            Thread.CurrentThread.CurrentUICulture = _germanCulture;
            var entriesGerman = EntryConvert.EncodeObject(dummyClass);

            // Assert
            Assert.That(entriesGerman.SubEntries[14].DisplayName, Is.EqualTo(nameof(DummyClass.SingleClassNonLocalized)));
            Assert.That(entriesGerman.SubEntries[14].Description, Is.Null);
        }

        [Test]
        public void LocalizedClassDisplay()
        {
            // Arrange
            var subClass = new LocalizedClass();

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
            var methodClass = new LocalizedMethodDummyClass();

            // Act
            Thread.CurrentThread.CurrentUICulture = _germanCulture;
            var entriesGerman = EntryConvert.EncodeMethods(methodClass).ToArray();

            Thread.CurrentThread.CurrentUICulture = _invariantCulture;
            var entriesInvariant = EntryConvert.EncodeMethods(methodClass).ToArray();

            // Assert
            Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _germanCulture), Is.EqualTo(entriesGerman[0].DisplayName));
            Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _germanCulture), Is.EqualTo(entriesGerman[0].Description));
            Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _germanCulture), Is.EqualTo(entriesGerman[0].Parameters.SubEntries[0].DisplayName));
            Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _germanCulture), Is.EqualTo(entriesGerman[0].Parameters.SubEntries[0].Description));

            Assert.That(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _germanCulture), Is.EqualTo(entriesGerman[0].Parameters.SubEntries[1].DisplayName));
            Assert.That(entriesGerman[0].Parameters.SubEntries[1].Description, Is.Null);

            Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _invariantCulture), Is.EqualTo(entriesInvariant[0].DisplayName));
            Assert.That(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _invariantCulture), Is.EqualTo(entriesInvariant[0].Description));

            Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _invariantCulture), Is.EqualTo(entriesInvariant[0].Parameters.SubEntries[0].DisplayName));
            Assert.That(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _invariantCulture), Is.EqualTo(entriesInvariant[0].Parameters.SubEntries[0].Description));

            Assert.That(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _invariantCulture), Is.EqualTo(entriesInvariant[0].Parameters.SubEntries[1].DisplayName));
            Assert.That(entriesInvariant[0].Parameters.SubEntries[1].Description, Is.Null);
        }
    }
}
