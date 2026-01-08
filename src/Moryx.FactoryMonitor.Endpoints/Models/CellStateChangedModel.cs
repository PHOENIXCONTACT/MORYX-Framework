// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// Model for the cell information for every ActivityUpdated event.
    /// </summary>
    [DataContract]
    public class CellStateChangedModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public CellState State { get; set; }

    }
}

