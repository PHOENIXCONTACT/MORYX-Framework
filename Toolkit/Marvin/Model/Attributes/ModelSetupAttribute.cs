using System;
using Marvin.Testing;

namespace Marvin.Model
{
    /// <summary>
    /// Attribute for IModelSetups to determine their target model
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModelSetupAttribute : Attribute
    {
        /// <summary>
        /// Target model of this setup
        /// </summary>
        public string TargetModelNamespace { get; set; }
    }
}
