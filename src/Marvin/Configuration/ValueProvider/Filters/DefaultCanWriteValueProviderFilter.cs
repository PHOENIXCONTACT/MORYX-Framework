using System.Reflection;

namespace Marvin.Configuration
{
    /// <summary>
    /// ValueProviderFilter that skips read only properties
    /// </summary>
    public sealed class DefaultCanWriteValueProviderFilter : IValueProviderFilter
    {
        /// <inheritdoc />
        public bool CheckProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.CanWrite;
        }
    }
}
