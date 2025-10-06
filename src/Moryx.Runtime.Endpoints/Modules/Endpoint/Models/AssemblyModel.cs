// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Endpoints.Modules.Endpoint.Models
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
        /// AssemblyVersion of the assembly.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// AssemblyFileVersion of the assembly.
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// AssemblyInformationalVersion of the assembly.
        /// </summary>
        public string InformationalVersion { get; set; }
    }
}
