﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.ProcessData.Endpoints.Models
{
    public class MeasurandBindings
    {
        /// <summary>
        /// List of all available bindings for this measurand
        /// </summary>
        public List<string> Bindings { get; set; }
    }
}
