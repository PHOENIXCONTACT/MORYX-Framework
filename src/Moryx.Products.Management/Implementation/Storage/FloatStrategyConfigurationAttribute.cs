// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    public class FloatStrategyConfigurationAttribute : PropertyStrategyConfigurationAttribute
    {
        public FloatStrategyConfigurationAttribute()
        {
            ColumnType = typeof(double);
            SupportedTypes = new[]
            {
                typeof(float),
                typeof(double),
                typeof(decimal)
            };
        }
    }
}