﻿// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders.Endpoints
{
    [DataContract]
    public class DocumentModel
    {
        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public short Revision { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string ContentType { get; set; }

        [DataMember]
        public string Source { get; set; }
    }
}
