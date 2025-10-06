// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Jobs.Endpoints.Models
{
    [DataContract]
    public class StateChangeModel
    {
        [DataMember]
        public JobModel JobModel { get; set; }

        [DataMember]
        public JobClassification PreviousState { get; set; }

        [DataMember]
        public JobClassification CurrentState { get; set; }

    }
}

