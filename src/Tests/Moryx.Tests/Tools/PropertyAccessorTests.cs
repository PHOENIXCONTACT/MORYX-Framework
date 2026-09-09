// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Tests.Tools;

[TestFixture]
public class PropertyAccessorTests
{
    private static IPropertyAccessor<object, TValue> Accessor<TValue>(string propertyName)
        => ReflectionTool.PropertyAccessor<object, TValue>(typeof(TestTarget).GetProperty(propertyName));

    [Test(Description = "A nullable property with a value is converted to the target type.")]
    public void ReadNullablePropertyWithValue()
    {
        // Arrange
        var target = new TestTarget { NullableNumber = 5 };
        var accessor = Accessor<long>(nameof(TestTarget.NullableNumber));

        // Act
        var value = accessor.ReadProperty(target);

        // Assert
        Assert.That(value, Is.EqualTo(5L));
    }

    [Test(Description = "Reading an empty nullable property must not throw but return the default of the target type.")]
    public void ReadNullablePropertyWithoutValue()
    {
        // Arrange
        var target = new TestTarget { NullableNumber = null };
        var accessor = Accessor<long>(nameof(TestTarget.NullableNumber));

        // Act
        var value = accessor.ReadProperty(target);

        // Assert
        Assert.That(value, Is.EqualTo(0L), "A null value can only be represented by the default of the target type");
    }

    [Test(Description = "Reading a null reference property returns null instead of throwing.")]
    public void ReadNullReferenceProperty()
    {
        // Arrange
        var target = new TestTarget { Text = null };
        var accessor = Accessor<object>(nameof(TestTarget.Text));

        // Act
        var value = accessor.ReadProperty(target);

        // Assert
        Assert.That(value, Is.Null);
    }

    [Test(Description = "A value is converted to the nullable type of the property.")]
    public void WriteNullableProperty()
    {
        // Arrange
        var target = new TestTarget();
        var accessor = Accessor<long>(nameof(TestTarget.NullableNumber));

        // Act
        accessor.WriteProperty(target, 7L);

        // Assert
        Assert.That(target.NullableNumber, Is.EqualTo(7));
    }

    [Test(Description = "Writing null to a nullable property clears it instead of throwing.")]
    public void WriteNullToNullableProperty()
    {
        // Arrange
        var target = new TestTarget { NullableNumber = 7 };
        var accessor = Accessor<object>(nameof(TestTarget.NullableNumber));

        // Act
        accessor.WriteProperty(target, null);

        // Assert
        Assert.That(target.NullableNumber, Is.Null);
    }

    [Test(Description = "Writing null to a value type property must not throw but reset it to its default.")]
    public void WriteNullToValueTypeProperty()
    {
        // Arrange
        var target = new TestTarget { Number = 7 };
        var accessor = Accessor<object>(nameof(TestTarget.Number));

        // Act
        accessor.WriteProperty(target, null);

        // Assert
        Assert.That(target.Number, Is.EqualTo(0));
    }

    private sealed class TestTarget
    {
        public int Number { get; set; }

        public int? NullableNumber { get; set; }

        public string Text { get; set; }
    }
}
