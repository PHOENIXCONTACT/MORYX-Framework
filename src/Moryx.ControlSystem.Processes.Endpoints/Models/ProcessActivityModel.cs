// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.ControlSystem.Activities;
using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Processes.Endpoints
{
    [DataContract]
    public class ProcessActivityModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public ActivityResourceModel Resource { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public ActivityClassification Classification { get; set; }

        [DataMember]
        public string RequiredCapabilities { get; set; }

        [DataMember]
        public ActivityResourceModel[] PossibleResources { get; set; }

        [DataMember]
        public long? Result { get; set; }

        [DataMember]
        public string ResultName { get; set; }

        [DataMember]
        public long? InstanceId { get; set; }

        [DataMember]
        public TracingModel Tracing { get; set; }

        [DataMember]
        public bool IsCompleted { get; set; }
    }

}

