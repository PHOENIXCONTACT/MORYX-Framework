// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management;

public class TextStrategyConfigurationAttribute : PropertyStrategyConfigurationAttribute
{
    public TextStrategyConfigurationAttribute()
    {
        ColumnType = typeof(string);
        DerivedTypes = true;
        SupportedTypes = [typeof(string), typeof(object), typeof(Guid)];
    }

    /// <inheritdoc />
    public override int TypeCompliance(Type targetType)
    {
        // Fallback for interfaces
        if (targetType.IsInterface)
            return BadCompliance - 1;

        // Non-primitive structs like Vector3 are JSON-serialized into a text column
        // Exclude types explicitly listed in SupportedTypes (e.g. Guid)
        if (targetType.IsValueType && !targetType.IsPrimitive && !targetType.IsEnum
            && !SupportedTypes.Contains(targetType))
            return PerfectMatch + 1;

        return base.TypeCompliance(targetType);
    }
}