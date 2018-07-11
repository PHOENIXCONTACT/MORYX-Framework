using System;
using System.Collections.Generic;
using System.Reflection;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <inheritdoc />
    internal class AdvancedEditorVisibleSerialization : PossibleValuesSerialization
    {
        private static readonly EditorVisibleSerialization EditorVisbleFilter = new EditorVisibleSerialization();

        /// <inheritdoc />
        public AdvancedEditorVisibleSerialization(IContainer container, IEmptyPropertyProvider emptyPropertyProvider) : base(container, emptyPropertyProvider)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            return EditorVisbleFilter.GetMethods(sourceType);
        }
    }
}
