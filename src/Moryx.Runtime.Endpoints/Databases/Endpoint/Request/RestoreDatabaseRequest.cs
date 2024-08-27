// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Endpoints.Databases.Endpoint.Models;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Request
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
