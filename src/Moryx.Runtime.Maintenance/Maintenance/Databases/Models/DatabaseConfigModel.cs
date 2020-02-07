// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Databases
{
    /// <summary>
    /// Configuration of the database.
    /// </summary>
    public class DatabaseConfigModel
    {
        /// <summary>
        /// Databse server
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Port on server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Database to use
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Databse user
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Password for the username
        /// </summary>
        public string Password { get; set; }
    }
}
