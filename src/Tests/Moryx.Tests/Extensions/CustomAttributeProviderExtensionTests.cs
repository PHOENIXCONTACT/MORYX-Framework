// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using NUnit.Framework;
using System.ComponentModel;
using System.Reflection;
using Moryx.Tools;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Tests.Extensions
{
    internal class TestConstants
    {
        public const string ClassDisplayName = "Some Display Name";
        public const string ClassDescription = "Some Description";
    }

    [Display(Name = TestConstants.ClassDisplayName, Description = TestConstants.ClassDescription)]
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
        public void GetDisplayNameFromAttribute(Type target)
        {
            // Act
            var displayName = target.GetDisplayName();

            // Assert
            Assert.That(displayName, Is.EqualTo(TestConstants.ClassDisplayName));
        }

        [System.ComponentModel.Description("Tests if the GetDescription extension returns the correct description")]
        [TestCase(typeof(ClassDisplayAttributeDummy))]
        [TestCase(typeof(DotnetAttributesDummy))]
        public void GetDescriptionFromAttribute(Type target)
        {
            // Act
            var description = target.GetDescription();

            // Assert
            Assert.That(description, Is.EqualTo(TestConstants.ClassDescription));
        }

        [Test]
        public void GetValidCustomAttribute()
        {
            // Arrange
            var provider = (ICustomAttributeProvider)typeof(DotnetAttributesDummy);

            // Act
            var attr = provider.GetCustomAttribute<DisplayNameAttribute>();

            // Assert
            Assert.That(attr, Is.Not.Null);
        }

        [Test]
        public void GetInvalidCustomAttribute()
        {
            // Arrange
            var provider = (ICustomAttributeProvider)typeof(DotnetAttributesDummy);

            // Act
            var attr = provider.GetCustomAttribute<AssemblyTitleAttribute>();

            // Assert
            Assert.That(attr, Is.Null);
        }
    }
}
