// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests;

[TestFixture]
public class ValidationTests
{
    private List<Entry> _validationDummySubEntries;

    [SetUp]
    public void Setup()
    {
        var encoded = EntryConvert.EncodeClass(typeof(ValidationDummy));

        // Assert
        Assert.That(encoded, Is.Not.Null);
        Assert.That(encoded.SubEntries.Count, Is.EqualTo(9));
        _validationDummySubEntries = encoded.SubEntries;
    }

    [Test]
    public void ValidateBase64Attribute()
    {
        var base64StrEntryValidation = GetValidation(nameof(ValidationDummy.Base64String));
        Assert.That(base64StrEntryValidation.IsBase64String, Is.EqualTo(true));
    }

    [Test]
    public void ValidateDateTypeAttribute()
    {
        var emailValidation = GetValidation(nameof(ValidationDummy.EmailDataType));
        Assert.That(emailValidation.DataType, Is.EqualTo(DataType.EmailAddress));
    }

    [Test]
    public void ValidateRequiredAttribute()
    {
        var requiredBoolValidation = GetValidation(nameof(ValidationDummy.RequiredBool));
        Assert.That(requiredBoolValidation.IsRequired, Is.EqualTo(true));
    }

    [Test]
    public void ValidateRegexAttribute()
    {
        var regexValidation = GetValidation(nameof(ValidationDummy.RegexString));
        Assert.That(regexValidation.Regex, Is.EqualTo(ValidationDummy.Regex));
    }

    [Test]
    public void ValidateLenghtAttribute()
    {
        var minMaxStringLenghtStringValidation = GetValidation(nameof(ValidationDummy.MinMaxStringLenghtString));
        Assert.That(minMaxStringLenghtStringValidation.Minimum, Is.EqualTo(ValidationDummy.MinimumLength));
        Assert.That(minMaxStringLenghtStringValidation.Maximum, Is.EqualTo(ValidationDummy.MaximumLength));

        var minMaxLenghtStringValidation = GetValidation(nameof(ValidationDummy.MinMaxLenghtString));
        Assert.That(minMaxLenghtStringValidation.Minimum, Is.EqualTo(ValidationDummy.MinimumLength));
        Assert.That(minMaxLenghtStringValidation.Maximum, Is.EqualTo(ValidationDummy.MaximumLength));

        var minLengthValidation = GetValidation(nameof(ValidationDummy.MinLenghtString));
        Assert.That(minLengthValidation.Minimum, Is.EqualTo(ValidationDummy.MinimumLength));

        var maxLengthValidation = GetValidation(nameof(ValidationDummy.MaxLengthString));
        Assert.That(maxLengthValidation.Maximum, Is.EqualTo(ValidationDummy.MaximumLength));
    }

    private EntryValidation GetValidation(string identifier)
    {
        var validation = _validationDummySubEntries.Single(e => e.Identifier == identifier).Validation;
        return validation;
    }
}
