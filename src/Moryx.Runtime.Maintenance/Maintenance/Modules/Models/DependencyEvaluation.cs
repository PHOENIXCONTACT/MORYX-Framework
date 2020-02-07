// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Maintenance.Modules
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
        /// <param name="source">The source which should be used.</param>
        public DependencyEvaluation(IDependencyEvaluation source)
        {
            RootModules = source.RootModules;
            MaxDepth = source.MaxDepth;
            MaxDependencies = source.MaxDependencies;
            MaxDependends = source.MaxDependends;
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
    }
}
