using Moryx.AbstractionLayer.Resources;
using System;
using System.Collections.Generic;

namespace Moryx.Resources.Management.Facades
{
    internal class ResourceTypeTreeFacade: IResourceTypeTree
    {
        #region Dependency Injection

        public IResourceTypeTree TypeTree { get; set; }
        #endregion
        #region IResourceTypeTree
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


        #endregion
    }
}
