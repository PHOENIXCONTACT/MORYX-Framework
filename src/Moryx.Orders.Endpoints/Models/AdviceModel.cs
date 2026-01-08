// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models
{
    [DataContract]
    public class AdviceModel
    {
        [DataMember]
        public string ToteBoxNumber { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public long? PartId { get; set; }
    }
}

