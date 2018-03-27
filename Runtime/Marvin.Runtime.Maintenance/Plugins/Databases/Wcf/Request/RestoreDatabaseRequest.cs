namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Request to restore a database 
    /// </summary>
    public class RestoreDatabaseRequest
    {
        /// <summary>
        /// Configuration of the database
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// Backup which should be restored
        /// </summary>
        public BackupModel BackupModel { get; set; }
    }
}
