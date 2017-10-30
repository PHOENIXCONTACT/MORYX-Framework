﻿using Marvin.Bindings;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests.Bindings
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
            Assert.AreEqual("Thomas", foo.Branch.Name);
        }
    }
}