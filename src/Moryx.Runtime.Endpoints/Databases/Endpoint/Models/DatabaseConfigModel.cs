// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Models
{
    /// <summary>
    /// Configuration of the database.
    /// </summary>
    public class DatabaseConfigModel
    {
        /// <summary>
        /// Database connection string. Given a connection string, it has
        /// highest priority over all other properties.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The database provider to use with a `ConnectionString`
        /// Takes a fully qualified assembly name
        /// </summary>
        public string ConfiguratorTypename { get; set; }

        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }
    }
}
