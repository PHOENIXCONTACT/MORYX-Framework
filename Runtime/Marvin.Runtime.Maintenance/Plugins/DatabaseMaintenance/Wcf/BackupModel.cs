using System;
using System.Runtime.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    /// <summary>
    /// Configuration of the backup.
    /// </summary>
    [DataContract]
    public class BackupModel
    {
        /// <summary>
        /// Name of the backup file.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }
   
        /// <summary>
        /// Size of the current backup file.
        /// </summary>
        [DataMember]
        public int Size { get; set; }

        /// <summary>
        /// Flag to tell if the backup is for the target model.
        /// </summary>
        [DataMember]
        public bool IsForTargetModel { get; set; }

        /// <summary>
        /// Date of the backup creation.
        /// </summary>
        [DataMember]
        public DateTime CreationDate { get; set; }
    }
}