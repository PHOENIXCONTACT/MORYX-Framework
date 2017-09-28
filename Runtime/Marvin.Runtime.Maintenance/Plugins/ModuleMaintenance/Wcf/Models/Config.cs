using System.Collections.Generic;
using System.Runtime.Serialization;
using Marvin.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.ModuleMaintenance.Wcf
{
    /// <summary>
    /// Class which contains all config entries for one module.
    /// </summary>
    [DataContract]
    public class Config
    {
        /// <summary>
        /// Constructor for the configuration class.
        /// </summary>
        public Config()
        {
            Entries = new List<Entry>();
        }

        /// <summary>
        /// Name of the module for which the configuration belongs.
        /// </summary>
        [DataMember]
        public string Module { get; set; }

        /// <summary>
        /// List of <see cref="Entry"/>'s for this module.
        /// </summary>
        [DataMember]
        public List<Entry> Entries { get; set; } 
    }
}
