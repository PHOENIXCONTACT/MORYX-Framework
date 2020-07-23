// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Configuration for the ModuleManager.
    /// </summary>
    [DataContract]
    public class ModuleManagerConfig : ConfigBase
    {
        /// <summary>
        /// Restricts the maximum of dependecies the module can have on other modules.
        /// </summary>
        [DataMember]
        public int MaxModuleDependencies { get; set; }

        /// <summary>
        /// restricts the maximum of other modules which depend on this module.
        /// </summary>
        [DataMember]
        public int MaxModuleDependends { get; set; }

        /// <summary>
        /// List of configurations of managed modules.
        /// </summary>
        [DataMember]
        public List<ManagedModuleConfig> ManagedModules { get; set; }

        /// <summary>
        /// Gets or create the configuraton of the managed module.
        /// </summary>
        /// <param name="moduleName">The module for which the configuration should be fetched.</param>
        /// <returns>A ManagedModuleConfig for the requested module name.</returns>
        public ManagedModuleConfig GetOrCreate(string moduleName)
        {
            var config = ManagedModules.FirstOrDefault(item => item.ModuleName == moduleName);
            if (config == null)
            {
                config = new ManagedModuleConfig {ModuleName = moduleName};
                ManagedModules.Add(config);
            }
            return config;
        }
    }

    /// <summary>
    /// Configuration of a managed module.
    /// </summary>
    [DataContract]
    public class ManagedModuleConfig
    {
        /// <summary>
        /// The name of the managed module.
        /// </summary>
        [DataMember]
        public string ModuleName { get; set; }

        /// <summary>
        /// Start behavior of the managed module. See <see cref="ModuleStartBehaviour"/> for behavior information.
        /// </summary>
        [DataMember]
        public ModuleStartBehaviour StartBehaviour { get; set; }

        /// <summary>
        /// Failure behavior of the managed module. See <see cref="FailureBehaviour"/> for behavir information.
        /// </summary>
        [DataMember]
        public FailureBehaviour FailureBehaviour { get; set; }
    }
}
