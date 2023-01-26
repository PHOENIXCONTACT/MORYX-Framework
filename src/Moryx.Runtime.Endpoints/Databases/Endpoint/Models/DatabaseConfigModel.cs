// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Models
{
    /// <summary>
    /// Configuration of the database.
    /// </summary>
    public class DatabaseConfigModel
    {
        /// <summary>
        /// Name of the database configurator type
        /// </summary>
        public string ConfiguratorTypename { get; set; }

        /// <summary>
        /// Configuration of the database model.
        /// </summary>
        public Dictionary<string, string> Entries { get;set; }
    }
}
