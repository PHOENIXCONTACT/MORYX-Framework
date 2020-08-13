// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Modules
{
    /// <summary>
    /// Request to save the config of a module
    /// </summary>
    public class SaveConfigRequest
    {
        /// <summary>
        /// Config of the module
        /// </summary>
        public Config Config { get; set; }

        /// <summary>
        /// Update mode how the config will be applied
        /// </summary>
        public ConfigUpdateMode UpdateMode { get; set; }
    }
}
