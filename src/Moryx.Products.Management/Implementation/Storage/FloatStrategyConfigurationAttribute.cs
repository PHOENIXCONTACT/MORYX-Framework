// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    /// <inheritdoc/>
    public class FloatStrategyConfigurationAttribute : PropertyStrategyConfigurationAttribute
    {
        /// <inheritdoc/>
        public FloatStrategyConfigurationAttribute()
        {
            ColumnType = typeof(double);
            SupportedTypes =
            [
                typeof(float),
                typeof(double),
                typeof(decimal)
            ];
        }
    }
}