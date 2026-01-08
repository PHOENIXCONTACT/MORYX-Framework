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

        return base.TypeCompliance(targetType);
    }
}