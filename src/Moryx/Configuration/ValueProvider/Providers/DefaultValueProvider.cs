// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.ComponentModel;
using System.Reflection;

namespace Moryx.Configuration
{

    /// <summary>
    /// Old DefaultValueProvider for compatibility reasons.
    /// It is equivalent to using the <see cref="DefaultValueAttributeProvider" /> and the <see cref="ActivatorValueProvider" /> in series
    /// </summary>
    [Obsolete]
    public sealed class DefaultValueProvider : IValueProvider
    {

        private DefaultValueAttributeProvider defaultValueAttributeProvider = new DefaultValueAttributeProvider();
        private ActivatorValueProvider activatorValueProvider = new ActivatorValueProvider();
        
        /// <inheritdoc />
        public ValueProviderResult Handle(object parent, PropertyInfo property)
        {
            if (activatorValueProvider.Handle(parent, property) == ValueProviderResult.Skipped)
                return defaultValueAttributeProvider.Handle(parent, property);
            return ValueProviderResult.Skipped;
        }

    }
}
