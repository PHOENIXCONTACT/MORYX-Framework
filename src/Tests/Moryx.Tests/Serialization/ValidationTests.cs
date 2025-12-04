// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class ValidationTests
{
    private List<Entry> _validationDummySubEntries;

    [SetUp]
    public void Setup()
    {
        // Act
        var encoded = EntryConvert.EncodeClass(typeof(ValidationDummy));

        // Assert
        Assert.That(encoded, Is.Not.Null);
        Assert.That(encoded.SubEntries.Count, Is.EqualTo(8));
        _validationDummySubEntries = encoded.SubEntries;
    }

    [Test(Description = "Base64StringAttribute must be used to set EntryValueType in EntryEntryValue")]
    public void ValidateBase64StringAttribute()
    {
        // Arrange / Act
        var entry = _validationDummySubEntries.Single(e => e.Identifier == nameof(ValidationDummy.Base64String));

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

    private EntryValidation GetValidation(string identifier)
    {
        var validation = _validationDummySubEntries.Single(e => e.Identifier == identifier).Validation;
        return validation;
    }
}
