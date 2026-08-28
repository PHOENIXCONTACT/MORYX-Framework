// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Moryx.Tools;
using NUnit.Framework;

namespace Moryx.Tests.Extensions;

file class TestConstants
{
    public const string EnumDisplayName = "Some Display Name";
    public const string EnumDescription = "Some Description";
}

file enum DisplayAttributeEnum
{
    [Display(Name = TestConstants.EnumDisplayName, Description = TestConstants.EnumDescription)]
    Decorated,

    Undecorated
}

file enum DescriptionAttributeEnum
{
    [System.ComponentModel.Description(TestConstants.EnumDescription)]
    Decorated,

    Undecorated
}

[Flags]
file enum FlagsDummyEnum
{
    [Display(Name = "None")]
    None = 0,

    [Display(Name = "Alpha")]
    Alpha = 1,

    [Display(Name = "Beta")]
    Beta = 2,

    [Display(Name = "Gamma")]
    Gamma = 4
}

[TestFixture]
public class EnumExtensionTests
{
    [Test(Description = "Returns the display name from the Display attribute")]
    public void GetDisplayNameFromAttribute()
    {
        // Arrange
        var value = DisplayAttributeEnum.Decorated;

        // Act
        var displayName = value.GetDisplayName();

        // Assert
        Assert.That(displayName, Is.EqualTo(TestConstants.EnumDisplayName));
    }

    [Test(Description = "Falls back to the field name when no display attribute is present")]
    public void GetDisplayNameFallsBackToFieldName()
    {
        // Arrange
        var value = DisplayAttributeEnum.Undecorated;

        // Act
        var displayName = value.GetDisplayName();

        // Assert
        Assert.That(displayName, Is.EqualTo(nameof(DisplayAttributeEnum.Undecorated)));
    }

    [Description("Returns the description from Display or Description attribute")]
    [TestCase(DisplayAttributeEnum.Decorated)]
    [TestCase(DescriptionAttributeEnum.Decorated)]
    public void GetDescriptionFromAttribute(Enum value)
    {
        // Act
        var description = value.GetDescription();

        // Assert
        Assert.That(description, Is.EqualTo(TestConstants.EnumDescription));
    }

    [Test(Description = "Returns null when no description attribute is present")]
    public void GetDescriptionReturnsNullWhenMissing()
    {
        // Arrange
        var value = DisplayAttributeEnum.Undecorated;

        // Act
        var description = value.GetDescription();

        // Assert
        Assert.That(description, Is.Null);
    }

    [Test]
    public void GetValidCustomAttribute()
    {
        // Arrange
        var value = DescriptionAttributeEnum.Decorated;

        // Act
        var attr = value.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();

        // Assert
        Assert.That(attr, Is.Not.Null);
        Assert.That(attr.Description, Is.EqualTo(TestConstants.EnumDescription));
    }

    [Test]
    public void GetInvalidCustomAttribute()
    {
        // Arrange
        var value = DescriptionAttributeEnum.Decorated;

        // Act
        var attr = value.GetCustomAttribute<AssemblyTitleAttribute>();

        // Assert
        Assert.That(attr, Is.Null);
    }

    [Test(Description = "Returns null when requesting a custom attribute on an undecorated enum value")]
    public void GetCustomAttributeReturnsNullOnUndecoratedValue()
    {
        // Arrange
        var value = DisplayAttributeEnum.Undecorated;

        // Act
        var attr = value.GetCustomAttribute<DisplayAttribute>();

        // Assert
        Assert.That(attr, Is.Null);
    }

    [Test(Description = "Retrieves the display name for a single flag value in a Flags enum")]
    public void GetDisplayNameForSingleFlag()
    {
        // Arrange
        var value = FlagsDummyEnum.Alpha;

        // Act
        var displayName = value.GetDisplayName();

        // Assert
        Assert.That(displayName, Is.EqualTo("Alpha"));
    }

    [Test(Description = "Retrieves the display name of the zero-valued flag when the enum value itself is zero")]
    public void GetDisplayNameForZeroFlag()
    {
        // Arrange
        var value = FlagsDummyEnum.None;

        // Act
        var displayName = value.GetDisplayName();

        // Assert
        Assert.That(displayName, Is.EqualTo("None"));
    }
}