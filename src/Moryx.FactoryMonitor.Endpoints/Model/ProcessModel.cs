// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Processes.Endpoints;
using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Model
{
    /// <summary>
    /// Extended ProcessActivityModel 
    /// </summary>
    public class ProcessModel : ProcessActivityModel
    {
        [DataMember]
        [Obsolete]
        public virtual OrderModel Order { get; set; }
    }
}

