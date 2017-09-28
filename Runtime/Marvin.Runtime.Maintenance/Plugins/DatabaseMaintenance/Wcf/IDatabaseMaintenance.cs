using System.ServiceModel;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Plugins.DatabaseMaintenance.Wcf
{
    /// <summary>
    /// Service contracts for database operations.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.0.0.0", MinClientVersion = "1.0.0.0")]
    public interface IDatabaseMaintenance
    {
        /// <summary>
        /// Configuration of the database.
        /// </summary>
        DatabaseConfig Config { get; set; }

        /// <summary>
        /// Get all database configs
        /// </summary>
        /// <returns>The fetched DataModels.</returns>
        [OperationContract]
        DataModel[] GetDataModels();

        /// <summary>
        /// Overwrite all configs with this one
        /// </summary>
        [OperationContract]
        void SetAllConfigs(DatabaseConfigModel config);

        /// <summary>
        /// Set database config
        /// </summary>
        [OperationContract]
        void SetDatabaseConfig(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Test a new config
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool TestDatabaseConfig(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Create all datamodels with current config
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string CreateAll();

        /// <summary>
        /// Create a new database matching the model
        /// </summary>
        /// <returns>True if database could be created</returns>
        [OperationContract]
        string CreateDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Drop all data models
        /// </summary>
        [OperationContract]
        string EraseAll();

        /// <summary>
        /// Erases the database given by the model
        /// </summary>
        /// <returns>True if erased successful</returns>
        [OperationContract]
        string EraseDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Dumps the database matching the model to create a restoreable backup
        /// </summary>
        /// <returns>True if async dump is in progress</returns>
        [OperationContract]
        DumpResult DumpDatabase(string targetModel, DatabaseConfigModel config);

        /// <summary>
        /// Restores the database.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string RestoreDatabase(string targetModel, DatabaseConfigModel config, BackupModel backupModel);

        /// <summary>
        /// Execute setup for this config
        /// </summary>
        [OperationContract]
        string ExecuteSetup(string targetModel, DatabaseConfigModel config, SetupModel setup);

        /// <summary>
        /// Executed a databasescript for the given model
        /// </summary>
        [OperationContract]
        string ExecuteScript(string targetModel, DatabaseConfigModel model, ScriptModel script);
    }
}
