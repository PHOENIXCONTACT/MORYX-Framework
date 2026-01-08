// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// Locaiton of the cell inside the factory
    /// </summary>
    [DataContract]
    public class CellLocationModel
    {
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual double PositionX { get; set; }

        [DataMember]
        public virtual double PositionY { get; set; }

    }
}

