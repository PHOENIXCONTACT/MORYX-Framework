using System.Reflection;

namespace Moryx.Configuration
{
    /// <summary>
    /// Makes sure that all fields with a class type were filled
    /// </summary>
    public class ConcreteClassesInitializedValidator : IValueProviderValidator
    {
        /// <inheritdoc/>
        public bool CheckProperty(PropertyInfo propertyInfo, object parentObject)
        {
            if(propertyInfo.PropertyType.IsClass && !propertyInfo.PropertyType.IsAbstract)
            {
                return propertyInfo.GetValue(parentObject) != null;
            }
            return true;
        }
    }
}
