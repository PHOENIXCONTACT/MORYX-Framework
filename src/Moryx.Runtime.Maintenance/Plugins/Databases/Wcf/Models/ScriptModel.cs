// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Model for database scripts.
    /// </summary>
    public class ScriptModel
    {
        /// <summary>
        /// Name of the script.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Flag to find out if this script is needed for database creation.
        /// </summary>
        public bool IsCreationScript { get; set; }
    }
}
