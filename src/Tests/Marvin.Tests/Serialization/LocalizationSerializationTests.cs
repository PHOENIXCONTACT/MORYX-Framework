using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using Marvin.Serialization;
using Marvin.Tests.Serialization;
using NUnit.Framework;

namespace Marvin.Tests
{
    [TestFixture]
    public class LocalizationSerializationTests
    {
        private readonly CultureInfo _germanCulture = new CultureInfo("de");
        private readonly CultureInfo _invariantCulture = new CultureInfo("en");

        [Test]
        public void Localization()
        {
            var resourceManager = new ResourceManager(typeof(strings));
            var germanCulture = new CultureInfo("de");
            var invariantCulture = new CultureInfo("en");

            // Switch to german
            Thread.CurrentThread.CurrentUICulture = germanCulture;
            var expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), germanCulture);
            var expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), germanCulture);
            var encoded = EntryConvert.EncodeClass(typeof(LocalizedClass));

            Assert.AreEqual(expectedDisplayPropName, encoded.SubEntries[0].Key.Name);
            Assert.AreEqual(expectedDisplayPropDescription, encoded.SubEntries[0].Description);
            Assert.AreEqual(LocalizedClass.PropDisplayNameAttributeDisplayName, encoded.SubEntries[1]);

            // Switch to invariant
            Thread.CurrentThread.CurrentUICulture = invariantCulture;
            expectedDisplayPropName = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Name), invariantCulture);
            expectedDisplayPropDescription = resourceManager.GetString(nameof(strings.PropDisplayAttribute_Description), germanCulture);
            encoded = EntryConvert.EncodeClass(typeof(LocalizedClass));

            Assert.AreEqual(expectedDisplayPropName, encoded.SubEntries[0].Key.Name);
            Assert.AreEqual(expectedDisplayPropDescription, encoded.SubEntries[0].Description);
            Assert.AreEqual(LocalizedClass.PropDisplayNameAttributeDisplayName, encoded.SubEntries[1]);
        }

        [Test]
        public void PropertyLocalization()
        {
            // Arrange
            var dummyClass = new DummyClass();

            // Act
            Thread.CurrentThread.CurrentUICulture = _germanCulture;
            var entriesGerman = EntryConvert.EncodeObject(dummyClass);

            Thread.CurrentThread.CurrentUICulture = _invariantCulture;
            var entriesInvariant = EntryConvert.EncodeObject(dummyClass);

            // Assert
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.CrashTestDummy), _germanCulture), entriesGerman.Key.Name);
            Assert.AreEqual(null, entriesGerman.Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.Number), _germanCulture), entriesGerman.SubEntries[0].Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NumberDescription), _germanCulture), entriesGerman.SubEntries[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.SubClass), _germanCulture), entriesGerman.SubEntries[3].Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.SubClassDescription), _germanCulture), entriesGerman.SubEntries[3].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.CrashTestDummy), _invariantCulture), entriesInvariant.Key.Name);
            Assert.AreEqual(null, entriesInvariant.Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.Number), _invariantCulture), entriesInvariant.SubEntries[0].Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NumberDescription), _invariantCulture), entriesInvariant.SubEntries[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.SubClass), _invariantCulture), entriesInvariant.SubEntries[3].Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.SubClassDescription), _invariantCulture), entriesInvariant.SubEntries[3].Description);
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
            Assert.AreEqual(nameof(strings.SubClass), entriesGerman.SubEntries[14].Key.Name);
            Assert.AreEqual(nameof(strings.SubClassDescription), entriesGerman.SubEntries[14].Description);
        }

        [Test]
        public void LocalizedClassDisplay()
        {
            // Arrange
            var subClass = new SubClass();

            // Act
            Thread.CurrentThread.CurrentUICulture = _germanCulture;
            var entriesGerman = EntryConvert.EncodeObject(subClass);

            Thread.CurrentThread.CurrentUICulture = _invariantCulture;
            var entriesInvariant = EntryConvert.EncodeObject(subClass);

            // Assert
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NoShownAsProperty), _germanCulture), entriesGerman.Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NoShownAsPropertyDescription), _germanCulture), entriesGerman.Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NoShownAsProperty), _invariantCulture), entriesInvariant.Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NoShownAsPropertyDescription), _invariantCulture), entriesInvariant.Description);
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

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _germanCulture), entriesGerman[0].Parameters.SubEntries[0].Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _germanCulture), entriesGerman[0].Parameters.SubEntries[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _germanCulture), entriesGerman[0].Parameters.SubEntries[1].Key.Name);
            Assert.IsNull(entriesGerman[0].Parameters.SubEntries[1].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTermination), _invariantCulture), entriesInvariant[0].DisplayName);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.InitiateWorldTerminationDescription), _invariantCulture), entriesInvariant[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParam), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[0].Key.Name);
            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.EvacuatePeopleParamDescription), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[0].Description);

            Assert.AreEqual(strings.ResourceManager.GetString(nameof(strings.NameOfTerminatorParam), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[1].Key.Name);
            Assert.IsNull(entriesInvariant[0].Parameters.SubEntries[1].Description);
        }
    }
}
