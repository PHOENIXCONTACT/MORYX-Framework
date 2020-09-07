// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moryx.Logging;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel
{
    internal class FacadeContainer
    {
        public object Instance { get; set; }
        public IServerModule ManagingModule { get; set; }
    }

    internal class ImportingProperty
    {
        public bool Missing { get; set; }

        public PropertyInfo Property { get; set; }

        public RequiredModuleApiAttribute Attribute { get; set; }
    }

    internal class ModuleDependencyManager : IModuleDependencyManager
    {
        private readonly IModuleLogger _logger;

        private DependencyEvaluation _dependencyEvaluation;

        /// <summary>
        /// Cache of allready transformed branches
        /// </summary>
        private readonly IDictionary<IServerModule, IModuleDependency> _cache = new Dictionary<IServerModule, IModuleDependency>();

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
            return _dependencyEvaluation;
        }

        /// <summary>
        /// Get all start dependencies of this plugin
        /// </summary>
        public IModuleDependency GetDependencyBranch(IServerModule plugin)
        {
            return _cache.ContainsKey(plugin) ? _cache[plugin] : null;
        }

        public IReadOnlyList<IServerModule> BuildDependencyTree(IReadOnlyList<IServerModule> allModules)
        {
            // Fill facade dependencies
            var facadeProviders = BuildFacadeCache(allModules);
            var availableModules = FillDependencies(allModules, facadeProviders);
            
            // Build dependency tree from available modules
            var dependencyBranches = Convert(availableModules, facadeProviders);
            var tree = new PluginDependencyTree(dependencyBranches.Where(c => c.Dependencies.Count == 0));
            _dependencyEvaluation = ValidateDependencyTree(tree);

            // Only return modules that could be propery integrated in the dependency tree
            return dependencyBranches.Select(db => db.RepresentedModule).ToList();
        }

        #region Build dependency tree
        private Dictionary<object, IServerModule> BuildFacadeCache(IEnumerable<IServerModule> allServices)
        {
            var facadeProviders = new Dictionary<object, IServerModule>();
            foreach (var service in allServices)
            {
                var facadeContainers = service.GetType().GetInterfaces().Where(IsInterfaceFacadeContainer);
                foreach (var facadeContainer in facadeContainers)
                {
                    // Trust me, I hate this hard-coded string as much as you do. I just ran out of ideas
                    // var facadeInstance = facadeContainer.GetProperty("Facade").GetValue(service);
                    // Thanks to C# 6 we could finally get rid of this. We left it for the giggles
                    var facadeInstance = facadeContainer.GetProperty(nameof(IFacadeContainer<object>.Facade)).GetValue(service);
                    facadeProviders[facadeInstance] = service;
                }
            }

            return facadeProviders;
        }

        private bool IsInterfaceFacadeContainer(Type interfaceType)
        {
            return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IFacadeContainer<>);
        }

        private IReadOnlyList<IServerModule> FillDependencies(IEnumerable<IServerModule> modules, IDictionary<object, IServerModule> facadeProviders)
        {
            var satisfiedModules = new List<IServerModule>();
            foreach (var module in modules)
            {
                var importingProperties = GetImportingProperties(module);
                foreach (var importingProperty in importingProperties)
                {
                    var propType = importingProperty.Property.PropertyType;
                    if (typeof(Array).IsAssignableFrom(propType))
                        FillArray(propType, importingProperty, module, facadeProviders);
                    else
                        FillFacade(propType, importingProperty, module, facadeProviders);
                }

                // Make sure no imports are missing
                if (importingProperties.All(ip => !ip.Missing))
                    satisfiedModules.Add(module);
            }

            return satisfiedModules;
        }

        private void FillArray(Type propType, ImportingProperty importingProperty, IServerModule module, IDictionary<object, IServerModule> facadeProviders)
        {
            var elemType = propType.GetElementType();
            var facades = facadeProviders.Keys.Where(elemType.IsInstanceOfType).ToArray();
            var facadeArray = Array.CreateInstance(elemType, facades.Length);
            for (int i = 0; i < facades.Length; i++)
            {
                facadeArray.SetValue(facades[i], i);
            }
            importingProperty.Property.SetValue(module, facadeArray);
        }


        private void FillFacade(Type propType, ImportingProperty importingProperty, IServerModule service, IDictionary<object, IServerModule> facadeProviders)
        {
            var match = facadeProviders.Keys.FirstOrDefault(propType.IsInstanceOfType);
            if (match != null)
            {
                importingProperty.Property.SetValue(service, match);
            }
            else if (importingProperty.Attribute.IsOptional)
            {
                _logger.Log(LogLevel.Warning, "Found no module hosting a facade of type {0} which is referenced by {1}.{2}",
                    propType.Name, service.Name, importingProperty.Property);
            }
            else
            {
                _logger.Log(LogLevel.Error, "Found no module hosting a facade of type {0} which is required by {1}.{2}",
                    propType.Name, service.Name, importingProperty.Property);
                importingProperty.Missing = true;
            }
        }

        private IReadOnlyList<ModuleDependencyBranch> Convert(IReadOnlyList<IServerModule> availableModules, IDictionary<object, IServerModule> facadeProviders)
        {
            // Convert roots first
            var converted = new List<ModuleDependencyBranch>();
            var rootModules = availableModules.Where(m => !StartDependencies(m, facadeProviders).Any()).ToArray();
            foreach (var module in rootModules)
            {
                var rootBranch = new ModuleDependencyBranch(module);
                _cache[module] = rootBranch;
                converted.Add(rootBranch);
            }
            // Unconverted modules
            var remaining = new List<IServerModule>(availableModules);
            remaining.RemoveAll(rootModules.Contains);

            // Now keep iterating all available modules and convert every module with satisfied dependencies
            int preLoopCount;
            do
            {
                preLoopCount = converted.Count;

                // Use for loop to iterate remaining while modifying it
                for (var index = 0; index < remaining.Count; index++)
                {
                    var module = remaining[index];
                    // Go over all dependencies and convert if they were converted before
                    var dependencies = StartDependencies(module, facadeProviders);
                    if (dependencies.Any(dep => converted.All(con => con.RepresentedModule != dep)))
                        continue; // Unconverted dependency

                    var branch = new ModuleDependencyBranch(module);
                    _cache[module] = branch;
                    converted.Add(branch);

                    // Loop over dependencies and cross-link
                    foreach (var dependency in dependencies.Select(d => converted.First(cd => cd.RepresentedModule == d)))
                    {
                        branch.Dependencies.Add(dependency);
                        dependency.Dependends.Add(branch);
                    }

                    remaining.Remove(module);
                }
            } while (remaining.Count > 0 && preLoopCount < converted.Count); // Break when a loop did not provide additional conversion

            return converted;
        }

        private IReadOnlyList<IServerModule> StartDependencies(IServerModule module, IDictionary<object, IServerModule> facadeProviders)
        {
            // Get all dependencies of this plugin service
            var dependencyServices = new List<IServerModule>();
            foreach (var importingProperty in GetImportingProperties(module))
            {
                if(!importingProperty.Attribute.IsStartDependency)
                    continue;

                var propType = importingProperty.Property.PropertyType;
                if (propType.IsArray)
                    propType = propType.GetElementType();
                var dependencyProviders = facadeProviders.Where(facadePair => propType.IsInstanceOfType(facadePair.Key));
                foreach (var dependencyProvider in dependencyProviders)
                    dependencyServices.Add(dependencyProvider.Value);
            }

            return dependencyServices;
        }

        private static IReadOnlyList<ImportingProperty> GetImportingProperties(IServerModule plugin)
        {
            var dependencyAttributes = from prop in plugin.GetType().GetProperties()
                                       let att = prop.GetCustomAttribute<RequiredModuleApiAttribute>()
                                       where att != null
                                       select new ImportingProperty { Property = prop, Attribute = att };
            return dependencyAttributes.ToList();
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
