using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Filters methods and properties with <see cref="EditorVisibleAttribute"/> set.
    /// </summary>
    public class EditorVisibleSerialization : DefaultSerialization
    {
        /// <inheritdoc />
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            var properties = base.GetProperties(sourceType).ToList();
            return properties.Where(p => Attribute.IsDefined(p, typeof(EditorVisibleAttribute)));
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            var methods = base.GetMethods(sourceType);

            methods = Attribute.IsDefined(sourceType, typeof(EditorVisibleAttribute))
                ? methods.Where(method => method.DeclaringType != typeof(object)) // Filter methods defined by object
                : methods.Where(method => Attribute.IsDefined(method, typeof(EditorVisibleAttribute))); // Filter methods carrying the editor visible attribute

            return methods;
        }
    }
}
