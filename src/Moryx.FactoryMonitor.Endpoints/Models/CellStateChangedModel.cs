// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.FactoryMonitor.Endpoints.Model;

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

