namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Request to execute a database setup
    /// </summary>
    public class ExecuteSetupRequest
    {
        /// <summary>
        /// Configuration of the database
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// Setup to be exectued
        /// </summary>
        public SetupModel Setup { get; set; }
    }
}
