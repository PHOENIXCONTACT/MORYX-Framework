using System.Runtime.Serialization;
using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf
{
    /// <summary>
    /// Evaluation summary calculated by the <see cref="IModuleManager"/>
    /// </summary>
    [DataContract]
    public class DependencyEvaluation
    {
        /// <summary>
        /// Consructor for the dependency evaluation. Sets all properties from the given parameter.
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
        [DataMember]
        public int RootModules { get; set; }

        /// <summary>
        /// Maximum dependency depth
        /// </summary>
        [DataMember]
        public int MaxDepth { get; set; }

        /// <summary>
        /// Maximum number of dependencies
        /// </summary>
        [DataMember]
        public int MaxDependencies { get; set; }

        /// <summary>
        /// Maximum number of dependends
        /// </summary>
        [DataMember]
        public int MaxDependends { get; set; }
    }
}
