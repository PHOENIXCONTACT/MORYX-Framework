// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ProcessData.Configuration;

namespace Moryx.ProcessData.Endpoints.Models
{
    public class ConfiguredBindings
    {
        /// <summary>
        /// List of all configured bindings for this measurand
        /// </summary>
        public List<MeasurementBinding> Bindings { get; set; }
    }
}
