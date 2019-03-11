using System.Globalization;
using System.Linq;
using System.Threading;
using Marvin.Serialization;
using Marvin.Tests.Properties;
using NUnit.Framework;

namespace Marvin.Tests
{
    [TestFixture]
    public class LocalizationSerializationTests
    {
        private readonly CultureInfo _germanCulture = new CultureInfo("de");
        private readonly CultureInfo _invariantCulture = new CultureInfo("en");

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
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.CrashTestDummy), _germanCulture), entriesGerman.Key.Name);
            Assert.AreEqual(null, entriesGerman.Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.Number), _germanCulture), entriesGerman.SubEntries[0].Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NumberDescription), _germanCulture), entriesGerman.SubEntries[0].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.SubClass), _germanCulture), entriesGerman.SubEntries[4].Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.SubClassDescription), _germanCulture), entriesGerman.SubEntries[4].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.CrashTestDummy), _invariantCulture), entriesInvariant.Key.Name);
            Assert.AreEqual(null, entriesInvariant.Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.Number), _invariantCulture), entriesInvariant.SubEntries[0].Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NumberDescription), _invariantCulture), entriesInvariant.SubEntries[0].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.SubClass), _invariantCulture), entriesInvariant.SubEntries[4].Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.SubClassDescription), _invariantCulture), entriesInvariant.SubEntries[4].Description);
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
            Assert.AreEqual(nameof(Localization.SubClass), entriesGerman.SubEntries[3].Key.Name);
            Assert.AreEqual(nameof(Localization.SubClassDescription), entriesGerman.SubEntries[3].Description);
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
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NoShownAsProperty), _germanCulture), entriesGerman.Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NoShownAsPropertyDescription), _germanCulture), entriesGerman.Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NoShownAsProperty), _invariantCulture), entriesInvariant.Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NoShownAsPropertyDescription), _invariantCulture), entriesInvariant.Description);
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
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.InitiateWorldTermination), _germanCulture), entriesGerman[0].DisplayName);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.InitiateWorldTerminationDescription), _germanCulture), entriesGerman[0].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.EvacuatePeopleParam), _germanCulture), entriesGerman[0].Parameters.SubEntries[0].Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.EvacuatePeopleParamDescription), _germanCulture), entriesGerman[0].Parameters.SubEntries[0].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NameOfTerminatorParam), _germanCulture), entriesGerman[0].Parameters.SubEntries[1].Key.Name);
            Assert.IsNull(entriesGerman[0].Parameters.SubEntries[1].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.InitiateWorldTermination), _invariantCulture), entriesInvariant[0].DisplayName);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.InitiateWorldTerminationDescription), _invariantCulture), entriesInvariant[0].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.EvacuatePeopleParam), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[0].Key.Name);
            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.EvacuatePeopleParamDescription), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[0].Description);

            Assert.AreEqual(Localization.ResourceManager.GetString(nameof(Localization.NameOfTerminatorParam), _invariantCulture), entriesInvariant[0].Parameters.SubEntries[1].Key.Name);
            Assert.IsNull(entriesInvariant[0].Parameters.SubEntries[1].Description);
        }
    }
}
