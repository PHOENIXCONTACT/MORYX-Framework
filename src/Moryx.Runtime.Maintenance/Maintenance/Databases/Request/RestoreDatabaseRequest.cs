// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Databases
{
    /// <summary>
    /// Request to restore a database 
    /// </summary>
    public class RestoreDatabaseRequest
    {
        /// <summary>
        /// Configuration of the database
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// Name of the backup that shall be restored
        /// </summary>
        public string BackupFileName { get; set; }
    }
}
