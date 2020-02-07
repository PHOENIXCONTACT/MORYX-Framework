// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Databases
{
    /// <summary>
    /// Model for database scripts.
    /// </summary>
    public class DbMigrationsModel
    {
        /// <summary>
        /// Name of the script.
        /// </summary>
        public string Name { get; set; }
    }
}
