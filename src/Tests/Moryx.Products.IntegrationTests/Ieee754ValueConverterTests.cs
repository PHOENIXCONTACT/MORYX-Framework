// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Products.Management.Model;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests;

[TestFixture]
public class Ieee754ValueConverterTests
{
    private Ieee754ValueConverter _converter;

    [SetUp]
    public void SetUp()
    {
        _converter = new Ieee754ValueConverter();
    }

    [TestCase(0.0)]
    [TestCase(1.0)]
    [TestCase(-1.0)]
    [TestCase(42.5)]
    [TestCase(double.MinValue)]
    [TestCase(double.MaxValue)]
    [TestCase(double.Epsilon)]
    [Description("Regular double values are stored and restored without sentinel encoding.")]
    public void RoundTripsRegularValues(double value)
    {
        // Act
        var stored = _converter.ConvertToProvider(value);
        var restored = _converter.ConvertFromProvider(stored);

        // Assert
        Assert.That(stored, Is.EqualTo(value));
        Assert.That(restored, Is.EqualTo(value));
    }

    [Test(Description = "NaN is encoded as a sentinel for storage and decoded back to NaN after reading.")]
    public void RoundTripsNaN()
    {
        // Act
        var stored = (double)_converter.ConvertToProvider(double.NaN)!;
        var restored = (double)_converter.ConvertFromProvider(stored)!;

        // Assert
        Assert.That(double.IsNaN(stored), Is.False);
        Assert.That(double.IsNaN(restored), Is.True);
    }

    [TestCase(double.PositiveInfinity)]
    [TestCase(double.NegativeInfinity)]
    [Description("Infinity is encoded as a sentinel for storage and decoded back after reading.")]
    public void RoundTripsInfinity(double value)
    {
        // Act
        var stored = (double)_converter.ConvertToProvider(value)!;
        var restored = (double)_converter.ConvertFromProvider(stored)!;

        // Assert
        Assert.That(double.IsInfinity(stored), Is.False);
        Assert.That(restored, Is.EqualTo(value));
    }

    [Test(Description = "NaN, positive infinity, and negative infinity each map to a unique sentinel value so they can be distinguished after reading.")]
    public void SpecialValuesProduceDistinctSentinels()
    {
        // Act
        var nanStored = (double)_converter.ConvertToProvider(double.NaN)!;
        var posInfStored = (double)_converter.ConvertToProvider(double.PositiveInfinity)!;
        var negInfStored = (double)_converter.ConvertToProvider(double.NegativeInfinity)!;

        // Assert
        Assert.That(nanStored, Is.Not.EqualTo(posInfStored));
        Assert.That(nanStored, Is.Not.EqualTo(negInfStored));
        Assert.That(posInfStored, Is.Not.EqualTo(negInfStored));
    }
}
