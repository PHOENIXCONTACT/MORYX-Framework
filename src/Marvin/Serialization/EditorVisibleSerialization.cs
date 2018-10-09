using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Marvin.Serialization
{
    /// <summary>
    /// Filters methods and properties with <see cref="EditorVisibleAttribute"/> set.
    /// </summary>
    public class EditorVisibleSerialization : DefaultSerialization
    {
        private static readonly Lazy<EditorVisibleSerialization> LazyInstance 
            = new Lazy<EditorVisibleSerialization>(LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EditorVisibleSerialization Instance => LazyInstance.Value;

        /// <inheritdoc />
        public override IEnumerable<PropertyInfo> GetProperties(Type sourceType)
        {
            var properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return properties.Where(p => Attribute.IsDefined(p, typeof(EditorVisibleAttribute)));
        }

        /// <inheritdoc />
        public override IEnumerable<MethodInfo> GetMethods(Type sourceType)
        {
            var methods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m => !m.IsSpecialName);

            methods = Attribute.IsDefined(sourceType, typeof(EditorVisibleAttribute))
                ? methods.Where(method => method.DeclaringType != typeof(object)) // Filter methods defined by object
                : methods.Where(method => Attribute.IsDefined(method, typeof(EditorVisibleAttribute))); // Filter methods carrying the editor visible attribute

            return methods;
        }

        /// <inheritdoc />
        public override IEnumerable<ConstructorInfo> GetConstructors(Type sourceType)
        {
            return base.GetConstructors(sourceType).Where(c => Attribute.IsDefined(c, typeof(EditorVisibleAttribute)));
        }
    }
}
