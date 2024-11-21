// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Products.Management
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IntegerStrategyConfigurationAttribute : PropertyStrategyConfigurationAttribute
    {
        public IntegerStrategyConfigurationAttribute()
        {
            ColumnType = typeof(long);
            SupportedTypes = new[]
            {
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(Enum),
                typeof(DateTime),
                typeof(bool),
            };
        }

        public override int TypeCompliance(Type targetType)
        {
            if (targetType.IsEnum)
                return PerfectMatch;

            return base.TypeCompliance(targetType);
        }
    }
}