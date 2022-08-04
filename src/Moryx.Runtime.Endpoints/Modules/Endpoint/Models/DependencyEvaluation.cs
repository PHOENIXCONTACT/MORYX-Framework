// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Runtime.Endpoints.Modules.Endpoint.Models
{
    /// <summary>
    /// Evaluation summary calculated by the <see cref="IModuleManager"/>
    /// </summary>
    public class DependencyEvaluation
    {
        /// <summary>
        /// Default constructor for deserialization
        /// </summary>
        public DependencyEvaluation()
        {
        }

        /// <summary>
        /// Constructor for the dependency evaluation. Sets all properties from the given parameter.
        /// </summary>
        /// <param name="tree">The source which should be used.</param>
        public DependencyEvaluation(IModuleDependencyTree tree)
        {
            // Skip eval for empty list
            var allModules = tree.RootModules
                .Flatten(md => md.Dependends).ToList();
            if (tree.RootModules.Any())
            {
                RootModules = tree.RootModules.Count();
                MaxDepth = tree.RootModules.Max(branch => CalculateTreeDepth(1, branch));
                MaxDependencies = allModules.Max(item => item.Dependencies.Count());
                MaxDependends = allModules.Max(item => item.Dependends.Count());
            }
        }

        /// <summary>
        /// Number of root plugins
        /// </summary>
        public int RootModules { get; set; }

        /// <summary>
        /// Maximum dependency depth
        /// </summary>
        public int MaxDepth { get; set; }

        /// <summary>
        /// Maximum number of dependencies
        /// </summary>
        public int MaxDependencies { get; set; }

        /// <summary>
        /// Maximum number of dependents
        /// </summary>
        public int MaxDependends { get; set; }

        private int CalculateTreeDepth(int currentLevel, IModuleDependency branch)
        {
            var childLevel = currentLevel + 1;
            return branch.Dependends.Any() ? branch.Dependends.Max(dependency => CalculateTreeDepth(childLevel, dependency)) : currentLevel;
        }
    }
}
