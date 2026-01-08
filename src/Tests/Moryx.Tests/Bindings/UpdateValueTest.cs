// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Bindings;
using NUnit.Framework;

namespace Moryx.Tests.Bindings
{
    [TestFixture]
    public class UpdateValueTest
    {
        [Test, Description("Set name on the Branch class")]
        public void SetName()
        {
            // Arrange
            var foo = new Foo
            {
                Branch = new Branch()
            };
            var resolver = new BindingResolverFactory().Create($"{nameof(Foo.Branch)}.{nameof(Branch.Name)}");

            // Act
            resolver.Update(foo, "Thomas");

            // Assert
            Assert.That(foo.Branch.Name, Is.EqualTo("Thomas"));
        }
    }
}
