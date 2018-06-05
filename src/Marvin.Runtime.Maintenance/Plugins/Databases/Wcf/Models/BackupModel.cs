using System;

namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Configuration of the backup.
    /// </summary>
    public class BackupModel
    {
        /// <summary>
        /// Name of the backup file.
        /// </summary>
        public string FileName { get; set; }
   
        /// <summary>
        /// Size of the current backup file.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Date of the backup creation.
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}