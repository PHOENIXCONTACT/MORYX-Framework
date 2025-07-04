// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints
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

