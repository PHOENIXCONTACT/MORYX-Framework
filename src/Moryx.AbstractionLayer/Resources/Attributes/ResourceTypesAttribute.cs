// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Serialization;

namespace Moryx.AbstractionLayer.Resources;

/// <summary>
/// Attribute to decorate properties or method parameters of type string and return
/// all possible resource types.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class ResourceTypesAttribute : PossibleValuesAttribute
{
    /// <summary>
    /// String remains string
    /// </summary>
    public override bool OverridesConversion => false;

    /// <summary>
    /// Do not use last value
    /// </summary>
    public override bool UpdateFromPredecessor => false;

    /// <summary>
    /// Type constraint
    /// </summary>
    public Type TypeConstraint { get; }

    /// <summary>
    /// Create a new <see cref="ResourceTypesAttribute"/> for the given constraint type
    /// </summary>
    public ResourceTypesAttribute(Type typeConstraint)
    {
        TypeConstraint = typeConstraint;
    }

    /// <inheritdoc />
    public override IEnumerable<string> GetValues(IContainer pluginContainer, IServiceProvider serviceProvider)
    {
        return SelectCreatableTypes(pluginContainer).Select(linker => linker.Name);
    }

    /// <inheritdoc />
    public override IEnumerable<EntryPossible> GetPossibleValues(IContainer localContainer, IServiceProvider serviceProvider)
    {
        return SelectCreatableTypes(localContainer).Select(linker => linker.ResourceType.ToEntryPossible(linker.Name));
    }

    private IEnumerable<IResourceTypeNode> SelectCreatableTypes(IContainer localContainer)
    {
        var typeController = localContainer.Resolve<IResourceTypeTree>();
        return SelectCreatableTypes(typeController.SupportedTypes(TypeConstraint), new List<IResourceTypeNode>());
    }

    /// <summary>
    /// Recursive function to select all creatable resource types
    /// </summary>
    private static IEnumerable<IResourceTypeNode> SelectCreatableTypes(IEnumerable<IResourceTypeNode> linkers, ICollection<IResourceTypeNode> nodes)
    {
        foreach (var linker in linkers)
        {
            if (linker.Creatable)
            {
                nodes.Add(linker);
            }

            SelectCreatableTypes(linker.DerivedTypes, nodes);
        }
        return nodes;
    }
}
