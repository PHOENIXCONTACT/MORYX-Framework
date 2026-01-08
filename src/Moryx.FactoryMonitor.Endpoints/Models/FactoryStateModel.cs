// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{
    /// <summary>
    /// Simple model of factory
    /// </summary>
    [DataContract]
    public class FactoryStateModel : VisualizableItemModel
    {

        [DataMember]
        public virtual string BackgroundURL { get; set; }

        [DataMember]
        public virtual bool HasManufacturingFactory { get; set; }

        [DataMember]
        public List<OrderModel> OrderModels { get; set; }

        [DataMember]
        public List<ActivityChangedModel> ActivityChangedModels { get; set; }

        [DataMember]
        public List<CellStateChangedModel> CellStateChangedModels { get; set; }

        [DataMember]
        public List<ResourceChangedModel> ResourceChangedModels { get; set; }
    }
}

