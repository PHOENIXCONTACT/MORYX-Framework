// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Moryx.Tools;

namespace Moryx.Products.Management
{
    /// <summary>
    /// Base class for all internal accessor wrappers with type conversion
    /// </summary>
    internal abstract class ConversionAccessor<TColumn, TProperty> : IPropertyAccessor<object, TColumn>
    {
        protected IPropertyAccessor<object, TProperty> Target { get; }

        public string Name => Target.Name;

        public PropertyInfo Property => Target.Property;

        protected ConversionAccessor(PropertyInfo property)
        {
            Target = ReflectionTool.PropertyAccessor<object, TProperty>(property);
        }

        public abstract TColumn ReadProperty(object instance);

        public abstract void WriteProperty(object instance, TColumn value);
    }
}