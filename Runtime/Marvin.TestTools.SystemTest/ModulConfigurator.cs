using System;
using System.IO;
using System.Linq;
using Marvin.Serialization;
using Marvin.TestTools.SystemTest.Maintenance;
using NUnit.Framework;

namespace Marvin.TestTools.SystemTest
{
    /// <summary>
    /// Helper to modify the configutration servermodul
    /// </summary>
    public class ModulConfigurator
    {
        private readonly HeartOfGoldController _hogController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModulConfigurator"/> class.
        /// </summary>
        /// <param name="hogController">The hog controller.</param>
        public ModulConfigurator(HeartOfGoldController hogController)
        {
            _hogController = hogController;
        }

        /// <summary>
        /// Gets the "trunk" servermodul configuration.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>the "trunk" configuration</returns>
        public Config GetServiceConfig(string serviceName)
        {
            return _hogController.GetConfig(serviceName);
        }

        /// <summary>
        /// Help method to go through a servermoduls configuration.
        /// Gets a Entry by its path. The path must be splited by '\\'.
        /// </summary>
        /// <param name="serviceConfig">The server modul "trunk" configuration.</param>
        /// <param name="pathOfConfigValues">The path to the configuration value; splited by '\\'</param>
        /// <returns>Configuration at the end of the path</returns>
        private Entry BrowseToConfig(Config serviceConfig, string pathOfConfigValues)
        {
            // split the config path
            var configPathSteps = pathOfConfigValues.Split('\\');
            var index = 0;
            var configName = configPathSteps[index];
            int? configIndex = null;

            if (configName.EndsWith("]"))
            {
                var splits = configName.Split('[');
                configName = splits[0];
                var indexString = splits[1].TrimEnd(']');
                configIndex = int.Parse(indexString);
            }

            Entry foundEntry = null;

            if (configIndex == null)
            {
                // Search the trunk Entry for a matching entry
                foreach (Entry Entry in serviceConfig.Entries)
                {
                    // does the displaying value matches to the first entry of the path?
                    if (Entry.Key.Identifier == configName)
                    {
                        Assert.IsNull(foundEntry, "Found more than one config entry model named '{0}'. You must provide the index of the config entry model!", configName);
                        foundEntry = Entry;
                    }
                }
            }
            else
            {
                var currentIndex = -1;
                // Search the trunk Entry for a matching entry
                foreach (var Entry in serviceConfig.Entries)
                {
                    // does the displaying value matches to the first entry of the path?
                    if (Entry.Key.Identifier == configName)
                    {
                        currentIndex++;
                        if (currentIndex == configIndex)
                        {
                            foundEntry = Entry;
                            break;
                        }
                    }
                }                
            }
            
            // Fail if the config entry wasn't found
            Assert.IsNotNull(foundEntry, "Config entry '{0}' was not found in the 'trunk' servermodul config!", configName);

            // index of the config path 
            index++;
            // step throught the next steps of the configuration.
            return StepThroughEntryModel(foundEntry, configPathSteps, ref index);
       }

        /// <summary>
        /// Steps through the sub configuration.
        /// Recursiv method to find a config entry in the sub entrys of a configuration.
        /// </summary>
        /// <param name="currentConfig">The current configuration entry that was found before.</param>
        /// <param name="configPath">The path to the configuration value; splited by '\\'</param>
        /// <param name="index">The index of the current path step</param>
        /// <returns>the found configuration</returns>
        private Entry StepThroughEntryModel(Entry currentConfig, string[] configPath, ref int index)
        {
            // check if last index has been reached.
            if (index >= configPath.Length)
                return currentConfig;

            var configName = configPath[index];
            Entry foundEntry = null;

            // Search for a given entry in the sub entries
            foreach (var Entry in currentConfig.SubEntries)
            {
                // does the displaying value matches to current path?
                if (Entry.Key.Identifier == configName)
                {
                    Assert.IsNull(foundEntry, "Found more than one config entry model named '{0}'. You must provide the index of the config entry model!", configName);
                    foundEntry = Entry;
                }
            }

            // Fail if the config entry wasn't found
            Assert.IsNotNull(foundEntry,"Config entry '{0}' was not found in the sub config of '{1}'!", configName, currentConfig.Key);
            
            // next index of the config path 
            index++;
            // step throught the next steps of the configuration.
            return StepThroughEntryModel(foundEntry, configPath, ref index);
        }


        /// <summary>
        /// Adds a configuration entry to a server moduls configuration.
        /// </summary>
        /// <param name="serviceConfig">The 'trunk' configuration of the server modul.</param>
        /// <param name="pathOfConfigValues">The path to the configuration value; splited by '\\'</param>
        /// <param name="configToAdd">The name of the configuration to add.</param>
        /// <param name="EntryIndex">Index of the added configuration entry.</param>
        /// <returns>
        /// The changed 'trunk' configuration
        /// </returns>
        public Config AddConfiguration(Config serviceConfig, string pathOfConfigValues, string configToAdd, out int EntryIndex)
        {
            return AddOrChangeConfigurationType(serviceConfig, pathOfConfigValues, configToAdd, out EntryIndex);
        }

        /// <summary>
        /// Adds a configuration entry to a server moduls configuration.
        /// </summary>
        /// <param name="serviceConfig">The 'trunk' configuration of the server modul.</param>
        /// <param name="itemPath">The path to the configuration value; splited by '\\'</param>
        /// <param name="newConfigType">The name of the configuration to add.</param>
        /// <param name="EntryIndex">Index of the added configuration entry (for collection entrys).</param>
        /// <returns>
        /// The changed 'trunk' configuration
        /// </returns>
        public Config AddOrChangeConfigurationType(Config serviceConfig, string itemPath, string newConfigType, out int EntryIndex)
        {
            EntryIndex = -1;
 
            // find the config entry
            Entry Entry = BrowseToConfig(serviceConfig, itemPath);

            // check if there are posible values listed and if the given name is listed.
            if (Entry.Value.Possible != null)
                if (!Entry.Value.Possible.Contains(newConfigType))
                    Assert.Fail("ConfigType '{0}' is not in the list of posible values!", newConfigType);

            // Request a replacement for the current entry
            var requestedConfig = _hogController.RequestEntry(serviceConfig.Module, Entry, newConfigType);

            // check if it is a collection or a singel object
            if (Entry.Value.Type == EntryValueType.Collection)
            {
                Entry.SubEntries.Add(requestedConfig);
                // Get the index of the added item to identify it later.
                EntryIndex = Entry.SubEntries.IndexOf(requestedConfig);
            }
            else
            {
                // Get the real parent config object 
                var pathSplit = itemPath.Split('\\');
                Entry parentEntry = null;

                if (pathSplit.Length > 1)
                {
                    // Go one level higher
                    string parentPath = Path.Combine(pathSplit.Take(pathSplit.Length - 1).ToArray());
                    parentEntry = BrowseToConfig(serviceConfig, parentPath);
                }

                // Use the real parent or the base config as parent
                if (parentEntry == null)
                {
                    // Remove the old entry and add the new generated item.
                    serviceConfig.Entries.Remove(Entry);
                    serviceConfig.Entries.Add(requestedConfig);
                }
                else
                {
                    // Remove the old entry and add the new generated item.
                    parentEntry.SubEntries.Remove(Entry);
                    parentEntry.SubEntries.Add(requestedConfig);
                }
            }

            // save the changes
            _hogController.SetConfig(serviceConfig);
            return _hogController.GetConfig(serviceConfig.Module);       
        }


        /// <summary>
        /// Changes the configuration.
        /// </summary>
        /// <param name="serviceConfig">The 'trunk' servermodul configuration.</param>
        /// <param name="pathOfConfigValue">The path to the configuration value; splited by '\\'</param>
        /// <param name="value">The new value</param>
        /// <returns>Server modul configuration with changed value (in a subentry)</returns>
        public Config ChangeConfig(Config serviceConfig, string pathOfConfigValue, string value)
        {
            // find the config entry
            Entry Entry = BrowseToConfig(serviceConfig, pathOfConfigValue);

            // check if there are posible values and if the given name is listed.
            if (Entry.Value.Possible != null)
                if (!Entry.Value.Possible.Contains(value))
                    Assert.Fail("ConfigValue '{0}' is not in the list of posible values!", value);

            // change the config value
            Entry.Value.Current = value;

            // save the changes
            _hogController.SetConfig(serviceConfig);
            return _hogController.GetConfig(serviceConfig.Module);
        }
    }
}
