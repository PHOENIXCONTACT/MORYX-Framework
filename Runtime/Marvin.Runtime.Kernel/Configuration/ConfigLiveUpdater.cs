using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.Configuration;

namespace Marvin.Runtime.Kernel.Configuration
{
    internal class ConfigLiveUpdater : IConfigLiveUpdate
    {
        public void UpdateLive(Type sharedType, IConfig activeConfig, IConfig modifiedCopy)
        {
            UpdateObject(sharedType, activeConfig, modifiedCopy);
        }

        private bool UpdateObject(Type sharedType, object target, object source)
        {
            var modifiedProperties = (from propertyInfo in sharedType.GetProperties() 
                                      where UpdateProperty(propertyInfo, target, source) 
                                      select propertyInfo.Name).ToList();

            if (modifiedProperties.Any() & target is IUpdatableConfig)
                ((IUpdatableConfig)target).RaiseConfigChanged(modifiedProperties.ToArray());

            return modifiedProperties.Any();
        }

        private bool UpdateProperty(PropertyInfo prop, object target, object source)
        {
            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
            {
                // Add and remove of entries is not supported jet
                var collectionModified = false;
                var targetEnum = ((IEnumerable)prop.GetValue(target)).GetEnumerator();
                var sourceEnum = ((IEnumerable) prop.GetValue(source)).GetEnumerator();
                while (targetEnum.MoveNext() && sourceEnum.MoveNext())
                {
                    var sharedType = prop.PropertyType.GetGenericArguments()[0];
                    collectionModified |= UpdateObject(GetSharedType(sharedType, targetEnum.Current, sourceEnum.Current), targetEnum.Current, sourceEnum.Current);
                }
                return collectionModified;
            }

            var targetValue = prop.GetValue(target);
            var sourceValue = prop.GetValue(source);
            if (targetValue != null && targetValue.Equals(sourceValue) || sourceValue == null) 
                return false;

            if (prop.PropertyType.IsClass && prop.PropertyType != typeof(string) 
                && (targetValue != null && targetValue.GetType() == sourceValue.GetType()))
                return UpdateObject(GetSharedType(prop.PropertyType, targetValue, sourceValue), targetValue, sourceValue);
            
            prop.SetValue(target, sourceValue);
            return true;
        }

        private Type GetSharedType(Type sharedType, object target, object source)
        {
            // Check if both objects are identical or have the same type
            Type targetType = target.GetType(), sourceType;
            if (target == source || target.GetType() == (sourceType = source.GetType()))
                return targetType;

            // Check if they are are direct relatives
            if (targetType.IsInstanceOfType(source))
                return targetType;
            if (sourceType.IsInstanceOfType(targetType))
                return sourceType;

            // Check if their inheritance trees meet somewhere
            var targetsBases = GetBaseTypes(targetType);
            var sourceBases = GetBaseTypes(sourceType);
            var common = targetsBases.FirstOrDefault(sourceBases.Contains);

            return common ?? sharedType;
        }

        private List<Type> GetBaseTypes(Type targetType)
        {
            var targetsBases = new List<Type>();
            var baseType = targetType.BaseType;
            while (baseType != typeof(object))
            {
                targetsBases.Add(baseType);
                baseType = baseType.BaseType;
            }
            return targetsBases;
        }
    }
}
