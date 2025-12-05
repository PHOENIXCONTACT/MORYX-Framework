// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Bindings;
using NUnit.Framework;

namespace Moryx.Tests.Bindings
{
    [TestFixture]
    public class RegexResolverTest
    {
        [TestCase("{Branch.Name}", "Howard" , Description = "Resolve sub-property value using internal regex resolver")]
        [TestCase("{Branch1.Name}", "Stark", Description = "Resolve sub-property value using internal regex resolver")]
        [TestCase("{Branch.Value1}", "21", Description = "Resolve sub-property value using internal regex resolver")]
        [TestCase("{Name}", "Test", Description = "Resolve property value using internal regex resolver")]
        [TestCase("{Value1}", "42", Description = "Resolve property value using internal regex resolver")]
        public void ResolveValue(string parameterWithBindings, string expectedResult)
        {
            // Arrange
            var foo = new Foo
            {
                Branch = new Branch { Name = "Howard", Value1 = 21 },
                Branch1 = new Branch { Name = "Stark", Value1 = 99 },
                Name = "Test",
                Value1 = 42
            };

            var factory = TextBindingResolverFactory.Create(parameterWithBindings, new BindingResolverFactory());

            // Act
            var result = factory.Resolve(foo);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
