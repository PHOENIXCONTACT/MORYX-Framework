// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Contains the data to the target model.
    /// </summary>
    public class DataModel
    {
        /// <summary>
        /// Name of the target model..
        /// </summary>
        public string TargetModel { get; set; }

        /// <summary>
        /// Configuration of the database model.
        /// </summary>
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// An amount of setups for this model.
        /// </summary>
        public SetupModel[] Setups { get; set; }

        /// <summary>
        /// An amount of backups of this model.
        /// </summary>
        public BackupModel[] Backups { get; set; }
    }
}
