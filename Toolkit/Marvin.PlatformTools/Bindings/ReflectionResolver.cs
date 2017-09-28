using System.Linq;
using System.Reflection;

namespace Marvin.Bindings
{
    /// <summary>
    /// Resolve binding using reflection
    /// </summary>
    public class ReflectionResolver : BindingResolverBase
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        private readonly string _propertyName;

        /// <summary>
        /// Create new <see cref="ReflectionResolver"/> for a property
        /// </summary>
        public ReflectionResolver(string propertyName)
        {
            _propertyName = propertyName;
        }

        /// <inheritdoc />
        public sealed override object Resolve(object source)
        {
            var type = source.GetType();

            // Find correct property by navigating down the type tree
            PropertyInfo property = null;
            var filter = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            while (property == null && type != null)
            {
                property = type.GetProperty(_propertyName, filter);
                type = type.BaseType;
            }

            return property == null ? null : Proceed(property.GetValue(source));
        }
    }
}