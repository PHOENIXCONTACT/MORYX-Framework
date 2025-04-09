// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for all internal accessor wrappers with type conversion
    /// </summary>
    internal class ConversionAccessor<TColumn, TProperty> : IPropertyAccessor<object, TColumn>
    {
        protected IPropertyAccessor<object, TProperty> Target { get; }

        public string Name => Target.Name;

        public PropertyInfo Property => Target.Property;

        public Func<TProperty, TColumn> ToColumn { get; }

        public Func<TColumn, TProperty> ToProperty { get; }

        public ConversionAccessor(PropertyInfo property, Func<TProperty, TColumn> toColumn, Func<TColumn, TProperty> toProperty)
        {
            ToColumn = toColumn;
            ToProperty = toProperty;
            Target = ReflectionTool.PropertyAccessor<object, TProperty>(property);
        }

        public TColumn ReadProperty(object instance)
        {
            var value = Target.ReadProperty(instance);
            return ToColumn(value);
        }

        public void WriteProperty(object instance, TColumn value)
        {
            var converted = ToProperty(value);
            Target.WriteProperty(instance, converted);
        }
    }
}