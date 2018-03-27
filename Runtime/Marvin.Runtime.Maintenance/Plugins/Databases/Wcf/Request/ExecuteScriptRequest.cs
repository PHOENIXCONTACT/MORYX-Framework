namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Request to execute a database script
    /// </summary>
    public class ExecuteScriptRequest
    {
        /// <summary>
        /// Configuration of the database
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// Script to be executed
        /// </summary>
        public ScriptModel Script { get; set; }
    }
}
