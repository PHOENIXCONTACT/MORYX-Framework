using System.Reflection;
using System.Runtime.Serialization;
using Marvin.Configuration;

namespace Marvin.Resources.Management
{
    public class DataMemberAttributeValueProviderFilter : IValueProviderFilter
    {
        private readonly bool _filterDataMembers;

        public DataMemberAttributeValueProviderFilter(bool filterDataMembers)
        {
            _filterDataMembers = filterDataMembers;
        }

        public bool CheckProperty(PropertyInfo propertyInfo)
        {
            if(_filterDataMembers)
                return propertyInfo.GetCustomAttribute<DataMemberAttribute>() == null;
            return propertyInfo.GetCustomAttribute<DataMemberAttribute>() != null;
        }
    }
}
