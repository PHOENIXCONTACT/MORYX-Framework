// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management
{
    /// <inheritdoc/>
    [AttributeUsage(AttributeTargets.Class)]
    public class IntegerStrategyConfigurationAttribute : PropertyStrategyConfigurationAttribute
    {
        /// <inheritdoc/>
        public IntegerStrategyConfigurationAttribute()
        {
            ColumnType = typeof(long);
            SupportedTypes =
            [
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(Enum),
                typeof(DateTime),
                typeof(bool)
            ];
        }

        public override int TypeCompliance(Type targetType)
        {
            if (targetType.IsEnum)
                return PerfectMatch;

            return base.TypeCompliance(targetType);
        }
    }
}