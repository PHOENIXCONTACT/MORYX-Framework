﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;
using Moryx.FactoryMonitor.Endpoints.Model;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// Model for the cell information for every ResourceUpdated event.
    /// </summary>
    [DataContract]
    public class ResourceChangedModel : VisualizableItemModel
    {
        [DataMember]
        public virtual string CellName { get; set; }

        [DataMember]
        public virtual string CellIconName { get; set; }

        [DataMember]
        public virtual string CellImageURL { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public CellLocationModel CellLocation { get; set; }

        [DataMember]
        public Dictionary<string, CellPropertySettings> CellPropertySettings { get; set; }
        
        [DataMember]
        public long FactoryId { get; internal set; }
    }
}

