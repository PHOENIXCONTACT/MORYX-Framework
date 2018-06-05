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
        /// Name of the backup that shall be restored
        /// </summary>
        public string BackupFileName { get; set; }
    }
}
