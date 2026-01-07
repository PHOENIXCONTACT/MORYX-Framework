// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0
using Moryx.Container;
using Moryx.Serialization;
using Moryx.Tools;

namespace Moryx.Maintenance;
/// <summary>
/// Creates a new instance of the <see cref="EntryPrototypesAttribute"/>
/// </summary>
/// <param name="type">Type of the service which should be selectable</param>
public class EntryPrototypesAttribute(Type type) : PossibleValuesAttribute
{
    /// <summary>
    /// Type of the service which should be selectable
    /// </summary>
    protected Type type { get; } = type;

    /// <summary>
    /// All possible values for this member represented as strings. The given container might be null
    /// and can be used to resolve possible values
    /// </summary>
    public override IEnumerable<string> GetValues(IContainer container, IServiceProvider provider)
    {
        var possibleValues = GetPossibleTypes();
        return possibleValues.Select(configType => configType.Name);
    }

    /// <inheritdoc />
    public override bool OverridesConversion => true;

    /// <inheritdoc />
    public override bool UpdateFromPredecessor => true;

    /// <inheritdoc />
    public override object Parse(IContainer container, IServiceProvider serviceProvider, string value)
    {
        var possibleTypes = GetPossibleTypes();
        return Activator.CreateInstance(possibleTypes.First(type => type.Name == value));
    }

    private IEnumerable<Type> GetPossibleTypes()
    {
        return ReflectionTool.GetPublicClasses(type)
            .Where(x => !x.IsAbstract);
    }
}
