// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Serialization;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

/// <summary>
/// Contains all tests concerned with the type transformation happening within the <see cref="EntryConvert"/>.
/// </summary>
[TestFixture]
public class TransformTypeTests
{
    [Test(Description = "Decimal maps to EntryValueType.Double.")]
    public void DecimalMapsToDoubleValueType()
    {
        // Act
        var valueType = EntryConvert.TransformType(typeof(decimal));

        // Assert
        Assert.That(valueType, Is.EqualTo(EntryValueType.Double));
    }

    [Test(Description = "DateTime maps to EntryValueType.DateTime.")]
    public void DateTimeMapsToDateTimeValueType()
    {
        // Act
        var valueType = EntryConvert.TransformType(typeof(DateTime));

        // Assert
        Assert.That(valueType, Is.EqualTo(EntryValueType.DateTime));
    }

    [Test(Description = "DateTime values are converted to UTC and preserved on round-trip.")]
    public void DateTimeValuesOnRoundTripPreservedAsUtc()
    {
        // Arrange
        var original = new ClassDummy
        {
            ModifiedAt = new DateTime(2026, 8, 18, 14, 30, 0, DateTimeKind.Utc)
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new ClassDummy();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        Assert.That(restored.ModifiedAt, Is.EqualTo(original.ModifiedAt));
        Assert.That(restored.ModifiedAt.Kind, Is.EqualTo(DateTimeKind.Utc));
    }

    [Test(Description = "DateTimeOffset maps to EntryValueType.DateTime, same as DateTime.")]
    public void DateTimeOffsetMapsToDateTimeValueType()
    {
        // Act
        var valueType = EntryConvert.TransformType(typeof(DateTimeOffset));

        // Assert
        Assert.That(valueType, Is.EqualTo(EntryValueType.DateTime));
    }

    [Test(Description = "DateTimeOffset UTC instant is preserved on round-trip, but the original offset is lost.")]
    public void DateTimeOffsetValuesOnRoundTripPreserved()
    {
        // Arrange
        var original = new ClassDummy
        {
            CreatedAt = new DateTimeOffset(2026, 8, 18, 14, 30, 0, TimeSpan.FromHours(2))
        };

        // Act
        var entry = EntryConvert.EncodeObject(original);
        var restored = new ClassDummy();
        EntryConvert.UpdateInstance(restored, entry);

        // Assert
        // Offset is lost, but the UTC instant is preserved
        Assert.That(restored.CreatedAt.UtcDateTime, Is.EqualTo(original.CreatedAt.UtcDateTime));
    }
}
