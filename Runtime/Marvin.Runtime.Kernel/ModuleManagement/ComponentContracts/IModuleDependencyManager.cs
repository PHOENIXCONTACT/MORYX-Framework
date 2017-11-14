using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.Kernel
{
    internal interface IModuleDependencyManager : IModuleManagerComponent
    {
        /// <summary>
        /// Build and fill the dependency tree
        /// </summary>
        void BuildDependencyTree();

        /// <summary>
        /// Get the full dependency tree
        /// </summary>
        /// <returns></returns>
        IDependencyEvaluation GetDependencyEvalutaion();

        /// <summary>
        /// Get all start dependencies of this plugin
        /// </summary>
        IModuleDependency GetDependencyBranch(IServerModule module);
    }
}
