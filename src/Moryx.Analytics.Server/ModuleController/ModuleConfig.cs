// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Configuration;


namespace Moryx.Analytics.Server.ModuleController
{
    /// <summary>
    /// Module configuration of the MediaServer <see cref="ModuleController"/>
    /// </summary>
    [DataContract]
    public class ModuleConfig : ConfigBase
    {

        /// <summary>
        ///Dashboards
        /// </summary>
        [DataMember]
        public List<DashboardInformation> Dashboards { get; set; } = new List<DashboardInformation>();

    }
}
