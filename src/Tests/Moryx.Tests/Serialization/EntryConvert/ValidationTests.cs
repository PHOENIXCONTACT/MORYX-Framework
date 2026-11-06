// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the handling of <see cref="ValidationAttribute"/>s within the
/// <see cref="EntryConvert"/>
/// </summary>
[TestFixture]
public class ValidationTests
{
    private Entry _encodedValidationDummy;

    [SetUp]
    public void Setup()
    {
        // Act
        var encoded = EntryConvert.EncodeClass(typeof(ValidationDummy));

        // Assert
        Assert.That(encoded, Is.Not.Null);
        Assert.That(encoded.SubEntries.Count, Is.EqualTo(8));
        _encodedValidationDummy = encoded;
    }

    [Test(Description = "Base64StringAttribute must be used to set EntryValueType in EntryEntryValue")]
    public void ValidateBase64StringAttribute()
    {
        // Arrange / Act
        var entry = _encodedValidationDummy.SubEntry(nameof(ValidationDummy.Base64String));

        // Assert
        Assert.That(entry.Value.UnitType, Is.EqualTo(EntryUnitType.Base64));
    }

    [Test(Description = "DataType must be set in EntryValidation")]
    public void ValidateDateTypeAttribute()
    {
        // Arrange / Act
        var emailValidation = GetValidation(nameof(ValidationDummy.EmailDataType));

        // Assert
        Assert.That(emailValidation.DataType, Is.EqualTo(DataType.EmailAddress));
    }

    [Test(Description = "RequiredAttribute must be noted in EntryValidation")]
    public void ValidateRequiredAttribute()
    {
        // Arrange / Act
        var requiredBoolValidation = GetValidation(nameof(ValidationDummy.RequiredBool));

        // Assert
        Assert.That(requiredBoolValidation.IsRequired, Is.EqualTo(true));
    }

    [Test(Description = "RegexAttribute must be noted in EntryValidation")]
    public void ValidateRegexAttribute()
    {
        // Arrange / Act
        var regexValidation = GetValidation(nameof(ValidationDummy.RegexString));

        // Assert
        Assert.That(regexValidation.Regex, Is.EqualTo(ValidationDummy.Regex));
    }

    [Test(Description = "Attributes defining Length (min/max) must be noted in EntryValidation")]
    public void ValidateLenghtAttribute()
    {
        // Arrange / Act
        var minMaxStringLenghtStringValidation = GetValidation(nameof(ValidationDummy.MinMaxStringLenghtString));
        var minMaxLenghtStringValidation = GetValidation(nameof(ValidationDummy.MinMaxLenghtString));
        var minLengthValidation = GetValidation(nameof(ValidationDummy.MinLenghtString));
        var maxLengthValidation = GetValidation(nameof(ValidationDummy.MaxLengthString));

        // Assert
        Assert.That(minMaxStringLenghtStringValidation.Minimum, Is.EqualTo(ValidationDummy.MinimumLength));
        Assert.That(minMaxStringLenghtStringValidation.Maximum, Is.EqualTo(ValidationDummy.MaximumLength));
        Assert.That(minMaxLenghtStringValidation.Minimum, Is.EqualTo(ValidationDummy.MinimumLength));
        Assert.That(minMaxLenghtStringValidation.Maximum, Is.EqualTo(ValidationDummy.MaximumLength));
        Assert.That(minLengthValidation.Minimum, Is.EqualTo(ValidationDummy.MinimumLength));
        Assert.That(maxLengthValidation.Maximum, Is.EqualTo(ValidationDummy.MaximumLength));

    }

    [Test]
    public void EncodeReadonlyCollection()
    {
        // Arrange
        var dummy = new ListDummy();

        // Act
        var encoded = EntryConvert.EncodeObject(dummy);

        // Assert
        var readList = encoded.SubEntry(nameof(ListDummy.ReadOnly));
        Assert.That(readList.SubEntries.Count, Is.EqualTo(1));
        Assert.That(readList.Value.IsReadOnly);
        Assert.That(readList.Prototypes, Is.Empty);
    }

    [Test]
    public void ParametersShouldRespectRequiredAttribute()
    {
        // Arrange
        var myClass = typeof(ValidationDummy);
        var method = myClass.GetMethod(nameof(ValidationDummy.MethodWithRequiredAndOptionalParameters));

        // Act
        var entry = EntryConvert.EncodeMethod(method);

        var plainParameterValidation = entry.Parameters.SubEntry("plainParameter").Validation;
        var requiredParameterValidation = entry.Parameters.SubEntry("requiredParameter").Validation;
        var nullableValidation = entry.Parameters.SubEntry("nullableString").Validation;
        var defaultValueValidation = entry.Parameters.SubEntry("defaultValueString").Validation;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(plainParameterValidation.IsRequired, Is.True, "Plain parameter should be reuired");
            Assert.That(requiredParameterValidation.IsRequired, Is.True, "Required parameter should be required");
            Assert.That(nullableValidation.IsRequired, Is.False, "Nullable parameter should not be required");
            Assert.That(defaultValueValidation.IsRequired, Is.False, "Default value shuold not be required");
        });
    }

    private EntryValidation GetValidation(string identifier) =>
        _encodedValidationDummy.SubEntry(identifier).Validation;
}
