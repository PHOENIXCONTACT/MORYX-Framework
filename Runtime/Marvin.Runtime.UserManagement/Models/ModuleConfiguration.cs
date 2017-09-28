using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Marvin.Runtime.UserManagement
{
    /// <summary>
    /// Module configuration.
    /// </summary>
    [DataContract]
    public class ModuleConfiguration
    {
        /// <summary>
        /// Constructor for the module configuration.
        /// </summary>
        public ModuleConfiguration()
        {
            Dependencies = new List<string>();
        }
        
        /// <summary>
        /// Name of the application.
        /// </summary>
        [DataMember]
        public string Application { get; set; }

        /// <summary>
        /// Name of the library where this module belongs to.
        /// </summary>
        [DataMember]
        public string Library { get; set; }

        /// <summary>
        /// List of dependecies of this module.
        /// </summary>
        [DataMember]
        public List<string> Dependencies { get; set; }

        /// <summary>
        /// Enable / disable this modul.
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }

        /// <summary>
        /// The sort index.
        /// </summary>
        [DataMember]
        public int SortIndex { get; set; }

        /// <summary>
        /// Name which will be shown and overrides the displayname.
        /// </summary>
        [DataMember]
        public string OverriddenDisplayName { get; set; }
    }
}
