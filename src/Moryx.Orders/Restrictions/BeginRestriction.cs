// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Restrictions;

/// <summary>
/// Restriction to limit possibility to begin an operation
/// </summary>
public class BeginRestriction : IOperationRestriction
{
    /// <summary>
    /// Creates a new instance of <see cref="BeginRestriction"/>
    /// </summary>
    /// <param name="canBegin">Indicates whether this restriction still allows to begin the operation</param>
    /// <param name="description">Provides a <see cref="RestrictionDescription"/> for the restriction</param>
    public BeginRestriction(bool canBegin, RestrictionDescription description)
    {
        CanBegin = canBegin;
        Description = description;
    }

    /// <summary>
    /// Creates a new instance of <see cref="BeginRestriction"/>
    /// </summary>
    /// <param name="canBegin">Indicates whether this restriction still allows to begin the operation</param>
    /// <param name="text">Provides a textual description for the restriction</param>
    /// <param name="severity">Sets the severity of the restriction</param>
    public BeginRestriction(bool canBegin, string text, RestrictionSeverity severity)
        : this(canBegin, new RestrictionDescription(text, severity))
    {
    }

    /// <summary>
    /// Creates a new <see cref="BeginRestriction"/>
    /// </summary>
    /// <param name="canBegin">Indicates whether this restriction still allows to begin the operation</param>
    /// <param name="canReduce">Indicates whether this restriction still allows to reduce the target amount of the operation</param>
    /// <param name="minimalTargetAmount">Provides the minimal target amount that can be set in the adjustment</param>
    /// <param name="description">Provides a <see cref="RestrictionDescription"/> for the restriction</param>
    public BeginRestriction(bool canBegin, bool canReduce, int minimalTargetAmount, RestrictionDescription description)
        : this(canBegin, description)
    {
        CanReduce = canReduce;
        MinimalTargetAmount = minimalTargetAmount;
    }

    /// <summary>
    /// Creates a new <see cref="BeginRestriction"/>
    /// </summary>
    /// <param name="canBegin">Indicates whether this restriction still allows to begin the operation</param>
    /// <param name="canReduce">Indicates whether this restriction still allows to reduce the target amount of the operation</param>
    /// <param name="minimalTargetAmount">Provides the minimal target amount that can be set in the adjustment</param>
    /// <param name="description">Provides a textual description for the restriction</param>
    /// <param name="severity">Sets the severity of the restriction</param>
    public BeginRestriction(bool canBegin, bool canReduce, int minimalTargetAmount, string description, RestrictionSeverity severity)
        : this(canBegin, canReduce, minimalTargetAmount, new RestrictionDescription(description, severity))
    {
    }

    /// <summary>
    /// Indicator if a begin is possible
    /// </summary>
    public bool CanBegin { get; }

    /// <summary>
    /// Indicator if an adjustement of the operation is possible
    /// </summary>
    public bool CanReduce { get; }

    /// <summary>
    /// Minimal target amount that can be set in the adjustement
    /// </summary>
    public int MinimalTargetAmount { get; }

    /// <summary>
    /// Description of the Result why the rule was not complied
    /// </summary>
    public RestrictionDescription Description { get; }
}