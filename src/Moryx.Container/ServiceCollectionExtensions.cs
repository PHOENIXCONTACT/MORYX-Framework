using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Moryx.Container
{
    /// <summary>
    /// Extensions for the service collection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all types exported for registration from a given assembly
        /// </summary>
        public static IServiceCollection AddFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            // Iterate all types from the assembly
            foreach (var type in assembly.GetTypes().Where(ShallRegister))
            {
                if (type.IsInterface)
                {
                    services.AddFactory(type);
                }
                else
                {
                    Register(type, GetComponentServices(type));
                }
            }

            return services;
        }

        /// <summary>
        /// Find all implementations of a service in the AppDomain
        /// </summary>
        public static IServiceCollection AddFromAppDomain<TService>(this IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// Find all implementations of a service in the AppDomain
        /// </summary>
        public static IServiceCollection AddFromAppDomain<TService>(this IServiceCollection services, Predicate<Type> predicate)
        {
            return services;
        }

        /// <summary>
        /// Add an auto implemented factory for the given service interface
        /// </summary>
        public static IServiceCollection AddFactory<TFactory>(this IServiceCollection services)
        {
            return services.AddFactory(typeof(TFactory));
        }

        /// <summary>
        /// Add an auto implemented factory for the given service interface
        /// </summary>
        public static IServiceCollection AddFactory(this IServiceCollection services, Type factoryType)
        {
            return services;
        }

        private static bool ShallRegister(Type type)
        {
            return type.GetCustomAttribute<ComponentAttribute>() != null || type.GetCustomAttribute<PluginFactoryAttribute>() != null;
        }


        private static Type[] GetComponentServices(Type type)
        {
            throw new NotImplementedException();
        }

        private static void Register(Type type, Type[] types)
        {
            throw new NotImplementedException();
        }
    }
}
