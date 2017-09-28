using System.Reflection;

namespace Marvin.Serialization
{
    /// <summary>
    /// Wrapper factory based on the name
    /// </summary>
    internal class NameWrapperFactory : ITypeWrapperFactory
    {
        /// <summary>
        /// Indicates that this reader can read this property
        /// </summary>
        public bool CanWrap(PropertyInfo property)
        {
            return true;
        }

        /// <summary>
        /// Create wrapper around the property
        /// </summary>
        /// <param name="property">Property that shall be wrapped</param>
        /// <returns>Wrapped property</returns>
        public PropertyTypeWrapper Wrap(PropertyInfo property)
        {
            var wrapper = new PropertyTypeWrapper(property)
            {
                Key = property.Name,
                Required = false
            };
            return wrapper;
        }
    }
}