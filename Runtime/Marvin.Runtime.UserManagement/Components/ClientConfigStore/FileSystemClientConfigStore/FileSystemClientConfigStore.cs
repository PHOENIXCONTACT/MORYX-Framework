using System.Collections.Generic;
using System.IO;
using System.Linq;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Runtime.UserManagement.ClientConfigStore
{
    /// <summary>
    /// Client config store which stores the config from the clinet at the file system
    /// </summary>
    [Plugin(LifeCycle.Singleton, typeof(IClientConfigStore), Name = ComponentName)]
    [ExpectedConfig(typeof(FileSystemClientConfigStoreConfig))]
    public class FileSystemClientConfigStore : IClientConfigStore
    {
        internal const string ComponentName = "FileSystemClientConfigStore";
        
        #region Fields

        private FileSystemClientConfigStoreConfig _config;

        private readonly Dictionary<string, List<ClientConfigModel>> _storedConfigurations = new Dictionary<string, List<ClientConfigModel>>();

        #endregion

        #region IConfiguredModulePlugin

        /// <summary>
        /// Initializes the component with the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public void Initialize(ClientConfigStoreConfigBase config)
        {
            _config = (FileSystemClientConfigStoreConfig) config;

            //Check and create client config directory
            CheckConfigDirectory();
        }

        /// <summary>
        /// Start internal execution of active and/or periodic functionality.
        /// </summary>
        public void Start()
        {
            //reload clinet configs to the memory
            LoadClientConfigs();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            SaveClientConfigs();
            _storedConfigurations.Clear();
        }

        #endregion

        #region IClientConfigStore

        /// <summary>
        /// Get a config model from a specific library.
        /// </summary>
        public ClientConfigModel GetConfiguration(string clientId, string typeName)
        {
            if (!_storedConfigurations.ContainsKey(clientId))
            {
                return null;
            }

            var clientConfigs = _storedConfigurations[clientId];
            var config = clientConfigs.FirstOrDefault(c => c.TypeName.Equals(typeName));

            return config;
        }

        /// <summary>
        /// Save a user configuration model.
        /// </summary>
        public bool SaveConfiguration(string clientId, ClientConfigModel configModel)
        {
            if (!_storedConfigurations.ContainsKey(clientId))
            {
                _storedConfigurations.Add(clientId, new List<ClientConfigModel>());
            }

            var clientConfigs = _storedConfigurations[clientId];
            var config = clientConfigs.FirstOrDefault(c => c.TypeName.Equals(configModel.TypeName));

            if (config == null)
            {
                clientConfigs.Add(new ClientConfigModel(configModel.TypeName, configModel.JsonText));
            }
            else
            {
                config.JsonText = configModel.JsonText;
            }

            SaveConfig(configModel, GetClientFolderName(clientId), true);

            return true;
        }

        #endregion

        #region File Handling 

        /// <summary>
        /// Loads all client configs out of the configured folder
        /// </summary>
        private void LoadClientConfigs()
        {
            var folders = Directory.GetDirectories(_config.ConfigFolder);

            foreach (var folder in folders)
            {
                var clientId = new DirectoryInfo(folder).Name;
                var configFilePathes = Directory.GetFiles(folder, $"*{ConfigConstants.FileExtension}");

                var configObjects = (from configFilePath in configFilePathes
                                     let fileName = Path.GetFileNameWithoutExtension(configFilePath)
                                     let content = File.ReadAllText(configFilePath)
                                     select new ClientConfigModel(fileName, content)).ToList();

                _storedConfigurations.Add(clientId, configObjects);
            }
        }

        /// <summary>
        /// This method will save all current configs out of the cache.
        /// </summary>
        private void SaveClientConfigs()
        {
            foreach (var storedConfiguration in _storedConfigurations)
            {
                var clientFolder = GetClientFolderName(storedConfiguration.Key);
                CheckAndCreateDirectory(clientFolder);

                foreach (var config in storedConfiguration.Value)
                {
                    //folder will not be checked while saving. Checked before for all configs
                    SaveConfig(config, clientFolder, false);
                }
            }
        }

        /// <summary>
        /// Returns a complete path to the folder for the specified client
        /// </summary>
        private string GetClientFolderName(string clientId)
        {
            return Path.Combine(_config.ConfigFolder, clientId);
        }

        /// <summary>
        /// Will save the given config model in the given folder. 
        /// CheckFolder activates the check and creation of the config folder.
        /// </summary>
        private void SaveConfig(ClientConfigModel config, string folder, bool checkFolder)
        {
            if (checkFolder)
            {
                CheckAndCreateDirectory(folder);
            }

            var filePath = Path.Combine(folder, config.TypeName + ConfigConstants.FileExtension);
            File.WriteAllText(filePath, config.JsonText);
        }

        /// <summary>
        /// Checks the configuration of the config directory.
        /// </summary>
        /// <exception cref="Marvin.Configuration.InvalidConfigException">ConfigFolder</exception>
        private void CheckConfigDirectory()
        {
            if (string.IsNullOrEmpty(_config.ConfigFolder))
                throw new InvalidConfigException(_config.ConfigFolder, "ConfigFolder");

            CheckAndCreateDirectory(_config.ConfigFolder);
        }

        /// <summary>
        /// Checks and creates the given directory.
        /// </summary>
        private void CheckAndCreateDirectory(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        #endregion
    }
}