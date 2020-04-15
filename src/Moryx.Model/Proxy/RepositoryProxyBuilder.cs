// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Moryx.Model
{
    /// <summary>
    /// Repository builder class
    /// </summary>
    public class RepositoryProxyBuilder
    {
        private const string AssemblyName = "DynamicModelRepository";
        private const string DynamicModuleName = "Proxies";
        private const string ProxySuffixName = "_MoryxProxy";

        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly IMethodProxyStrategy[] MethodStrategies;

        /// <summary>
        /// Static constructor to generate regex
        /// </summary>
        static RepositoryProxyBuilder()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(AssemblyName), AssemblyBuilderAccess.Run);
            ModuleBuilder = assemblyBuilder.DefineDynamicModule(DynamicModuleName);
            MethodStrategies = new IMethodProxyStrategy[]
            {
                new CreateMethodStrategy(),
                new FilterMethodStrategy(),
                new RemoveMethodStrategy()
            };
        }

        /// <summary>
        /// Creates new implementation of the given repository API.
        /// All methods will be implemented by the proxy
        /// </summary>
        /// <param name="repoApi">Interface of the repository</param>
        public Type Build(Type repoApi)
        {
            // Check interface
            ValidateRepositoryApi(repoApi, false);

            var proxyName = CreateProxyName(repoApi);

            var generatedType = ModuleBuilder.GetType(proxyName);
            if (generatedType != null)
                return generatedType;

            var entityType = repoApi.GetInterfaces().Single(i =>
                i.GetGenericArguments().Length == 1 &&
                typeof(IEntity).IsAssignableFrom(i.GetGenericArguments()[0])).GetGenericArguments()[0];

            // Create base type
            var baseType = typeof(Repository<>).MakeGenericType(entityType);

            // Get Methods and matching Strategies
            var methodStrategyMaps = GetInterfaceMethodStrategies(repoApi);

            // Create TypeBuilder
            var typeBuilder = ModuleBuilder.DefineType(proxyName, TypeAttributes.Public, baseType);
            typeBuilder.AddInterfaceImplementation(repoApi);

            // Do the method implementation
            ImplementMethods(typeBuilder, methodStrategyMaps, baseType, entityType);

            generatedType = typeBuilder.CreateType();

            return generatedType;
        }

        /// <summary>
        /// Creates new implementation of the given repository implementation.
        /// Abstract or virtual methods will be implemented
        /// </summary>
        /// <param name="repoApi">Interface of the repository</param>
        /// <param name="repoImpl">Implementation of the repository</param>
        public Type Build(Type repoApi, Type repoImpl)
        {
            // Check interface
            ValidateRepositoryApi(repoApi, false);

            var proxyName = CreateProxyName(repoImpl);

            var generatedType = ModuleBuilder.GetType(proxyName);
            if (generatedType != null)
                return generatedType;

            // Select base type
            var baseType = repoImpl.BaseType;
            if (baseType == null || baseType.GetGenericArguments().Length != 1)
                throw new InvalidOperationException($"{repoImpl.Name} must have the base type {nameof(Repository)}<T>.");

            var entityType = baseType.GetGenericArguments()[0];

            // Get Methods and matching Strategies
            var methodStrategyMaps = repoImpl.IsAbstract
                ? GetAbstractMethodStartegies(repoImpl)
                : GetVirutalMethodStrategies(repoImpl);

            // Create TypeBuilder
            var typeBuilder = ModuleBuilder.DefineType(proxyName, TypeAttributes.Public, repoImpl);

            // Do the method implementation
            ImplementMethods(typeBuilder, methodStrategyMaps, baseType, entityType);

            generatedType = typeBuilder.CreateType();

            return generatedType;
        }

        private static string CreateProxyName(Type type)
        {
            return (type.FullName?.Replace(".", "_") ?? type.Name) + ProxySuffixName;
        }

        private static MethodStrategyMap[] GetInterfaceMethodStrategies(Type repoApi)
        {
            // Load strategy for every method
            var methodStrategyMaps = repoApi.GetMethods().Select(methodInfo => new MethodStrategyMap
            {
                MethodInfo = methodInfo,
                Strategy = MethodStrategies.SingleOrDefault(s => s.CanImplement(methodInfo))
            }).ToArray();

            // Check for invalid strategies
            var invalidStrategy = methodStrategyMaps.FirstOrDefault(ms => ms.Strategy == null);
            if (invalidStrategy != null)
                throw new Exception($"Method '{invalidStrategy.MethodInfo.Name}' cannot be implemented because of invalid name.");

            return methodStrategyMaps;
        }

        private static MethodStrategyMap[] GetAbstractMethodStartegies(Type repoImpl)
        {
            // Class uses abstract methods
            var methods = repoImpl.GetMethods().Where(m => m.IsAbstract).ToArray();
            var methodStrategyMaps = methods.Select(methodInfo => new MethodStrategyMap
            {
                MethodInfo = methodInfo,
                Strategy = MethodStrategies.SingleOrDefault(s => s.CanImplement(methodInfo))
            }).ToArray();

            var invalidStrategy = methodStrategyMaps.FirstOrDefault(ms => ms.Strategy == null);
            if (invalidStrategy != null)
                throw new Exception($"Method '{invalidStrategy.MethodInfo.Name}' cannot be implemented because of invalid name.");

            return methodStrategyMaps;
        }

        private static MethodStrategyMap[] GetVirutalMethodStrategies(Type repoImpl)
        {
            // Virtual methods will be implemented
            var methods = repoImpl.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => m.IsVirtual);
            var methodStrategyMaps = (from methodInfo in methods
                let strategy = MethodStrategies.SingleOrDefault(s => s.CanImplement(methodInfo))
                where strategy != null // Methods which cannot be implemented can be ignored
                select new MethodStrategyMap
                {
                    MethodInfo = methodInfo,
                    Strategy = strategy
                }).ToArray();

            return methodStrategyMaps;
        }

        private static void ImplementMethods(TypeBuilder typeBuilder, IEnumerable<MethodStrategyMap> methodStrategyMap, Type baseType, Type entityType)
        {
            foreach (var methodStrategy in methodStrategyMap)
            {
                var strategy = methodStrategy.Strategy;
                var methodInfo = methodStrategy.MethodInfo;
                strategy.Implement(typeBuilder, methodInfo, baseType, entityType);
            }
        }

        private class MethodStrategyMap
        {
            public MethodInfo MethodInfo { get; set; }

            public IMethodProxyStrategy Strategy { get; set; }
        }

        private static void ValidateRepositoryApi(Type repoApi, bool additionalApis)
        {
            if (!repoApi.IsInterface)
            {
                throw new InvalidOperationException($"'{repoApi.Name}' is not an interface");
            }

            var repoInterfaces = repoApi.GetInterfaces();
            if (additionalApis && (repoInterfaces.Length > 2 || repoInterfaces.First().GetGenericTypeDefinition() != typeof(IRepository<>)))
            {
                throw new InvalidOperationException($"'{repoApi.Name}' API does not inherit from IRepository<T>");
            }
            else if (repoInterfaces.Length != 2 || repoInterfaces.First().GetGenericTypeDefinition() != typeof(IRepository<>))
            {
                 throw new InvalidOperationException($"'{repoApi.Name}' API does not inherit from IRepository<T>");
            }
        }
    }
}
