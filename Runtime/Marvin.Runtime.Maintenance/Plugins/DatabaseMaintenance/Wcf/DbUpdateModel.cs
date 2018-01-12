using System.Runtime.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    /// <summary>
    /// Model for database scripts.
    /// </summary>
    [DataContract]
    public class DbUpdateModel
    {
        /// <summary>
        /// Name of the script.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}