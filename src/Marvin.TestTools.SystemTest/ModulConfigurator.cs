// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Runtime.Maintenance.Plugins.Modules;
using Marvin.Serialization;
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
                foreach (Entry Entry in serviceConfig.Root.SubEntries)
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
                foreach (var Entry in serviceConfig.Root.SubEntries)
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
        private static Entry StepThroughEntryModel(Entry currentConfig, string[] configPath, ref int index)
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
    }
}
