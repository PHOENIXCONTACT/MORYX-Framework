using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement
{
    /// <summary>
    /// Application configuration.
    /// </summary>
    [DataContract]
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Constructor for the ApplicationConfiguration
        /// </summary>
        public ApplicationConfiguration()
        {
            Modules = new List<ModuleConfiguration>();
        }

        /// <summary>
        /// Name of the application.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The configuration of the application.
        /// </summary>
        [DataMember]
        public ModuleConfiguration Shell { get; set; }

        /// <summary>
        /// List of <see cref="ModuleConfiguration"/>.
        /// </summary>
        [DataMember]
        public List<ModuleConfiguration> Modules { get; set; }
    }
}
