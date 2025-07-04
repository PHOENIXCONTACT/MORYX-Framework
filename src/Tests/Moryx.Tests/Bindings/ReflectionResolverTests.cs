// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using Moryx.Bindings;
using NUnit.Framework;

namespace Moryx.Tests.Bindings
{
    [TestFixture]
    public class ReflectionResolverTests
    {
        [TestCase(false, Description = "Checks if the correct property will be resolved.")]
        [TestCase(true, Description = "Checks if the correct property will be resolved on a derived type.")]
        public void SimpleResolve(bool createDerived)
        {
            const string str = "HelloWorld";

            // Arrange
            var obj = createDerived ? new SomeHiddenPropertyClass() : new SomeClass();
            obj.SimpleString = str;
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleString));

            // Act
            var result = reflectionResolver.Resolve(obj);

            // Assert
            Assert.That(result is string);
            Assert.That(result, Is.EqualTo(str));
        }

        [Test]
        public void SimpleAssign()
        {
            const string str = "HelloWorld";

            // Arrange
            var obj = new SomeHiddenPropertyClass();
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleString));

            // Act
            var result = reflectionResolver.Update(obj, str);

            // Assert
            Assert.That(result);
            Assert.That(obj.SimpleString, Is.EqualTo(str));
        }

        [Test]
        public void AssignStringToInt()
        {
            const string number = "5";

            // Arrange
            var obj = new SomeHiddenPropertyClass();
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleInt));

            // Act
            var result = reflectionResolver.Update(obj, number);

            // Assert
            Assert.That(result);
            Assert.That(obj.SimpleInt, Is.EqualTo(int.Parse(number)));
        }

        [Test]
        public void AssignDoubleToString()
        {
            const double value = 7.78;

            // Arrange
            var obj = new SomeHiddenPropertyClass();
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SimpleString));

            // Act
            var result = reflectionResolver.Update(obj, value);

            // Assert
            Assert.That(result);
            Assert.That(Convert.ToDouble(obj.SimpleString), Is.EqualTo(value));
        }

        [Test(Description = "Checks if the reflection resolver returns null by resolving an unknown property")]
        public void NullByUnknownProperty()
        {
            // Arrange
            var obj = new SomeClass();
            IBindingResolver reflectionResolver = new ReflectionResolver("SomeUnknownProperty");

            // Act
            var result = reflectionResolver.Resolve(obj);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test(Description = "The error was that the reflection resolver throws an " +
                            nameof(AmbiguousMatchException) + "if a property was hidden by the new keyword.")]
        public void HiddenPropertyByNewKeyword()
        {
            // Arrange
            IBindingResolver reflectionResolver = new ReflectionResolver(nameof(SomeClass.SomeObject));
            var obj = new SomeHiddenPropertyClass
            {
                SomeObject = new SomeImplementation()
            };

            // Act
            // Should resolve the value of SomeObject
            object result = null;
            Assert.DoesNotThrow(delegate
            {
                result = reflectionResolver.Resolve(obj);
            });

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
