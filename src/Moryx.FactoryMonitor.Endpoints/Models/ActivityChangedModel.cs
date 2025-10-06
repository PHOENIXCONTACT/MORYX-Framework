// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Activities;
using System.Runtime.Serialization;

namespace Moryx.FactoryMonitor.Endpoints.Models
{

    /// <summary>
    /// Model for the cell information for every ActivityUpdated event.
    /// </summary>
    [DataContract]
    public class ActivityChangedModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long ResourceId { get; set; }

        [DataMember]
        public ActivityClassification Classification { get; set; }

        //Possible extensions: tracing, ...

        [DataMember]
        public OrderReferenceModel OrderReferenceModel { get; set; }

    }
}

