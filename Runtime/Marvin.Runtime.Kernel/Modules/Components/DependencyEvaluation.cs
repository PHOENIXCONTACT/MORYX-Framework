using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel
{
    internal class DependencyEvaluation : IDependencyEvaluation
    {
        /// <summary>
        /// Full dependency tree
        /// </summary>
        public IModuleDependencyTree FullTree { get; set; }

        /// <summary>
        /// Number of root modules
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
        /// Maximum number of dependends
        /// </summary>
        public int MaxDependends { get; set; }
    }
}
