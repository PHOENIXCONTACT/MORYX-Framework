// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Marvin.Serialization;

namespace Marvin.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Contract for a config response
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Entry converted configuration
        /// </summary>
        public Entry Root { get; set; }
    }
}
