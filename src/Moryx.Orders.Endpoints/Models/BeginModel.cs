// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints.Models
{
    [DataContract]
    public class BeginModel
    {
        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public string? UserId { get; set; }
    }
}

