// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Maintenance.Common
{
    /// <summary>
    /// Response contract for application information
    /// </summary>
    public class ApplicationInformationResponse
    {
        /// <summary>
        /// Product name of this application
        /// </summary>
        public string AssemblyProduct { get; set; }

        /// <summary>
        /// Product version of this application
        /// </summary>
        public string AssemblyVersion { get; set; }

        /// <summary>
        /// Informational version of this application
        /// </summary>
        public string AssemblyInformationalVersion { get; set; }

        /// <summary>
        /// Description of this application
        /// </summary>
        public string AssemblyDescription { get; set; }
    }
}
