// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using NUnit.Framework;
using System.ComponentModel;
using System.Reflection;
using Moryx.Tools;

namespace Moryx.Tests.Extensions
{
    internal class TestConstants
    {
        public const string ClassDisplayName = "Some Display Name";
        public const string ClassDescription = "Some Description";
    }

    [ClassDisplay(Name = TestConstants.ClassDisplayName, Description = TestConstants.ClassDescription)]
    internal class ClassDisplayAttributeDummy
    {
    }

    [DisplayName(TestConstants.ClassDisplayName)]
    [System.ComponentModel.Description(TestConstants.ClassDescription)]
    internal class DotnetAttributesDummy
    {
    }

    [TestFixture]
    public class CustomAttributeProviderExtensionTests
    {
        [System.ComponentModel.Description("Tests if the GetDisplayName extension returns the correct display name")]
        [TestCase(typeof(ClassDisplayAttributeDummy))]
        [TestCase(typeof(DotnetAttributesDummy))]
        public void ClassDisplayAttribute(Type target)
        {
            // Act
            var displayName = target.GetDisplayName();

            // Assert
            Assert.AreEqual(TestConstants.ClassDisplayName, displayName);
        }

        [System.ComponentModel.Description("Tests if the GetDescription extension returns the correct description")]
        [TestCase(typeof(ClassDisplayAttributeDummy))]
        [TestCase(typeof(DotnetAttributesDummy))]
        public void ClassDescriptionAttrbute(Type target)
        {
            // Act
            var description = target.GetDescription();

            // Assert
            Assert.AreEqual(TestConstants.ClassDescription, description);
        }

        [Test]
        public void GetValidCustomAttribute()
        {
            // Arrange
            var provider = (ICustomAttributeProvider)typeof(DotnetAttributesDummy);

            // Act
            var attr = provider.GetCustomAttribute<DisplayNameAttribute>();

            // Assert
            Assert.NotNull(attr);
        }

        [Test]
        public void GetInvalidCustomAttribute()
        {
            // Arrange
            var provider = (ICustomAttributeProvider)typeof(DotnetAttributesDummy);

            // Act
            var attr = provider.GetCustomAttribute<AssemblyTitleAttribute>();

            // Assert
            Assert.IsNull(attr);
        }
    }
}
