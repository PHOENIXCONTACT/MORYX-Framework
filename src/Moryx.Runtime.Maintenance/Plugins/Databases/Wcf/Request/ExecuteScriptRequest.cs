// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Plugins.Databases
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
