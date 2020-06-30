// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Moryx
{
    /// <summary>
    /// Extension to the <see cref="DisplayAttribute"/> which supports also classes
    /// </summary>
    /// TODO: Remove with upgrade to .NET Core
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ClassDisplayAttribute : Attribute
    {
        private readonly DisplayAttribute _display = new DisplayAttribute();

        // Comments are copy and paste of the DisplayAttribute

        /// <summary>
        /// Gets or sets the ShortName attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetShortName"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// short name for display.
        /// <para>
        /// The <see cref="GetShortName"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// The short name is generally used as the grid column label for a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string ShortName
        {
            get { return _display.ShortName; }
            set { _display.ShortName = value; }
        }

        /// <summary>
        /// Gets or sets the Name attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetName"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// name for display.
        /// <para>
        /// The <see cref="GetName"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// The name is generally used as the field label for a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string Name
        {
            get { return _display.Name; }
            set { _display.Name = value; }
        }

        /// <summary>
        /// Gets or sets the Description attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetDescription"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// description for display.
        /// <para>
        /// The <see cref="GetDescription"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// Description is generally used as a tool tip or description a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string Description
        {
            get { return _display.Description; }
            set { _display.Description = value; }
        }

        /// <summary>
        /// Gets or sets the Prompt attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetPrompt"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// prompt for display.
        /// <para>
        /// The <see cref="GetPrompt"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// A prompt is generally used as a prompt or watermark for a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string Prompt
        {
            get { return _display.Prompt; }
            set { _display.Prompt = value; }
        }

        /// <summary>
        /// Gets or sets the GroupName attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetGroupName"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// group name for display.
        /// <para>
        /// The <see cref="GetGroupName"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// A group name is used for grouping fields into the UI.  A <c>null</c> or empty string is legal,
        /// and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string GroupName
        {
            get { return _display.GroupName; }
            set { _display.GroupName = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> that contains the resources for <see cref="Name"/> and <see cref="Description"/>.
        /// Using <see cref="ResourceType"/> along with these Key properties, allows the <see cref="GetName"/> and <see cref="GetDescription"/>
        /// methods to return localized values.
        /// </summary>
        public Type ResourceType
        {
            get { return _display.ResourceType; }
            set { _display.ResourceType = value; }
        }

        /// <summary>
        /// Default constructor for ClassDisplayAttribute.
        /// All associated string properties and methods will return <c>null</c>.
        /// </summary>
        public ClassDisplayAttribute()
        {
        }

        /// <summary>
        /// Gets the UI display string for ShortName.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="ShortName"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="ShortName"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="ShortName"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="ShortName"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// <para>
        /// If <see cref="ShortName"/> is <c>null</c>, the value from <see cref="GetName"/> will be returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="ShortName"/> property,
        /// but a public static property with a name matching the <see cref="ShortName"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetShortName() => _display.GetShortName();

        /// <summary>
        /// Gets the UI display string for Name.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="Name"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="Name"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="Name"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="Name"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// <para>
        /// Can return <c>null</c> and will not fall back onto other values, as it's more likely for the
        /// consumer to want to fall back onto the property name.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="Name"/> property,
        /// but a public static property with a name matching the <see cref="Name"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetName() => _display.GetName();

        /// <summary>
        /// Gets the UI display string for Description.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="Description"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="Description"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="Description"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="Description"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="Description"/> property,
        /// but a public static property with a name matching the <see cref="Description"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetDescription() => _display.GetDescription();

        /// <summary>
        /// Gets the UI display string for Prompt.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="Prompt"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="Prompt"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="Prompt"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="Prompt"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="Prompt"/> property,
        /// but a public static property with a name matching the <see cref="Prompt"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetPrompt() => _display.GetPrompt();

        /// <summary>
        /// Gets the UI display string for GroupName.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="GroupName"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="GroupName"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="GroupName"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="GroupName"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="GroupName"/> property,
        /// but a public static property with a name matching the <see cref="GroupName"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetGroupName() => _display.GetGroupName();
    }
}
