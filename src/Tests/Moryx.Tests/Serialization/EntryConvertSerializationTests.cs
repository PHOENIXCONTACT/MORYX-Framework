// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using NUnit.Framework;
using System.Linq;

namespace Moryx.Tests.Serialization
{
    [TestFixture]
    public class EntryConvertSerializationTests
    {
        [Test]
        public void ParameterWithNoDefaultValueShouldHaveValidation_IsRequired()
        {
            // Arrange
            var myClass = typeof(EntrySerialize_Methods);
            var myMethodWithParameters = myClass.GetMethods().FirstOrDefault(x => x.GetParameters().Length > 0);
            const string parameter1Name = "intValue";
            const string parameter2Name = "stringValue1";
            const string parameter3Name = "stringValue2";

            // Act
            var entry = EntryConvert.EncodeMethod(myMethodWithParameters);
            var parameter1Validation = entry.Parameters.SubEntries.FirstOrDefault(x => x.DisplayName == parameter1Name).Validation;
            var parameter2Validation = entry.Parameters.SubEntries.FirstOrDefault(x => x.DisplayName == parameter2Name).Validation;
            var parameter3Validation = entry.Parameters.SubEntries.FirstOrDefault(x => x.DisplayName == parameter3Name).Validation;

            // Assert
            Assert.That(parameter1Validation.IsRequired, Is.True);
            Assert.That(parameter2Validation.IsRequired, Is.True);
            Assert.That(parameter3Validation.IsRequired, Is.False); // parameter with default value is not required
        }

        [Test]
        public void ShouldEncodeClass_Properties()
        {
            // Act
            var entry = EntryConvert.EncodeClass(typeof(DummyClass));

            // Assert
            Assert.That(entry.SubEntries.FirstOrDefault(x => x.DisplayName == nameof(DummyClass.Number)) != null, Is.True);
        }

        [Test]
        public void ShouldEncodeObject_PropertyWithValue()
        {
            // Arrange
            var myObject = new DummyClass();
            myObject.Number = 10;

            // Act
            var entry = EntryConvert.EncodeObject(myObject);

            // Assert
            Assert.That(entry.SubEntries
                .FirstOrDefault(x => x.DisplayName == nameof(DummyClass.Number)).Value.Current,
                Is.EqualTo(myObject.Number.ToString()));
        }
    }
}

