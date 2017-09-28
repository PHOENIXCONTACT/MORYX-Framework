using System.Runtime.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    /// <summary>
    /// Model for database scripts.
    /// </summary>
    [DataContract]
    public class ScriptModel
    {
        /// <summary>
        /// Name of the script.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Flag to find out if this script is needed for database creation.
        /// </summary>
        [DataMember]
        public bool IsCreationScript { get; set; }
    }
}