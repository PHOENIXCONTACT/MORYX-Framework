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
