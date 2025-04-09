// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer.Resources
{
    /// <summary>
    /// Component that manages the resource type tree
    /// </summary>
    public interface IResourceTypeTree
    {
        /// <summary>
        /// Type tree starting at <see cref="Resource"/>
        /// </summary>
        IResourceTypeNode RootType { get; }

        /// <summary>
        /// Indexed selection of a resource type
        /// </summary>
        IResourceTypeNode this[string typeName] { get; }

        /// <summary>
        /// Find all types that implement the given type constraint
        /// </summary>
        IEnumerable<IResourceTypeNode> SupportedTypes(Type constraint);

        /// <summary>
        /// Find all resource type trees that implement the given type constraints
        /// </summary>
        IEnumerable<IResourceTypeNode> SupportedTypes(ICollection<Type> constraints);
    }
}
