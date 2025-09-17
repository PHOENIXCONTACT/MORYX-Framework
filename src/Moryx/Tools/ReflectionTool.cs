using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Moryx.Tools
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

        private static readonly Lazy<Assembly[]> RelevantAssemblies = new Lazy<Assembly[]>(LoadAssemblies, LazyThreadSafetyMode.ExecutionAndPublication);
        /// <summary>
        /// Load assemblies
        /// </summary>
        /// <returns></returns>
        private static Assembly[] LoadAssemblies()
        {
            // Fetch location of binaries from our assembly
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).ToLower();
            var relevantAssemblies = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      // Only load non-dynamic assemblies from our directory
                                      where !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location)
                                      let path = Path.GetDirectoryName(assembly.Location).ToLower()
                                      where TestMode || path == currentDir
                                      select assembly).ToArray();
            return relevantAssemblies;
        }

        /// <summary>
        /// Assemblies relevant for our application
        /// </summary>
        public static Assembly[] GetAssemblies()
        {
            return RelevantAssemblies.Value;
        }

        private static readonly Lazy<Type[]> PublicClasses = new Lazy<Type[]>(LoadPublicClasses, LazyThreadSafetyMode.ExecutionAndPublication);
        /// <summary>
        /// Load all public classes
        /// </summary>
        private static Type[] LoadPublicClasses()
        {
            // Assume 30 exports per assembly for initial size
            var publicClasses = new List<Type>(RelevantAssemblies.Value.Length * 30);
            foreach (var assembly in RelevantAssemblies.Value)
            {
                try
                {
                    var exports = assembly.GetExportedTypes()
                        .Where(type => type.IsClass && !type.IsAbstract);
                    publicClasses.AddRange(exports);
                }
                catch(Exception x)
                {
                    CrashHandler.WriteErrorToFile($"Failed to load types from {assembly.FullName}. Error: {x.Message}");
                }
            }

            return publicClasses.ToArray();
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

        /// <summary>
        /// Get public classes of a certain base type that match a filter
        /// </summary>
        public static Type[] GetPublicClasses(Type baseType, Predicate<Type> filter)
        {
            return (from publicClass in PublicClasses.Value
                    where baseType.IsAssignableFrom(publicClass) && filter(publicClass)
                    select publicClass).ToArray();
        }

        /// <summary>
        /// Generate a delegate for fast constructor invocation of types
        /// only known at runtime
        /// </summary>
        public static Func<object> ConstructorDelegate(Type objectType)
        {
            return ConstructorDelegate<object>(objectType);
        }

        /// <summary>
        /// Generate a delegate for fast constructor invocation of types
        /// only known at runtime
        /// </summary>
        /// <param name="objectType">Concrete object type to construct</param>
        /// <typeparam name="T">Lower bound common type, for example the interface all instances share</typeparam>
        public static Func<T> ConstructorDelegate<T>(Type objectType)
        {
            return Expression.Lambda<Func<T>>(
                    Expression.Convert(Expression.New(objectType), typeof(T)))
                .Compile();
        }

        /// <summary>
        /// Generates a delegate to perform fast type checks on given objects including
        /// instances of derived types
        /// </summary>
        /// <param name="targetType">The type to compare objects to</param>
        public static Func<object, bool> TypePredicate(Type targetType)
        {
            return TypePredicate(targetType, true);
        }

        /// <summary>
        /// Generates a delegate to perform fast type checks on given objects
        /// </summary>
        /// <param name="targetType">The type to compare objects to</param>
        /// <param name="derivedTypes">Indicator if derived types should be considered as matches</param>
        public static Func<object, bool> TypePredicate(Type targetType, bool derivedTypes)
        {
            var parameter = Expression.Parameter(typeof(object));
            var typeCompare = derivedTypes ? Expression.TypeIs(parameter, targetType) : Expression.TypeEqual(parameter, targetType);
            var expression = Expression.Lambda<Func<object, bool>>(typeCompare, parameter);
            return expression.Compile();
        }

        /// <summary>
        /// Create a fast, dynamic property accessor for the property info
        /// </summary>
        public static IPropertyAccessor<object, object> PropertyAccessor(PropertyInfo property)
        {
            return PropertyAccessor<object, object>(property);
        }

        /// <summary>
        /// Create a fast, dynamic property accessor for the property info
        /// </summary>
        public static IPropertyAccessor<TBase, object> PropertyAccessor<TBase>(PropertyInfo property)
            where TBase : class
        {
            return PropertyAccessor<TBase, object>(property);
        }

        /// <summary>
        /// Create a fast, dynamic property accessor for the property info
        /// </summary>
        public static IPropertyAccessor<TBase, TValue> PropertyAccessor<TBase, TValue>(PropertyInfo property)
            where TBase : class
        {
            Type accessorType;

            if (property.DeclaringType == typeof(TBase) && typeof(TValue) == property.PropertyType)
                accessorType = typeof(DirectAccessor<,>).MakeGenericType(typeof(TBase), typeof(TValue));

            else if (typeof(TBase).IsAssignableFrom(property.DeclaringType) && typeof(TValue) == property.PropertyType)
                accessorType = typeof(InstanceCastAccessor<,,>).MakeGenericType(property.DeclaringType, typeof(TBase), typeof(TValue));

            else if (typeof(TBase).IsAssignableFrom(property.DeclaringType) && typeof(TValue).IsAssignableFrom(property.PropertyType) && typeof(TValue) != typeof(object))
                accessorType = typeof(ValueCastAccessor<,,,>).MakeGenericType(property.DeclaringType, typeof(TBase), property.PropertyType, typeof(TValue));

            else if (typeof(TBase).IsAssignableFrom(property.DeclaringType) && property.PropertyType.IsAssignableFrom(typeof(TValue)) && typeof(TValue) != typeof(object))
                accessorType = typeof(PropertyCastAccessor<,,,>).MakeGenericType(property.DeclaringType, typeof(TBase), property.PropertyType, typeof(TValue));

            else
                accessorType = typeof(ConversionAccessor<,,,>).MakeGenericType(property.DeclaringType, typeof(TBase), property.PropertyType, typeof(TValue));

            return (IPropertyAccessor<TBase, TValue>)Activator.CreateInstance(accessorType, property);
        }

        /// <summary>
        /// Get all properties and values of a certain type
        /// </summary>
        public static IEnumerable<IGrouping<PropertyInfo, TReference>> GetReferences<TReference>(object instance)
        {
            return GetReferences<TReference>(instance, p => true);
        }

        /// <summary>
        /// Get all properties and values of a certain type
        /// </summary>
        public static IEnumerable<IGrouping<PropertyInfo, TReference>> GetReferences<TReference>(object instance, Predicate<PropertyInfo> predicate)
        {
            var type = instance.GetType();
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var property in properties)
            {
                if (typeof(TReference).IsAssignableFrom(property.PropertyType) && predicate(property))
                {
                    yield return new ReferenceGroup<TReference>(property, (TReference)property.GetValue(instance));
                }
                else if (typeof(IEnumerable<TReference>).IsAssignableFrom(property.PropertyType) && predicate(property))
                {
                    yield return new ReferenceGroup<TReference>(property, (IEnumerable<TReference>)property.GetValue(instance));
                }
            }
        }

        /// <summary>
        /// Checks it the given PropertyInfo is part of an explicit implementation
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns><c>true</c> if the property is part of an explicit implementation</returns>
        public static bool IsExplicitInterfaceImplementation(PropertyInfo propertyInfo)
        {
            // At least one accessor must exists
            return IsExplicitInterfaceImplementation(propertyInfo.GetMethod != null
                ? propertyInfo.GetMethod : propertyInfo.SetMethod);
        }

        /// <summary>
        /// Checks it the given MethodInfo is part of an explicit implementation
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns><c>true</c> if the method is part of an explicit implementation</returns>
        public static bool IsExplicitInterfaceImplementation(MethodInfo methodInfo)
        {
            // Based on ideas of: https://stackoverflow.com/questions/17853671/how-to-know-if-a-memberinfo-is-an-explicit-implementation-of-a-property

            // Iterate over interfaces to access interface map
            var declaringType = methodInfo.DeclaringType;
            foreach (var implementedInterface in declaringType.GetInterfaces())
            {
                var mapping = declaringType.GetInterfaceMap(implementedInterface);

                // Only look at declaring types
                if (mapping.TargetType != declaringType)
                    continue;

                for (var index = 0; index < mapping.TargetMethods.Length; index++)
                {
                    // If mapped method and interface method name do not match, it is explicit
                    if (mapping.TargetMethods[index] == methodInfo &&
                        mapping.InterfaceMethods[index]?.Name.Equals(methodInfo.Name, StringComparison.Ordinal) == false)
                        return true;
                }
            }

            return false;
        }
    }
}