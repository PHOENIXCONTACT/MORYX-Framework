// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Products.Management
{
    public class TextStrategyConfigurationAttribute : PropertyStrategyConfigurationAttribute
    {
        public TextStrategyConfigurationAttribute()
        {
            ColumnType = typeof(string);
            DerivedTypes = true;
            SupportedTypes = new[] {typeof(string), typeof(object), typeof(Guid)};
        }

        /// <inheritdoc />
        public override int TypeCompliance(Type targetType)
        {
            // Fallback for interfaces
            if (targetType.IsInterface)
                return BadCompliance - 1;

            return base.TypeCompliance(targetType);
        }
    }
}