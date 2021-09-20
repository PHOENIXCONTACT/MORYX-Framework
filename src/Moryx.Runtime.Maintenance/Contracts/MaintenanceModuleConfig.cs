// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;
using Moryx.Modules;
#if USE_WCF
using Moryx.Tools.Wcf;
#endif

namespace Moryx.Runtime.Maintenance
{
    /// <summary>
    /// Base configuration for a maintenance plugin.
    /// </summary>
    [DataContract]
    public abstract class MaintenancePluginConfig : UpdatableEntry, IPluginConfig
    {
        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public abstract string PluginName { get; }
        
        /// <summary>
        /// Endpoint which the plugin provides.
        /// </summary>
        [DataMember]
        public HostConfig ProvidedEndpoint { get; set; }
    }

#if !USE_WCF
    /// <summary>
    /// Config clone for compatibility
    /// </summary>
    public class HostConfig
    {
        public string Endpoint { get; set; }
        
        public string BindingType { get; set; }
        
        public bool RequiresAuthentification { get; set; }
        
        public bool MetadataEnabled { get; set; }
        
        public bool HelpEnabled { get; set; }
    }
#endif
}
