// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization;

/// <summary>
/// Base attribute for all attributes that support multiple values
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public abstract class PossibleValuesAttribute : Attribute
{
    /// <summary>
    /// A flag indicating that the attribute requires a non-null <see cref="PossibleValuesContext"/>
    /// </summary>
    public bool RequiresPossibleValuesContext { get; set; }

    /// <summary>
    /// Flag if this member implements its own string to value conversion
    /// </summary>
    public abstract bool OverridesConversion { get; }

    /// <summary>
    /// Flag if new values shall be updated from the old value
    /// </summary>
    public abstract bool UpdateFromPredecessor { get; }

    // TODO: Remove both GetValues in next major MORYX release and make GetPossibleValues abstract
    /// <summary>
    /// Extract possible values from local or global DI registration
    /// </summary>
    public abstract IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider);

    /// <summary>
    /// Extract possible values from the given <paramref name="context"/>
    /// </summary>
    /// <param name="context">context of the possible value</param>
    public virtual IEnumerable<string> GetValues(PossibleValuesContext context = null)
    {
        yield break;
    }

    /// <summary>
    /// Extract possible values with display name and description from local or global DI registration.
    /// The keys must match <see cref="GetValues(IContainer, IServiceProvider)"/>.
    /// </summary>
    public virtual IEnumerable<EntryPossible> GetPossibleValues(IContainer localContainer, IServiceProvider serviceProvider) =>
        EntryPossible.FromStrings(GetValues(localContainer, serviceProvider));

    /// <summary>
    /// Extract possible values with display name and description from the given <paramref name="context"/>.
    /// The keys must match <see cref="GetValues(PossibleValuesContext)"/>.
    /// </summary>
    /// <param name="context">context of the possible value</param>
    public virtual IEnumerable<EntryPossible> GetPossibleValues(PossibleValuesContext context = null) =>
        EntryPossible.FromStrings(GetValues(context));

    /// <summary>
    /// Parse value from string using local or global DI container
    /// </summary>
    /// <param name="container">Module local DI container</param>
    /// <param name="serviceProvider">Global service registration</param>
    /// <param name="value">Value to parse</param>
    public virtual object Parse(IContainer container, IServiceProvider serviceProvider, string value)
    {
        return value;
    }
}
