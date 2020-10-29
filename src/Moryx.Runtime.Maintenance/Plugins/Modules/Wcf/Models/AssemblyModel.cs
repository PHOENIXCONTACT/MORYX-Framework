// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Plugins.Modules
{
    /// <summary>
    /// Holds important information about an assembly.
    /// </summary>
    public class AssemblyModel
    {
        /// <summary>
        /// Name of the assembly.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of the assembly.
        /// </summary>
        public string Version { get; set; }
    }
}
