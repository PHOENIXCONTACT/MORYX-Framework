// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;

namespace Moryx.Runtime.Kestrel.Maintenance
{
    /// <summary>
    /// Configuration of the maintenance module.
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {
        /// <inheritdoc />
        protected override bool PersistDefaultConfig => true;

        public LoggingMaintenanceConfig LoggingMaintenanceConfig { get; set; }

        public DatabaseConfig DatabaseConfig { get; set; }


        /// <summary>
        /// Initialize the maintenance module.
        /// </summary>
        protected override void Initialize()
        {
        }
    }
}
