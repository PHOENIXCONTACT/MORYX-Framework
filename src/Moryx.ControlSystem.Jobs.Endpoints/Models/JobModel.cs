// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.ControlSystem.Jobs.Endpoints
{
    [DataContract]
    public class JobModel
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long RecipeId { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string DisplayState { get; set; }

        [DataMember]
        public string Workplan { get; set; }

        [DataMember]
        public bool CanComplete { get; set; }

        [DataMember]
        public bool CanAbort { get; set; }

        [DataMember]
        public bool IsWaiting { get; set; }

        [DataMember]
        public bool IsRunning { get; set; }

        [DataMember]
        public ProductionJobModel ProductionJob { get; set; }

        [DataMember]
        public SetupJobModel SetupJob { get; set; }
    }
}

