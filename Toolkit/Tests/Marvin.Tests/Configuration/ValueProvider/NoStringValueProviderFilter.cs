using System.Reflection;
using Marvin.Configuration;

namespace Marvin.Tests.Configuration.ValueProvider
{
    public class NoStringValueProviderFilter : IValueProviderFilter
    {
        public bool CheckProperty(PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType != typeof(string);
        }
    }
}
