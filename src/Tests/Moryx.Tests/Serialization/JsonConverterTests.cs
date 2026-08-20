// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Numerics;
using Moryx.Serialization;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Moryx.Tests.Serialization;

[TestFixture]
public class JsonConverterTests
{
    [Test(Description = "Vector2 values are preserved.")]
    public void Vector2PreservesValues()
    {
        // Arrange
        var original = new Vector2(1.5f, 2.5f);

        // Act
        var json = JsonConvert.SerializeObject(original, new Vector2Converter());
        var restored = JsonConvert.DeserializeObject<Vector2>(json, new Vector2Converter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "Vector3 values are preserved.")]
    public void Vector3PreservesValues()
    {
        // Arrange
        var original = new Vector3(1.5f, 2.5f, 3.5f);

        // Act
        var json = JsonConvert.SerializeObject(original, new Vector3Converter());
        var restored = JsonConvert.DeserializeObject<Vector3>(json, new Vector3Converter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "Quaternion values are preserved.")]
    public void QuaternionPreservesValues()
    {
        // Arrange
        var original = new Quaternion(0.1f, 0.2f, 0.3f, 0.9f);

        // Act
        var json = JsonConvert.SerializeObject(original, new QuaternionConverter());
        var restored = JsonConvert.DeserializeObject<Quaternion>(json, new QuaternionConverter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "DateOnly values are preserved.")]
    public void DateOnlyPreservesValue()
    {
        // Arrange
        var original = new DateOnly(2026, 8, 18);

        // Act
        var json = JsonConvert.SerializeObject(original, new DateOnlyConverter());
        var restored = JsonConvert.DeserializeObject<DateOnly>(json, new DateOnlyConverter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "TimeOnly values are preserved.")]
    public void TimeOnlyPreservesValue()
    {
        // Arrange
        var original = new TimeOnly(14, 30, 45);

        // Act
        var json = JsonConvert.SerializeObject(original, new TimeOnlyConverter());
        var restored = JsonConvert.DeserializeObject<TimeOnly>(json, new TimeOnlyConverter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "Vector4 values are preserved.")]
    public void Vector4PreservesValues()
    {
        // Arrange
        var original = new Vector4(1.5f, 2.5f, 3.5f, 4.5f);

        // Act
        var json = JsonConvert.SerializeObject(original, new Vector4Converter());
        var restored = JsonConvert.DeserializeObject<Vector4>(json, new Vector4Converter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "Plane values are preserved.")]
    public void PlanePreservesValues()
    {
        // Arrange
        var original = new Plane(new Vector3(0f, 1f, 0f), 5.5f);

        // Act
        var json = JsonConvert.SerializeObject(original, new PlaneConverter(), new Vector3Converter());
        var restored = JsonConvert.DeserializeObject<Plane>(json, new PlaneConverter(), new Vector3Converter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "Vector3.Zero is preserved and not lost as a default value.")]
    public void Vector3ZeroPreservesDefault()
    {
        // Arrange
        var original = Vector3.Zero;

        // Act
        var json = JsonConvert.SerializeObject(original, new Vector3Converter());
        var restored = JsonConvert.DeserializeObject<Vector3>(json, new Vector3Converter());

        // Assert
        Assert.That(restored, Is.EqualTo(original));
    }

    [Test(Description = "DateOnly serializes to ISO 8601 date format.")]
    public void DateOnlySerializesAsDateString()
    {
        // Arrange
        var original = new DateOnly(2026, 8, 18);

        // Act
        var json = JsonConvert.SerializeObject(original, new DateOnlyConverter());

        // Assert
        Assert.That(json, Is.EqualTo("\"2026-08-18\""));
    }

    [Test(Description = "TimeOnly serializes to ISO 8601 round-trip time format.")]
    public void TimeOnlySerializesAsTimeString()
    {
        // Arrange
        var original = new TimeOnly(14, 30, 45);

        // Act
        var json = JsonConvert.SerializeObject(original, new TimeOnlyConverter());

        // Assert
        Assert.That(json, Is.EqualTo("\"14:30:45.0000000\""));
    }
}
