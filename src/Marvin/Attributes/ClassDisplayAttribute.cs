using System;
using System.ComponentModel.DataAnnotations;

namespace Marvin
{
    /// <summary>
    /// Extension to the <see cref="DisplayAttribute"/> which supports also classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ClassDisplayAttribute : Attribute
    {
        private readonly DisplayAttribute _display = new DisplayAttribute();

        // TODO: Remove with upgrade to .NET Core
        // Comments are copy and paste of the DisplayAttribute

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
        public string Description
        {
            get { return _display.Description; }
            set { _display.Description = value; }
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
        public string GetDescription() => _display.GetDescription();
    }
}
