// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Model
{
    /// <summary>
    /// Database provider specific connection settings
    /// </summary>
    [DataContract]
    public class DatabaseConnectionSettings
    {
        /// <summary>
        /// Database name
        /// </summary>
        [DataMember]
        public virtual string Database { get; set; }

        /// <summary>
        /// Connection string for the specified data provider
        /// </summary>
        [DataMember]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Validates if the settings are valid or not
        /// </summary>
        /// <returns>true, if valid</returns>
        public bool IsValid()
            => !string.IsNullOrEmpty(Database) && !string.IsNullOrEmpty(ConnectionString);
    }
}
