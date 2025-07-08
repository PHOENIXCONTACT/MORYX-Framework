// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

//using Moryx.AbstractionLayer.Resources;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Position = Moryx.Factory.Position;

namespace Moryx.FactoryMonitor.Endpoints.Model
{
    /// <summary>
    /// Class used to send TrnasportPath to the client
    /// </summary>
    [DataContract]
    public class TransportPathModel
    {
        [DataMember]
        public virtual CellLocationModel Origin { get; set; }

        [DataMember]
        public virtual CellLocationModel Destination { get; set; }

        [DataMember]
        public virtual List<Position> WayPoints { get; set; } = new List<Position>();
    }
}

