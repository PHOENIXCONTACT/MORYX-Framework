// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Bindings;
using NUnit.Framework;

namespace Moryx.Tests.Bindings
{
    [TestFixture]
    public class DelegateResolverTest
    {
        [Test(Description = "Resolve value using delegate resolver")]
        public void ResolveValue()
        {
            // Arrange
            var foo = new Foo
            {
                Branch = new Branch { Name = "Howard" }
            };
            IBindingResolver resolver = new DelegateResolver(src => ((Foo)src).Branch.Name);

            // Act
            var result = resolver.Resolve(foo);

            // Assert
            Assert.AreEqual("Howard", result);
        }

        [Test(Description = "Update the propert value using a delegate resolver")]
        public void Updatevalue()
        {
            // Arrange
            var foo = new Foo
            {
                Branch = new Branch()
            };
            IBindingResolver resolver = new DelegateResolver(src => ((Foo)src).Branch.Name, (src, value) => ((Foo)src).Branch.Name = (string)value);

            // Act
            resolver.Update(foo, "Howard");

            // Assert
            Assert.AreEqual("Howard", foo.Branch.Name);
        }
    }
}
