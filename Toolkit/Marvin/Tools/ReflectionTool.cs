using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Marvin.Tools
{
    /// <summary>
    /// Tool for common reflection operations
    /// </summary>
    public static class ReflectionTool
    {
        /// <summary>
        /// Assembly location check does not work in tests. This is not pretty but so far beats the alternatives
        /// </summary>
        public static bool TestMode { get; set; }

        private static Assembly[] _relevantAssemblies;
        /// <summary>
        /// Assemblies relevant for our application
        /// </summary>
        public static Assembly[] GetAssemblies()
        {
            if (_relevantAssemblies != null)
                return _relevantAssemblies;

            var currentDir = Directory.GetCurrentDirectory().ToLower();
            _relevantAssemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                   // Only load non-dynamic assemblies from our directory
                                   where !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location)
                                   let path = Path.GetDirectoryName(assembly.Location).ToLower()
                                   where TestMode || path == currentDir
                                   select assembly).ToArray();
            return _relevantAssemblies;
        }

        /// <summary>
        /// Get public classes of a certain base type
        /// </summary>
        public static Type[] GetPublicClasses<T>()
            where T : class
        {
            return GetPublicClasses(typeof(T), t => true);
        }

        /// <summary>
        /// Get public classes of a certain base type that match a filter
        /// </summary>
        public static Type[] GetPublicClasses<T>(Predicate<Type> filter)
        {
            return GetPublicClasses(typeof(T), filter);
        }

        /// <summary>
        /// Get public classes of a certain base type
        /// </summary>
        public static Type[] GetPublicClasses(Type baseType)
        {
            return GetPublicClasses(baseType, t => true);
        }

        private static Type[] _publicClasses;
        /// <summary>
        /// Get public classes of a certain base type that match a filter
        /// </summary>
        public static Type[] GetPublicClasses(Type baseType, Predicate<Type> filter)
        {
            if (_publicClasses == null)
            {
                _publicClasses = (from assembly in GetAssemblies()
                                  from type in assembly.GetExportedTypes()
                                  where type.IsClass && !type.IsAbstract
                                  select type).ToArray();
            }

            return (from publicClass in _publicClasses
                    where baseType.IsAssignableFrom(publicClass) && filter(publicClass)
                    select publicClass).ToArray();
        }
    }
}