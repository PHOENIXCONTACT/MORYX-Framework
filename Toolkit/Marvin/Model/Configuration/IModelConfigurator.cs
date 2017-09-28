using System.Collections.Generic;

namespace Marvin.Model
{
    /// <summary>
    /// Interface for interaction with model
    /// </summary>
    public interface IModelConfigurator
    {
        /// <summary>
        /// Target model of this configurator
        /// </summary>
        string TargetModel { get; }

        /// <summary>
        /// Get the config instance for this model
        /// </summary>
        /// <returns></returns>
        IDatabaseConfig Config { get; }

        /// <summary>
        /// Factory responsible for this data model
        /// </summary>
        IUnitOfWorkFactory ResponsibleFactory { get; }

        /// <summary>
        /// Test connection for config
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        bool TestConnection(IDatabaseConfig config);

        /// <summary>
        /// Create a new database for this model with given config
        /// </summary>
        /// <param name="config"></param>
        void CreateDatabase(IDatabaseConfig config);

        /// <summary>
        /// Update the currentdatabase to a new version if available
        /// </summary>
        /// <returns>True when an update was executed, false when this is allready the latest version</returns>
        UpdateSummary UpdateDatabase();

        /// <summary>
        /// Delete this database
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        void DeleteDatabase(IDatabaseConfig config);

        /// <summary>
        /// Dump the database und save the backup at the given file path
        /// This method works asynchronus
        /// </summary>
        /// <param name="config">Config describing the database target</param>
        /// <param name="filePath">Path to store backup</param>
        /// <returns>True if Backup is in progress</returns>
        void DumpDatabase(IDatabaseConfig config, string filePath);

        /// <summary>
        /// Restore this database with the given backup file
        /// </summary>
        /// <param name="config">Config to use</param>
        /// <param name="filePath">Filepath of dump</param>
        void RestoreDatabase(IDatabaseConfig config, string filePath);

        /// <summary>
        /// Get all setups for this model
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelSetup> GetAllSetups();

        /// <summary>
        /// Get all scripts of this model
        /// </summary>
        IEnumerable<IDatabaseScript> GetAllScripts(); 

        /// <summary>
        /// Execute setup for this config
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="setup">Setup</param>
        /// <param name="setupData"></param>
        void Execute(IDatabaseConfig config, IModelSetup setup, string setupData);
    }
}