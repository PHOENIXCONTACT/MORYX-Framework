using System;
using System.Collections.Generic;
using Marvin.Configuration;
using Marvin.Container;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Attribute to decorate properties or method parameters of type string and return
    /// all possible resource types.
    /// </summary>
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

        /// <summary>
        /// Resolve all resource type names for a resource type
        /// </summary>
        public override IEnumerable<string> GetValues(IContainer pluginContainer)
        {
            var typeController = pluginContainer.Resolve<IResourceTypeTree>();
            return SelectTypeNames(typeController.SupportedTypes(TypeConstraint), new List<string>());
        }

        /// <summary>
        /// Recursive function to select all resource types
        /// </summary>
        private static IEnumerable<string> SelectTypeNames(IEnumerable<IResourceTypeNode> linkers, ICollection<string> names)
        {
            foreach (var linker in linkers)
            {
                if (linker.Creatable)
                    names.Add(linker.Name);
                SelectTypeNames(linker.DerivedTypes, names);
            }
            return names;
        }
    }
}