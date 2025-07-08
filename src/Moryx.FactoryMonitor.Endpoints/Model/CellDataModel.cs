// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Model
{
    /// <summary>
    /// Model containing all the data about a resource (Activity, process,state etc..)
    /// </summary>
    [DataContract]
    [Obsolete("Use CellStateChanged model instead")]
    public class CellDataModel
    {      
        [DataMember]
        public virtual CellState State { get; set; }

        [DataMember]
        public virtual ProcessModel ProcessActivity { get; set; }

        [DataMember]
        public virtual List<TransportPathModel> ForkliftDispatchRoutes { get; set; }
    }
}

