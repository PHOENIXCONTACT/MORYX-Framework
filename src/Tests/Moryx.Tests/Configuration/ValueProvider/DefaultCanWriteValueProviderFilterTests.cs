// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Configuration;
using NUnit.Framework;

namespace Moryx.Tests.Configuration.ValueProvider
{
    [TestFixture]
    public class DefaultCanWriteValueProviderFilterTests
    {
        [Test(Description = "Test detects private setter right")]
        public void DetectPrivateSetter()
        {
            // Arrange
            var filter = new DefaultCanWriteValueProviderFilter();
            var classType = typeof(PrivateSetterClass);
            var privateSetterProperty = classType.GetProperty(nameof(PrivateSetterClass.PrivateSetterBool));
            var noSetterProperty = classType.GetProperty(nameof(PrivateSetterClass.NoSetterBool));

            // Act
            var canWritePrivateSetter = filter.CheckProperty(privateSetterProperty);
            var canWriteNoSetter = filter.CheckProperty(noSetterProperty);

            // Assert
            Assert.That(canWritePrivateSetter, Is.False, "Private setter should be treated as not writable");
            Assert.That(canWriteNoSetter, Is.False, "No setter should be treated as not writable");
        }

        public class PrivateSetterClass
        {
            public bool PrivateSetterBool { get; private set; }

            public bool NoSetterBool => false;
        }
    }
}
