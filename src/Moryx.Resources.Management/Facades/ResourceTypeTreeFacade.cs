// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Runtime.Modules;

namespace Moryx.Resources.Management
{
    internal class ResourceTypeTreeFacade : FacadeBase, IResourceTypeTree
    {
        #region Dependency Injection

        public IResourceTypeTree TypeTree { get; set; }

        #endregion

        public IResourceTypeNode RootType => TypeTree.RootType;

        public IResourceTypeNode this[string typeName] => TypeTree[typeName];

        public IEnumerable<IResourceTypeNode> SupportedTypes(Type constraint)
        {
            return TypeTree.SupportedTypes(constraint);
        }

        public IEnumerable<IResourceTypeNode> SupportedTypes(ICollection<Type> constraints)
        {
            return TypeTree.SupportedTypes(constraints);
        }
    }
}

