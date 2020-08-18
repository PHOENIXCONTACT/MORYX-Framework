// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Runtime.Maintenance.Plugins.Databases
{
    /// <summary>
    /// Conatins the data to the target model.
    /// </summary>
    [DataContract]
    public class DataModel
    {
        /// <summary>
        /// Name of the target model..
        /// </summary>
        [DataMember]
        public string TargetModel { get; set; }

        /// <summary>
        /// Configuration of the database model.
        /// </summary>
        [DataMember]
        public DatabaseConfigModel Config { get; set; }

        /// <summary>
        /// An amount of setups for this model.
        /// </summary>
        [DataMember]
        public SetupModel[] Setups { get; set; }

        /// <summary>
        /// An amount of backups of this model.
        /// </summary>
        [DataMember]
        public BackupModel[] Backups { get; set; }

        /// <summary>
        /// Available migrations for this context
        /// </summary>
        [DataMember]
        public DbMigrationsModel[] AvailableMigrations { get; set; }

        /// <summary>
        /// Installed migrations for this context
        /// </summary>
        [DataMember]
        public DbMigrationsModel[] AppliedMigrations { get; set; }
    }
}
