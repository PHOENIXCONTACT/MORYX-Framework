using System.Runtime.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    /// <summary>
    /// Conatins the data to the target model.
    /// </summary>
    [DataContract]
    public class DataModel
    {
        /// <summary>
        /// Name of the target model..
        /// </summary>
        [DataMember]
        public string TargetModel { get; set; }

        /// <summary>
        /// Configuration of the database model.
        /// </summary>
        [DataMember]
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// An amount of setups for this model.
        /// </summary>
        [DataMember]
        public SetupModel[] Setups { get; set; }

        /// <summary>
        /// An amount of backups of this model.
        /// </summary>
        [DataMember]
        public BackupModel[] Backups { get; set; }

        /// <summary>
        /// An amount of scripts for this model.
        /// </summary>
        [DataMember]
        public ScriptModel[] Scripts { get; set; }
    }
}