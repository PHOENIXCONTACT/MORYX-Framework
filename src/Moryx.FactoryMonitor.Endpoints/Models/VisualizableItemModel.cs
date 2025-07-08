﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.FactoryMonitor.Endpoints.Model;
using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{

    /// <summary>
    /// Model of an item/Resource that should be displayed in the FactoryMonitor view.
    /// </summary>
    [DataContract]
    public class VisualizableItemModel
    {
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual CellLocationModel Location { get; set; }

        [DataMember]
        public virtual string IconName { get; set; }

        [DataMember]
        public virtual bool IsACell { get; set; } = false;
    }
}

