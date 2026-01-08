// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models
{
    [DataContract]
    public class ReportModel
    {
        [DataMember]
        public ConfirmationType ConfirmationType { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public int SuccessCount { get; set; }

        [DataMember]
        public int FailureCount { get; set; }

        [DataMember]
        public string UserIdentifier { get; set; }
    }
}
