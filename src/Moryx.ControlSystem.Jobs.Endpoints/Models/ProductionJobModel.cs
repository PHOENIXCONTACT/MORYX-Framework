// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Jobs.Endpoints
{
    [DataContract]
    public class ProductionJobModel
    {
        [DataMember]
        public string ProductIdentity { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public int SuccessCount { get; set; }

        [DataMember]
        public int FailureCount { get; set; }

        [DataMember]
        public int RunningCount { get; set; }

        [DataMember]
        public int ReworkedCount { get; set; }
    }
}

