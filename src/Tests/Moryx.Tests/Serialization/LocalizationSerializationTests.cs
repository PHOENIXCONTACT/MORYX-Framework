// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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

            Assert.AreEqual(expectedDisplayPropName, encoded.SubEntries[0].DisplayName);
            Assert.AreEqual(expectedDisplayPropDescription, encoded.SubEntries[0].Description);
            Assert.AreEqual(LocalizedClass.PropDisplayNameAttributeDisplayName, encoded.SubEntries[1].DisplayName);

            // Switch to invariant
            Thread.CurrentThread.CurrentUICulture = invariantCulture;
            expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), invariantCulture);
            expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), invariantCulture);
            encoded = EntryConvert.EncodeClass(typeof(LocalizedClass));

            Assert.AreEqual(expectedDisplayPropName, encoded.SubEntries[0].DisplayName);
            Assert.AreEqual(expectedDisplayPropDescription, encoded.SubEntries[0].Description);
            Assert.AreEqual(LocalizedClass.PropDisplayNameAttributeDisplayName, encoded.SubEntries[1].DisplayName);
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
            Assert.AreEqual(nameof(DummyClass.SingleClassNonLocalized), entriesGerman.SubEntries[14].DisplayName);
            Assert.IsNull(entriesGerman.SubEntries[14].Description);
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
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.ClassName), _germanCulture), entriesGerman.DisplayName);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.ClassName), _invariantCulture), entriesInvariant.DisplayName);
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
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _germanCulture), entriesGerman[0].DisplayName);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _germanCulture), entriesGerman[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _germanCulture), entriesGerman[0].Parameters.SubEntries[0].DisplayName);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _germanCulture), entriesGerman[0].Parameters.SubEntries[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _germanCulture), entriesGerman[0].Parameters.SubEntries[1].DisplayName);
            Assert.IsNull(entriesGerman[0].Parameters.SubEntries[1].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _invariantCulture), entriesInvariant[0].DisplayName);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _invariantCulture), entriesInvariant[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[0].DisplayName);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[1].DisplayName);
            Assert.IsNull(entriesInvariant[0].Parameters.SubEntries[1].Description);
        }
    }
}
