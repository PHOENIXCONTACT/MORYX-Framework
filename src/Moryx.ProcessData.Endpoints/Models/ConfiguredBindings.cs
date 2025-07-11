// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ProcessData.Configuration;
using System.Collections.Generic;

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
