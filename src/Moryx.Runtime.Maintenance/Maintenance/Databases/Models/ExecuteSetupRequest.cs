// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Databases
{
    /// <summary>
    /// Request to execute a database setup
    /// </summary>
    public class ExecuteSetupRequest
    {
        /// <summary>
        /// Configuration of the database
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// Setup to be exectued
        /// </summary>
        public SetupModel Setup { get; set; }
    }
}
