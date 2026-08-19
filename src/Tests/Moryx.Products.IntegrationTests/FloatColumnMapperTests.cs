// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Products.Management;
using Moryx.Products.Management.Model;
using NUnit.Framework;

namespace Moryx.Products.IntegrationTests;

[TestFixture]
public class FloatColumnMapperTests
{
    private FloatColumnMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new FloatColumnMapper(typeof(TestObject));
        _mapper.Initialize(new PropertyMapperConfig
        {
            PropertyName = nameof(TestObject.Value),
            Column = nameof(IGenericColumns.Float1),
            PluginName = nameof(FloatColumnMapper)
        });
    }

    [TestCase(0.0)]
    [TestCase(1.0)]
    [TestCase(-1.0)]
    [TestCase(42.5)]
    [TestCase(double.MinValue)]
    [TestCase(double.MaxValue)]
    [TestCase(double.Epsilon)]
    [Description("Regular double values are stored in the column and restored exactly after a write-read cycle.")]
    public void RoundTripsRegularValues(double value)
    {
        // Arrange
        var source = new TestObject(value);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);
        var target = new TestObject();
        _mapper.ReadValue(columns, target);

        // Assert
        Assert.That(columns.Float1, Is.EqualTo(value));
        Assert.That(target.Value, Is.EqualTo(value));
    }

    [Test(Description = "NaN is encoded as a sentinel for storage and decoded back to NaN after reading.")]
    public void RoundTripsNaN()
    {
        // Arrange
        var source = new TestObject(double.NaN);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);
        var target = new TestObject();
        _mapper.ReadValue(columns, target);

        // Assert
        Assert.That(double.IsNaN(columns.Float1), Is.False);
        Assert.That(double.IsNaN(target.Value), Is.True);
    }

    [Test(Description = "Positive infinity is encoded as a sentinel for storage and decoded back to positive infinity after reading.")]
    public void RoundTripsPositiveInfinity()
    {
        // Arrange
        var source = new TestObject(double.PositiveInfinity);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);
        var target = new TestObject();
        _mapper.ReadValue(columns, target);

        // Assert
        Assert.That(double.IsInfinity(columns.Float1), Is.False);
        Assert.That(target.Value, Is.EqualTo(double.PositiveInfinity));
    }

    [Test(Description = "Negative infinity is encoded as a sentinel for storage and decoded back to negative infinity after reading.")]
    public void RoundTripsNegativeInfinity()
    {
        // Arrange
        var source = new TestObject(double.NegativeInfinity);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);
        var target = new TestObject();
        _mapper.ReadValue(columns, target);

        // Assert
        Assert.That(double.IsInfinity(columns.Float1), Is.False);
        Assert.That(target.Value, Is.EqualTo(double.NegativeInfinity));
    }

    [Test(Description = "Writing NaN stores a sentinel instead of raw NaN, which would be rejected by some database providers.")]
    public void DoesNotStoreRawNaNInColumn()
    {
        // Arrange
        var source = new TestObject(double.NaN);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);

        // Assert — the column must not contain raw NaN (SQLite would reject it)
        Assert.That(double.IsNaN(columns.Float1), Is.False);
    }

    [Test(Description = "Writing positive infinity stores a sentinel instead of raw infinity, which would be rejected by some database providers.")]
    public void DoesNotStoreRawPositiveInfinityInColumn()
    {
        // Arrange
        var source = new TestObject(double.PositiveInfinity);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);

        // Assert
        Assert.That(double.IsInfinity(columns.Float1), Is.False);
    }

    [Test(Description = "Writing negative infinity stores a sentinel instead of raw infinity, which would be rejected by some database providers.")]
    public void DoesNotStoreRawNegativeInfinityInColumn()
    {
        // Arrange
        var source = new TestObject(double.NegativeInfinity);
        var columns = CreateColumns();

        // Act
        _mapper.WriteValue(source, columns);

        // Assert
        Assert.That(double.IsInfinity(columns.Float1), Is.False);
    }

    [Test(Description = "NaN, positive infinity, and negative infinity each map to a unique sentinel value so they can be distinguished after reading.")]
    public void SpecialValuesProduceDistinctSentinels()
    {
        // Arrange
        var nanColumns = CreateColumns();
        var posInfColumns = CreateColumns();
        var negInfColumns = CreateColumns();

        // Act
        _mapper.WriteValue(new TestObject(double.NaN), nanColumns);
        _mapper.WriteValue(new TestObject(double.PositiveInfinity), posInfColumns);
        _mapper.WriteValue(new TestObject(double.NegativeInfinity), negInfColumns);

        // Assert — all three sentinels must be distinct
        Assert.That(nanColumns.Float1, Is.Not.EqualTo(posInfColumns.Float1));
        Assert.That(nanColumns.Float1, Is.Not.EqualTo(negInfColumns.Float1));
        Assert.That(posInfColumns.Float1, Is.Not.EqualTo(negInfColumns.Float1));
    }

    [Test(Description = "Reading a raw NaN from a column passes it through as NaN since it does not match any sentinel.")]
    public void ReturnsNaNForRawNaNFromPostgres()
    {
        // Simulate a PostgreSQL database that already stored raw NaN
        var columns = CreateColumns();
        columns.Float1 = double.NaN;
        var target = new TestObject();

        // Act
        _mapper.ReadValue(columns, target);

        // Assert — raw NaN does not match any sentinel
        Assert.That(double.IsNaN(columns.Float1), Is.True);
        Assert.That(double.IsNaN(target.Value), Is.True);
    }

    [Test(Description = "HasChanged detects a difference between a raw NaN in the column and the sentinel-encoded NaN")]
    public void DetectsChangeFromRawNaNToSentinelNaN()
    {
        // Simulate: PostgreSQL has raw NaN in the column, object has NaN
        // which gets encoded as sentine
        var columns = CreateColumns();
        columns.Float1 = double.NaN;
        var source = new TestObject(double.NaN);

        // Act
        var changed = _mapper.HasChanged(columns, source);

        // Assert
        Assert.That(double.IsNaN(columns.Float1), Is.True);
        Assert.That(changed, Is.True);
    }

    [Test(Description = "After writing a sentinel value, HasChanged returns false for the same source value.")]
    public void SentinelRoundTripShowsNoChange()
    {
        // Write sentinel, then check HasChanged against the same value
        var source = new TestObject(double.NaN);
        var columns = CreateColumns();
        _mapper.WriteValue(source, columns);

        // Act
        var changed = _mapper.HasChanged(columns, source);

        // Assert
        Assert.That(double.IsNaN(columns.Float1), Is.False);
        Assert.That(changed, Is.False);
    }

    private static IGenericColumns CreateColumns()
    {
        var mock = new Mock<IGenericColumns>();
        mock.SetupAllProperties();
        return mock.Object;
    }

    private class TestObject(double value = 0)
    {
        public double Value { get; set; } = value;
    }
}
