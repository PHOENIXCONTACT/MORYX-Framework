// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Moryx.Bindings;
using Moryx.ProcessData.Bindings;
using Moryx.ProcessData.Configuration;
using Moq;
using NUnit.Framework;

namespace Moryx.ProcessData.Tests
{
    [TestFixture]
    public class MeasurementBindingProcessorTests
    {
        private const string BindingPropA = "Object.PropA";
        private const string BindingPropB = "Object.PropB";

        [Test(Description = "Should write configured measurement bindings to the given measurement")]
        public void ResolvesConfiguredBindings()
        {
            const string valuePropA = "ValuePropA";
            const string valuePropB = "ValuePropB";

            // Arrange
            var resolverMockA = new Mock<IBindingResolver>();
            resolverMockA.Setup(r => r.Resolve(It.IsAny<object>())).Returns(valuePropA);

            var resolverMockB = new Mock<IBindingResolver>();
            resolverMockB.Setup(r => r.Resolve(It.IsAny<object>())).Returns(valuePropB);

            var bindingResolverFactoryMock = new Mock<IBindingResolverFactory>();
            bindingResolverFactoryMock.Setup(b => b.Create(BindingPropA))
                .Returns(() => resolverMockA.Object);
            bindingResolverFactoryMock.Setup(b => b.Create(BindingPropB))
                .Returns(() => resolverMockB.Object);

            var bindingConfiguration = CreateBindingConfiguration();
            var processor = new MeasurementBindingProcessor(bindingResolverFactoryMock.Object, bindingConfiguration);

            var someMeasurement = new Measurement(string.Empty);

            // Act
            processor.Apply(someMeasurement, new object());

            // Assert
            Assert.That(someMeasurement.Fields.Count, Is.EqualTo(1));
            var field = someMeasurement.Fields[0];
            Assert.That("propA", Is.EqualTo(field.Name));
            Assert.That(valuePropA, Is.EqualTo(field.Value));

            Assert.That(someMeasurement.Tags.Count, Is.EqualTo(1));
            var tag = someMeasurement.Tags[0];
            Assert.That("propB", Is.EqualTo(tag.Name));
            Assert.That(valuePropB, Is.EqualTo(tag.Value));
        }

        private static IEnumerable<MeasurementBinding> CreateBindingConfiguration()
        {
            return new[]
            {
                new MeasurementBinding
                {
                    Name = "propA",
                    Binding = BindingPropA,
                    ValueTarget = ValueTarget.Field
                },
                new MeasurementBinding
                {
                    Name = "propB",
                    Binding = BindingPropB,
                    ValueTarget = ValueTarget.Tag
                }
            };
        }
    }
}
