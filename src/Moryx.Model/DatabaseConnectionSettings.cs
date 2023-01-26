// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Moryx.Model
{
    /// <summary>
    /// Database provider specific connection settings
    /// </summary>
    [DataContract]
    public class DatabaseConnectionSettings
    {
        private const string KeyDatabase = "Database";
        private const string KeyConnectionString = "ConnectionString";

        /// <summary>
        /// Database name
        /// </summary>
        [DataMember]
        public virtual string Database { get; set; }

        /// <summary>
        /// Connection string for the specified data provider
        /// </summary>
        [DataMember]
        public virtual string ConnectionString { get; set; } = "";

        /// <summary>
        /// Validates if the settings are valid or not
        /// </summary>
        /// <returns>true, if valid</returns>
        public bool IsValid()
            => !string.IsNullOrEmpty(Database) && !string.IsNullOrEmpty(ConnectionString);

        /// <summary>
        /// Converts `DataMember` properties into a dictionary
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string> {
                { KeyDatabase, Database },
                { KeyConnectionString, ConnectionString }
            };
        }

        /// <summary>
        /// Deserializes a dictionary into properties
        /// </summary>
        /// <returns></returns>
        public virtual void FromDictionary(Dictionary<string, string> dictionary)
        {
            ConnectionString = dictionary.GetValueOrDefault(KeyConnectionString);
            Database = dictionary.GetValueOrDefault(KeyDatabase);
        }
    }
}
