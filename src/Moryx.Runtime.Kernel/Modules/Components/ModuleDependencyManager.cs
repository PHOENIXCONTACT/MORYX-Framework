// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.Logging;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    internal class FacadeContainer
    {
        public object Instance { get; set; }
        public IServerModule ManagingModule { get; set; }
    }

    internal class ImportingProperty
    {
        public PropertyInfo Property { get; set; }
        public RequiredModuleApiAttribute Attribute { get; set; }
    }

    internal class ModuleDependencyManager : ModuleManagerComponent, IModuleDependencyManager
    {
        private readonly IModuleLogger _logger;
        private DependencyEvaluation _dependencyEvalutaion;

        /// <summary>
        /// Cache of allready transformed branches
        /// </summary>
        private IDictionary<IServerModule, IModuleDependency> _cache;
        /// <summary>
        /// All facades found in the deployment. 
        /// - key is the facade instance
        /// - value is the host module
        /// </summary>
        private readonly IDictionary<object, IServerModule> _facadeCache = new Dictionary<object, IServerModule>();

        public ModuleDependencyManager(IModuleLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get the full dependency tree
        /// </summary>
        /// <returns></returns>
        public IDependencyEvaluation GetDependencyEvalutaion()
        {
            return _dependencyEvalutaion;
        }

        /// <summary>
        /// Get all start dependencies of this plugin
        /// </summary>
        public IModuleDependency GetDependencyBranch(IServerModule plugin)
        {
            return _cache.ContainsKey(plugin) ? _cache[plugin] : null;
        }

        public void BuildDependencyTree()
        {
            // Build dependency tree on first invocation
            BuildFacadeCache(AllModules());
            FillDependencies(AllModules());
            var roots = AllModules().Where(plugin => !StartDependencies(plugin).Any()).ToArray();
            var cache = new Dictionary<IServerModule, IModuleDependency>();

            var tree = new PluginDependencyTree(roots.Select(item => Convert(item, cache)).ToArray());
            _cache = cache;
            _dependencyEvalutaion = ValidateDependencyTree(tree);
        }

        #region Build dependency tree
        private void BuildFacadeCache(IEnumerable<IServerModule> allServices)
        {
            foreach (var service in allServices)
            {
                var facadeContainers = service.GetType().GetInterfaces().Where(IsInterfaceFacadeContainer);
                foreach (var facadeContainer in facadeContainers)
                {
                    // Trust me, I hate this hard-coded string as much as you do. I just ran out of ideas
                    // var facadeInstance = facadeContainer.GetProperty("Facade").GetValue(service);
                    // Thanks to C# 6 we could finally get rid of this. We left it for the giggles
                    var facadeInstance = facadeContainer.GetProperty(nameof(IFacadeContainer<object>.Facade)).GetValue(service);
                    _facadeCache[facadeInstance] = service;
                }
            }
        }

        private bool IsInterfaceFacadeContainer(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IFacadeContainer<>);
        }

        private void FillDependencies(IEnumerable<IServerModule> services)
        {
            foreach (var service in services)
            {
                var importingProperties = GetImportingProperties(service);
                foreach (var importingProperty in importingProperties)
                {
                    var propType = importingProperty.Property.PropertyType;
                    if (typeof(Array).IsAssignableFrom(propType))
                        FillArray(propType, importingProperty, service);
                    else
                        FillFacade(propType, importingProperty, service);
                }
            }
        }

        private void FillArray(Type propType, ImportingProperty importingProperty, IServerModule service)
        {
            var elemType = propType.GetElementType();
            var facades = _facadeCache.Keys.Where(elemType.IsInstanceOfType).ToArray();
            var facadeArray = Array.CreateInstance(elemType, facades.Length);
            for (int i = 0; i < facades.Length; i++)
            {
                facadeArray.SetValue(facades[i], i);
            }
            importingProperty.Property.SetValue(service, facadeArray);
        }


        private void FillFacade(Type propType, ImportingProperty importingProperty, IServerModule service)
        {
            var match = _facadeCache.Keys.FirstOrDefault(propType.IsInstanceOfType);
            if (match != null || importingProperty.Attribute.IsOptional)
            {
                if (match != null)
                    importingProperty.Property.SetValue(service, match);
                return;
            }

            _logger.Log(LogLevel.Error, "Found no plugin hosting a facade of type {0} which is required by {1}.{2}",
                propType.Name, service.Name, importingProperty.Property);
            throw new MissingFacadeException(service.Name, importingProperty.Property.Name, propType);
        }

        private IModuleDependency Convert(IServerModule plugin, IDictionary<IServerModule, IModuleDependency> cache)
        {
            var branch = new PluginDependencyBranch { RepresentedModule = plugin };
            cache[plugin] = branch;

            branch.Dependends = AllModules().Where(item => StartDependencies(item).Contains(plugin))
                                            .Select(item => cache.ContainsKey(item) ? cache[item] : Convert(item, cache)).ToArray();
            branch.Dependencies = StartDependencies(plugin).Select(item => cache.ContainsKey(item) ? cache[item] : Convert(item, cache)).ToArray();
            return branch;
        }

        private IEnumerable<IServerModule> StartDependencies(IServerModule plugin)
        {
            // Get all dependencies of this plugin service
            var dependencyServices = new List<IServerModule>();
            foreach (var importingProperty in GetImportingProperties(plugin))
            {
                var propType = importingProperty.Property.PropertyType;
                var dependencyProvider = _facadeCache.FirstOrDefault(facadePair => propType.IsInstanceOfType(facadePair.Key));
                if (dependencyProvider.Value != null && importingProperty.Attribute.IsStartDependency)
                    dependencyServices.Add(dependencyProvider.Value);
            }
            return dependencyServices;
        }

        private static IEnumerable<ImportingProperty> GetImportingProperties(IServerModule plugin)
        {
            var dependencyAttributes = from prop in plugin.GetType().GetProperties()
                                       let att = prop.GetCustomAttribute<RequiredModuleApiAttribute>()
                                       where att != null
                                       select new ImportingProperty { Property = prop, Attribute = att };
            return dependencyAttributes;
        }
        #endregion

        #region Validate dependency tree

        private DependencyEvaluation ValidateDependencyTree(IModuleDependencyTree tree)
        {
            var eval = new DependencyEvaluation { FullTree = tree };
            // Skip eval for empty list
            if (tree.RootModules.Any())
            {
                eval.RootModules = tree.RootModules.Count();
                eval.MaxDepth = tree.RootModules.Max(branch => CalculateTreeDepth(1, branch));
                eval.MaxDependencies = _cache.Values.Max(item => item.Dependencies.Count());
                eval.MaxDependends = _cache.Values.Max(item => item.Dependends.Count());
            }

            // For each element try to find it as a dependency of its dependencies
            foreach (var dependency in _cache.Values.Where(item => FindInDependencies(item, item)))
            {
                _logger.Log(LogLevel.Fatal, "Plugin dependency tree is not recursion free! Cause: {0}", dependency.RepresentedModule.Name);
            }

            return eval;
        }

        private int CalculateTreeDepth(int currentLevel, IModuleDependency branch)
        {
            var childLevel = currentLevel + 1;
            return branch.Dependends.Any() ? branch.Dependends.Max(dependency => CalculateTreeDepth(childLevel, dependency)) : currentLevel;
        }

        private bool FindInDependencies(IModuleDependency needle, IModuleDependency haystack)
        {
            return haystack.Dependencies.Any(dependency => dependency == needle
                                                        || FindInDependencies(needle, dependency));
        }

        #endregion
    }
}
