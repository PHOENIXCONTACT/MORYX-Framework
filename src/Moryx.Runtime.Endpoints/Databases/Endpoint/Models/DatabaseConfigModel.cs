// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Configuration;
using Moryx.Serialization;

namespace Moryx.Runtime.Endpoints.Databases.Endpoint.Models
{
    /// <summary>
    /// Configuration of the database.
    /// </summary>
    public class DatabaseConfigModel
    {
        /// <summary>
        /// Configuration of the database model.
        /// </summary>
        [PossibleTypes(typeof(IDatabaseConfig))]
        public Entry Config { get; set; }
    }
}
