// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Products.Management;

/// <summary>
/// Additional attribute for strategy implementation to enable auto configuration
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StrategyConfigurationAttribute : Attribute
{
    /// <summary>
    /// Constant that indicates a perfect match between strategy and target type
    /// </summary>
    public const int PerfectMatch = 0;

    /// <summary>
    /// Constant that indicates bad compliance between the strategy and target type
    /// </summary>
    public const int BadCompliance = int.MaxValue;

    /// <summary>
    /// Type supported by this strategy. The auto configuration will try to find the closest match
    /// </summary>
    public Type[] SupportedTypes { get; protected set; }

    /// <summary>
    /// Flag if the strategy also supports derived types
    /// </summary>
    public bool DerivedTypes { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="supportedTypes"></param>
    public StrategyConfigurationAttribute(params Type[] supportedTypes)
    {
        SupportedTypes = supportedTypes;
    }

    public virtual int TypeCompliance(Type targetType)
    {
        // Determine compliance as inheritance distance
        var compliance = PerfectMatch;
        var current = targetType;
        do
        {
            if (SupportedTypes.Contains(current))
                return compliance;

            current = current.BaseType;
            compliance++;
        } while (DerivedTypes & current != null);

        // Use full distance if it is an interface match
        if (targetType.GetInterfaces().Any(SupportedTypes.Contains))
            return compliance;

        // Otherwise it is no match
        return BadCompliance;
    }
}